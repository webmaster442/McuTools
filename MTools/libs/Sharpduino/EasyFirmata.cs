using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.EventArguments;
using Sharpduino.Messages;
using Sharpduino.Messages.Receive;
using Sharpduino.Messages.Send;
using Sharpduino.Messages.TwoWay;
using Sharpduino.SerialProviders;

namespace Sharpduino
{
    /// <summary>
    /// This is an easy to use master for an board running Standard Firmata 2.3 
    /// (bundled with Arduino 1.0 software)
    /// </summary>
    public class EasyFirmata : FirmataBase, IHandleAllMessages
    {
        private enum InitializationStages
        {
            QueryProtocolVersion = 0,
            QueryFirmwareVersion,
            QueryCapabilities,
            QueryAnalogMappings,
            QueryPinStates,
            StartReports,
            FullyInitialized
        }

        private InitializationStages currentInitState;

        /***********************************************************************************************/
        //                                       PROPERTIES                                            //
        /***********************************************************************************************/
        #region Properties

        /// <summary>
        /// This is true if we have finished the first communications with the board
        /// to setup the main functionality. The EasyFirmata can be used when this is true
        /// </summary>
        public bool IsInitialized
        {
            get { return currentInitState == InitializationStages.FullyInitialized; }
        }

        /// <summary>
        /// The pins available
        /// </summary>
        public List<Pin> Pins { get; private set; }

        /// <summary>
        /// The analog pins of the board
        /// </summary>
        public List<Pin> AnalogPins { get; private set; }

        /// <summary>
        /// The protocol version that the board uses to communicate
        /// </summary>
        public string ProtocolVersion { get; private set; }

        /// <summary>
        /// The firmware version that the board is running
        /// </summary>
        public string Firmware { get; private set; }

        #endregion

        /***********************************************************************************************/
        //                                         EVENTS                                              //
        /***********************************************************************************************/
        #region Events

        /// <summary>
        /// This event marks the end of the initialization procedure
        /// The EasyFirmata is usable now
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Event to notify about new analog values
        /// </summary>
        public event EventHandler<NewAnalogValueEventArgs> NewAnalogValue;

        /// <summary>
        /// Event that is raised when a digital message is received 
        /// </summary>
        public event EventHandler<NewDigitalValueEventArgs> NewDigitalValue;

        /// <summary>
        /// Event that is raised when a string message is received
        /// </summary>
        public event EventHandler<NewStringMessageEventArgs> NewStringMessage;

        /// <summary>
        /// Event that is raised when we receive a message about the state of a pin
        /// Usually in response to a PinStateQueryMessage
        /// </summary>
        public event EventHandler<PinStateEventArgs> PinStateReceived;

        /// <summary>
        /// Event that is raised when we receive an I2C message as a response to a previous request
        /// </summary>
        public event EventHandler<NewI2CMessageEventArgs> NewI2CMessage;

        #endregion

        /***********************************************************************************************/
        //                              CONSTRUCTOR - INITIALIZATION                                   //
        /***********************************************************************************************/
        #region Constructor - Initialization
        
        public EasyFirmata(ISerialProvider serialProvider): base(serialProvider)
        {
            // Initialize the objects
            Pins = new List<Pin>();
            AnalogPins = new List<Pin>();
            
            // Subscribe ourselves to the message broker
            MessageBroker.Subscribe(this);

            // Start the init procedure
            ReInit();
        }

        /// <summary>
        /// Do the initialization from the start
        /// </summary> 
        public void ReInit()
        {
            currentInitState = 0;
            AdvanceInitialization();
        }

