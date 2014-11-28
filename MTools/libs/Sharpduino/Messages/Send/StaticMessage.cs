namespace Sharpduino.Messages.Send
{
    public class StaticMessage
    {
        public byte[] Bytes { get; private set; }

        protected StaticMessage(params byte[] Bytes)
        {
            this.Bytes = Bytes;
        }
    }
}