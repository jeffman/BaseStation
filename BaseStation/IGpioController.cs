using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    /// <summary>
    /// Wraps the functionality of a <c>GpioController</c>.
    /// </summary>
    public interface IGpioController : IDisposable
    {
        PinNumberingScheme NumberingScheme { get; }
        int PinCount { get; }

        void ClosePin(int pinNumber);
        PinMode GetPinMode(int pinNumber);
        bool IsPinModeSupported(int pinNumber, PinMode mode);
        bool IsPinOpen(int pinNumber);
        void OpenPin(int pinNumber);
        void OpenPin(int pinNumber, PinMode mode);
        PinValue Read(int pinNumber);
        void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback);
        void SetPinMode(int pinNumber, PinMode mode);
        void UnregisterCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback);
        WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken);
        ValueTask<WaitForEventResult> WaitForEventAsync(int pinNumber, PinEventTypes eventTypes, CancellationToken token);
        void Write(ReadOnlySpan<PinValuePair> pinValuePairs);
        void Write (int pinNumber, PinValue value);
    }
}
