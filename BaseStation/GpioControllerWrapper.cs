using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    public class GpioControllerWrapper : IGpioController
    {
        public GpioController Controller { get; }

        public GpioControllerWrapper(GpioController controller)
        {
            controller.ThrowIfNull(nameof(controller));
            Controller = controller;
        }

        public PinNumberingScheme NumberingScheme => Controller.NumberingScheme;
        public int PinCount => Controller.PinCount;

        public void ClosePin(int pinNumber) => Controller.ClosePin(pinNumber);
        public PinMode GetPinMode(int pinNumber) => Controller.GetPinMode(pinNumber);
        public bool IsPinModeSupported(int pinNumber, PinMode mode) => Controller.IsPinModeSupported(pinNumber, mode);
        public bool IsPinOpen(int pinNumber) => Controller.IsPinOpen(pinNumber);
        public void OpenPin(int pinNumber) => Controller.OpenPin(pinNumber);
        public void OpenPin(int pinNumber, PinMode mode) => Controller.OpenPin(pinNumber, mode);
        public PinValue Read(int pinNumber) => Controller.Read(pinNumber);
        public void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback)
            => Controller.RegisterCallbackForPinValueChangedEvent(pinNumber, eventTypes, callback);
        public void SetPinMode(int pinNumber, PinMode mode) => Controller.SetPinMode(pinNumber, mode);
        public void UnregisterCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback)
            => Controller.UnregisterCallbackForPinValueChangedEvent(pinNumber, callback);
        public WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken)
            => Controller.WaitForEvent(pinNumber, eventTypes, cancellationToken);
        public ValueTask<WaitForEventResult> WaitForEventAsync(int pinNumber, PinEventTypes eventTypes, CancellationToken token)
            => Controller.WaitForEventAsync(pinNumber, eventTypes, token);
        public void Write(ReadOnlySpan<PinValuePair> pinValuePairs) => Controller.Write(pinValuePairs);
        public void Write (int pinNumber, PinValue value) => Controller.Write(pinNumber, value);
        public void Dispose() => Controller.Dispose();
    }
}
