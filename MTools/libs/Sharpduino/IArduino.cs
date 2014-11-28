using Sharpduino.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpduino
{
    public interface IArduino<DigitalPins, AnalogPins, PwmPins>
    {
        bool IsInitialized { get; }
        void SetDO(DigitalPins pin, bool newValue);
        void SetPinMode(DigitalPins pin, PinModes mode);
        void SetPWM(PwmPins pin, int newValue);
        void SetServo(DigitalPins pin, int newValue);
        void SetSamplingInterval(int milliseconds);
        Pin GetCurrentPinState(DigitalPins pin);
        float ReadAnalog(AnalogPins pin);
        int ReadDigital(DigitalPins pin);
    }
}