        /// <summary>
        /// Go through the initialization procedure
        /// </summary>
        private void AdvanceInitialization()
        {
            // Do nothing if we are initialized
            if ( currentInitState == InitializationStages.FullyInitialized)
                return;

            switch (currentInitState)
            {
                case InitializationStages.QueryProtocolVersion:
                    // This is the first inistialization stage
                    // Stop any previous reports
                    StopReports();
                    base.SendMessage(new ProtocolVersionRequestMessage());
                    break;
                case InitializationStages.QueryFirmwareVersion:
                    base.SendMessage(new QueryFirmwareMessage());
                    break;
                case InitializationStages.QueryCapabilities:
                    // Clear the pins, as we will be receiving new ones
                    Pins.Clear();
                    AnalogPins.Clear();
                    // Send the message to get the capabilities
                    base.SendMessage(new QueryCapabilityMessage());
                    break;
                case InitializationStages.QueryAnalogMappings:
                    base.SendMessage(new AnalogMappingQueryMessage());
                    break;
                case InitializationStages.QueryPinStates:
                    for (int i = 0; i < Pins.Count; i++)
                    {
                        base.SendMessage(new PinStateQueryMessage{Pin = (byte) i});
                    }
                    break;
                case InitializationStages.StartReports:
                    var portsCount = (byte) Math.Ceiling(Pins.Count/8.0);
                    for (byte i = 0; i < portsCount; i++)
                    {
                        base.SendMessage(new ToggleDigitalReportMessage() { Port = i, ShouldBeEnabled = true });
                    }

                    for (byte i = 0; i < AnalogPins.Count; i++)
                    {
                        base.SendMessage(new ToggleAnalogReportMessage() { Pin = i, ShouldBeEnabled = true });
                    }

                    // There is no callback for the above messages so advance anyway                    
                    OnInitialized();
                    break;
                    case InitializationStages.FullyInitialized:
                    // Do nothing we are finished with the initialization
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage");
            }

            // Go to the next state
            if ( !IsInitialized)
                currentInitState++;
        }
        
        #endregion

        /***********************************************************************************************/
        //                               INCOMING MESSAGE HANDLING                                     //
        /***********************************************************************************************/
        #region Incoming Message Handling

        /// <summary>
        /// Handle the Protocol Message. Contains info about the protocol that the board is using to communicate
        /// </summary>
        public void Handle(ProtocolVersionMessage message)
        {
            if ( IsInitialized )return;
            ProtocolVersion = string.Format("{0}.{1}", message.MajorVersion, message.MinorVersion);
            AdvanceInitialization();
        }

        /// <summary>
        /// Handle the Firmware Message. Contains info about the firmware running in the board
        /// </summary>
        public void Handle(SysexFirmwareMessage message)
        {
            if ( IsInitialized ) return;
            Firmware = string.Format("{0}:{1}.{2}", message.FirmwareName, message.MajorVersion, message.MinorVersion);
            AdvanceInitialization();
        }        

        /// <summary>
        /// Handle the capability messages. There will be one such message for each pin
        /// </summary>
        public void Handle(CapabilityMessage message)
        {
            var pin = new Pin();
            foreach (var mes in message.Modes)
                pin.Capabilities[mes.Key] = mes.Value;

            // Add it to the collection
            Pins.Add(pin);
        }

        /// <summary>
        /// Handle the Capabilities Finished Message. This is used to advance to the next step of
        /// the initialization after the capabilities
        /// </summary>
        public void Handle(CapabilitiesFinishedMessage message)
        {
            // If we haven't initialized then do the next thing in the init procedure
            if ( !IsInitialized )
                AdvanceInitialization();

            // Otherwise this message conveys no information
        }
        
        /// <summary>
        /// Handle the Analog Mapping Message. This is used to find out which pins have 
        /// analog input capabilities and fill the AnalogPins list
        /// </summary>
        public void Handle(AnalogMappingMessage message)
        {
            if (IsInitialized) return;
            
            for (int i = 0; i < message.PinMappings.Count; i++)
            {
                // If we have an analog pin
                if ( message.PinMappings[i] != 127 )
                {
                    // Put the corresponding pin to the analog pins dictionary
                    // this is a reference, so any changes to the primary object
                    // will be reflected here too.
                    AnalogPins.Add(Pins[i]);
                }
            }
            AdvanceInitialization();
        }

        /// <summary>
        /// Handler the Pin State Message. Get more information about each pin.
        /// This is called multiple times and we advance to the next step, only after
        /// we have received information about the last pin
        /// </summary>
        public void Handle(PinStateMessage message)
        {            
            Pin currentPin = Pins[message.PinNo];
            currentPin.CurrentMode = message.Mode;
            currentPin.CurrentValue = message.State;
            
            if (IsInitialized) return;

            // Notify others only when we are fully initialized
            OnPinStateReceived(message);

            // here we check to see if we have finished with the PinState Messages
            // and advance to the next step. Test the following:
            if ( message.PinNo == Pins.Count - 1 )
                AdvanceInitialization();
        }

