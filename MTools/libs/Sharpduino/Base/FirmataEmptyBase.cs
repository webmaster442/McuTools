using System;
using System.Collections.Generic;
using System.Threading;
using Sharpduino.Creators;
using Sharpduino.EventArguments;
using Sharpduino.Exceptions;
using Sharpduino.Handlers;
using Sharpduino.SerialProviders;

namespace Sharpduino.Base
{
    /// <summary>
    /// This is the base class for the firmata protocol. It is a bare bones implementation
    /// that should be overriden to provide any functionality. It has no message handling.
    /// Useful if you want to implement your own subset of the firmata protocol.
    /// </summary>
    public abstract class FirmataEmptyBase : IDisposable
    {
        /// <summary>
        /// The available handlers for this instance
        /// </summary>
        protected List<IMessageHandler> AvailableHandlers { get; private set; }

        /// <summary>
        /// A list of the appropriate handlers for the current message
        /// </summary>
        protected List<IMessageHandler> AppropriateHandlers { get; private set; }

        /// <summary>
        /// The serial port 
        /// </summary>
        protected ISerialProvider Provider { get; private set; }

        /// <summary>
        /// Incoming Data as a Queue of bytes, which is suitable for the handling mechanism
        /// </summary>
        protected Queue<byte> IncomingData { get; private set; }

        /// <summary>
        /// The messagebroker that handles all the incoming Message event creation
        /// </summary>
        protected MessageBroker MessageBroker { get; private set; }

        /// <summary>
        /// The messageCreators dictionary that will help create any message we want to send
        /// </summary>
        protected Dictionary<Type, IMessageCreator> MessageCreators { get; private set; }

        private bool processQueue;
        private bool firstTime = true;

        protected FirmataEmptyBase(ISerialProvider provider)
        {
            AvailableHandlers = new List<IMessageHandler>();
            AppropriateHandlers = new List<IMessageHandler>();
            IncomingData = new Queue<byte>();
            MessageCreators = new Dictionary<Type, IMessageCreator>();
            MessageBroker = new MessageBroker();
            this.Provider = provider;
            Initialize();
        }

        /// <summary>
        /// Initialize the comport
        /// </summary>
        private void Initialize()
        {
            if (firstTime)
            {
                // These things should only be done one time

                // Begin the parsing thread
                processQueue = true;
                var t = new ThreadStart(ReceiveQueueThread);
                var thr = new Thread(t);
                thr.Start();

                // Subscribe to the DataReceived messages
                Provider.DataReceived += ProviderDataReceived;
                firstTime = false;
            }
            Provider.Open();            
        }

        private void ProviderDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Just add the received bytes to the incoming data queue
            lock (IncomingData)
            {
                foreach (var b in e.BytesReceived)
                    IncomingData.Enqueue(b);
            }
        }


        #region Proper Dispose Code

        // Proper Dispose code should contain the following. See
        // http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface

        ~FirmataEmptyBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        protected virtual void Dispose(bool shouldDispose)
        {
            if ( shouldDispose )
            {
                // This should allow the parsing thread to close
                processQueue = false;
                // TODO : see if we should wait for the thread or not

                // Dispose of the com port as safely as possible
                if ( Provider != null )
                {
                    Provider.DataReceived -= ProviderDataReceived;
                    Provider.Dispose();
                    Provider = null;
                }
            }
        }

        #endregion


        /// <summary>
        /// Send a message to the board
        /// </summary>
        /// <typeparam name="T">The type of the message we want to send. Implicitly evaluated by the object</typeparam>
        /// <param name="message">The message object</param>
        public virtual void SendMessage<T>(T message)
        {
            var type = typeof(T);
            // Try to see if we have any creators for this type of message
            if (MessageCreators.ContainsKey(type))
            {
                // Use the creator to create the message then transmit it through the port
                var bytes = MessageCreators[type].CreateMessage(message);
                Provider.Send(bytes);
            }
            else
            {
                throw new FirmataException("Not recognizable message");
            }
        }

        /// <summary>
        /// This is the code that gets executed in another thread
        /// and parses all incoming bytes
        /// </summary>
        private void ReceiveQueueThread()
        {
            byte currentByte = 0x0;
            bool foundByteFlag = false;
            // This thread runs while we have the processQueue set
            while (processQueue)
            {             
                // lock the incoming data so we don't have any race confitions
                lock (IncomingData)
                {
                    // take a peek and dequeue the first byte in the queue
                    if (IncomingData.Count > 0)
                    {                    
                        currentByte = IncomingData.Dequeue();
                        foundByteFlag = true;
                    }
                }

                // If we found a byte then Handle it
                if ( foundByteFlag )
                    HandleByte(currentByte);
                else
                    Thread.Sleep(10);

                // reset the flag and the byte value
                foundByteFlag = false;
                currentByte = 0x0;
            }
        }

        /// <summary>
        /// Handle the next byte from the queue
        /// </summary>
        private void HandleByte(byte currentByte)
        {
            // Check to see if we are already handling a message
            if ( AppropriateHandlers.Count > 0 )
            {
                // This is a temporary list to remove handlers after iterating through 
                // the appropriate handlers
                List<IMessageHandler> handlersToRemove = new List<IMessageHandler>();
                foreach (var handler in AppropriateHandlers)
                {
                    // if it can handle the byte then do so
                    if (handler.CanHandle(currentByte))
                    {
                        // if the handler has finished with the message then remove it
                        if (handler.Handle(currentByte) == false)
                        {
                            handlersToRemove.Add(handler);
                        }
                    }
                    // if the handler cannot handle the message remove it
                    else
                        handlersToRemove.Add(handler);
                }

                handlersToRemove.ForEach(x => x.Reset());
                // Remove all handlers we found out earlier
                AppropriateHandlers.RemoveAll(handlersToRemove.Contains);

                return; // do not continue with the new message code
            }

            ///////////////////////////////////////////////
            // If we reach here then we have a new message.
            ///////////////////////////////////////////////
            
            // iterate through all available handlers to see which ones can handle this new message
            foreach (var availableHandler in AvailableHandlers)
            {
                if (availableHandler.CanHandle(currentByte))
                {
                    if (availableHandler.Handle(currentByte))
                        AppropriateHandlers.Add(availableHandler);
                }
            }
        }
    }
}
