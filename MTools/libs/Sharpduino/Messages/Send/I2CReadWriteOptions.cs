namespace Sharpduino.Messages.Send
{
    public enum I2CReadWriteOptions
    {
        Write = 0,
        ReadOnce = 1 << 3,
        ReadContinuously = 2 << 3,
        StopReading = 3 << 3
    }
}