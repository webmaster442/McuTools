using System;
using Sharpduino.Base;
using Sharpduino.Constants;
using Sharpduino.Exceptions;
using Sharpduino.Messages;
using Sharpduino.Messages.TwoWay;

namespace Sharpduino.Handlers
{
	public class DigitalMessageHandler : BaseMessageHandler<DigitalMessage>
	{
		private enum HandlerState
		{
			StartEnd,
			LSB,
			MSB
		}

		private HandlerState currentHandlerState;        
		private byte LSBCache;
			

		public DigitalMessageHandler(IMessageBroker messageBroker) : base(messageBroker)
		{
			START_MESSAGE = 0x90;
		}

	    protected override void OnResetHandlerState()
	    {
            currentHandlerState = HandlerState.StartEnd;
	    }

	    public override bool CanHandle(byte firstByte)
		{
			switch (currentHandlerState)
			{
				case HandlerState.StartEnd:
					return (firstByte & MessageConstants.MESSAGETYPEMASK) == START_MESSAGE;
				case HandlerState.LSB:
				case HandlerState.MSB:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override bool Handle(byte messageByte)
		{
			if (!CanHandle(messageByte))
			{
				Reset();
				throw new MessageHandlerException("Error with the incoming byte. This is not a valid DigitalMessage");
			}

			switch (currentHandlerState)
			{
				case HandlerState.StartEnd:
					message.Port = messageByte & MessageConstants.MESSAGEPINMASK;
					currentHandlerState = HandlerState.LSB;
					return true;
				case HandlerState.LSB:
					LSBCache = messageByte;
					currentHandlerState = HandlerState.MSB;
					return true;
				case HandlerState.MSB:
					message.PinStates = BitHelper.PortVal2PinVals((byte) BitHelper.BytesToInt(LSBCache, messageByte));
					messageBroker.CreateEvent(message);
					Reset();
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}

