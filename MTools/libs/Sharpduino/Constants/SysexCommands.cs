namespace Sharpduino.Constants
{
    public static class SysexCommands
    {
        public const byte ANALOG_MAPPING_QUERY = 0x69;
        public const byte ANALOG_MAPPING_RESPONSE = 0x6a;
        public const byte CAPABILITY_QUERY = 0x6b;
        public const byte CAPABILITY_RESPONSE = 0x6c;
        public const byte PIN_STATE_QUERY = 0x6d;
        public const byte PIN_STATE_RESPONSE = 0x6e;
        public const byte EXTENDED_ANALOG = 0x6F;
        public const byte SERVO_CONFIG = 0x70;
        public const byte STRING_DATA = 0x71;
        public const byte PULSE_DATA = 0x74;
        public const byte SHIFT_DATA = 0x75;
        public const byte I2C_REQUEST = 0x76;
        public const byte I2C_REPLY = 0x77;
        public const byte I2C_CONFIG = 0x78;
        public const byte QUERY_FIRMWARE = 0x79;
        public const byte SAMPLING_INTERVAL = 0x7a;
    }
}