        /// <summary>
        /// Handle the Analog Messsage. Update the value for the pin and raise a
        /// NewAnalogValue event
        /// </summary>
        public void Handle(AnalogMessage message)
        {
            // Here we are in the twilight zone
            if (currentInitState <= InitializationStages.QueryPinStates )
                return;

            // First save the value in the Pins and AnalogPins lists
            AnalogPins[message.Pin].CurrentValue = message.Value;

            OnNewAnalogValue(message.Pin,message.Value);
        }

        /// <summary>
        /// Handle the Digital Message. Update the values for the pins of the port
        /// and raise a NewDigitalValue event
        /// </summary>
        /// <param name="message"></param>
        public void Handle(DigitalMessage message)
        {
            var pinStart = (byte)(8*message.Port);
            for (byte i = 0; i < 8; i++)
            {
               int index = i + pinStart;
               if (index < Pins.Count) Pins[index].CurrentValue = message.PinStates[i] ? 1 : 0;
            }

            OnNewDigitalValue(message.Port,message.PinStates);
        }

        /// <summary>
        /// Handle the Sysex String Message. Raise a NewStringMessage event
        /// </summary>
        /// <param name="message"></param>
        public void Handle(SysexStringMessage message)
        {
            OnNewStringMessage(message.Message);
        }

        public void Handle(I2CResponseMessage message)
        {
            OnI2CMessageReceived(message);
        }
        #endregion

        /***********************************************************************************************/
        //                                  EVENTS CREATION                                            //
        /***********************************************************************************************/
        #region Event Creation

        private void OnInitialized()
        {
            var handler = Initialized;
            if ( handler != null )
            {
                handler(this,new EventArgs());
            }
        }

        private void OnNewAnalogValue(byte pin, int value)
        {
            var handler = NewAnalogValue;
            if ( handler != null )
            {
                handler(this, new NewAnalogValueEventArgs() { AnalogPin = pin, NewValue = value });
            }
        }

        private void OnNewDigitalValue(int port,bool[] pins)
        {
            var handler = NewDigitalValue;
            if ( handler != null )
            {
                handler(this, new NewDigitalValueEventArgs() { Port = port, Pins = pins });
            }
        }

        private void OnNewStringMessage(string message)
        {
            var handler = NewStringMessage;
            if ( handler != null )
            {
                handler(this, new NewStringMessageEventArgs() { Message = message });
            }
        }

        private void OnPinStateReceived(PinStateMessage message)
        {
            var handler = PinStateReceived;
            if ( handler != null)
            {
                handler(this, new PinStateEventArgs() {Pin = message.PinNo, Mode = message.Mode, Value = message.State});
            }
        }

        private void OnI2CMessageReceived(I2CResponseMessage message)
        {
            var handler = NewI2CMessage;
            if ( handler != null )
            {
                handler(this,new NewI2CMessageEventArgs(){Data = message.Data, Register = message.Register, SlaveAddress = message.SlaveAddress});
            }
        }
        #endregion

        /// <summary>
        /// Get the current values for a digital port. It is useful for creating a DigitalMessage
        /// </summary>
        /// <param name="port">The port whose values we want</param>
        /// <returns>A bool array representing the current state of each pin</returns>
        public bool[] GetDigitalPortValues(int port)
        {
            var values = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                // Even if we have analog values ie > 1 we put 0 as it doesn't matter
                // from the board side. They will be ignored anyway
                if ( port * 8 + i < Pins.Count )
                    values[i] = Pins[port*8 + i].CurrentValue == 1 ? true : false;
            }

            return values;
        }

        /// <summary>
        /// Stop receiving reports.
        /// </summary>
        private void StopReports()
        {
            for (byte i = 0; i < MessageConstants.MAX_DIGITAL_PORTS; i++)
            {
                base.SendMessage(new ToggleDigitalReportMessage() { Port = i, ShouldBeEnabled = false });
            }

            for (byte i = 0; i < AnalogPins.Count; i++)
            {
                base.SendMessage(new ToggleAnalogReportMessage() { Pin = i, ShouldBeEnabled = false });
            }
        }

        /// <summary>
        /// Send a message to the firmata board. 
        /// Take care: if the EasyFirmata hasn't finished with the initialization this will do nothing
        /// </summary>
        public override void SendMessage<T>(T message)
        {
            if ( !IsInitialized )
                return;
            base.SendMessage(message);
        }

        protected override void Dispose(bool shouldDispose)
        {
            try
            { 
                StopReports();
            }
            catch (Exception) { }

            base.Dispose(shouldDispose);
        }
    }
}