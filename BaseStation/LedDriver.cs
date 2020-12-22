using System;
using System.Device.Gpio;

namespace BaseStation
{
    public class LedDriver : IDisposable
    {
        public GpioController Controller { get; }
        public LedDriverSettings Settings { get; }

        public LedDriver(Func<GpioController> controllerFactory)
            : this(controllerFactory, new LedDriverSettings())
        { }

        public LedDriver(Func<GpioController> controllerFactory, LedDriverSettings settings)
        {
            controllerFactory.ThrowIfNull(nameof(controllerFactory));
            settings.ThrowIfNull(nameof(settings));

            Controller = controllerFactory();
            if (Controller == null)
               throw new ArgumentNullException("Factory cannot generate null values", nameof(controllerFactory));

            Settings = settings;
            SetUpController();
        }

        protected virtual void SetUpController()
        {
            Controller.OpenPin(Settings.DataInPin, PinMode.Output);
            Controller.OpenPin(Settings.ClockPin, PinMode.Output);
            Controller.OpenPin(Settings.LatchPin, PinMode.Output);
            Controller.OpenPin(Settings.EnablePin, PinMode.Output);
            SetOutputEnabled(false);
        }

        protected virtual void TearDownController()
        {
            SetOutputEnabled(false);
            Controller.ClosePin(Settings.DataInPin);
            Controller.ClosePin(Settings.ClockPin);
            Controller.ClosePin(Settings.LatchPin);
            Controller.ClosePin(Settings.EnablePin);
        }

        public void WriteBit(PinValue value)
        {
            Controller.Write(Settings.DataInPin, value);
            PulseWait();
            Controller.Write(Settings.ClockPin, PinValue.High);
            PulseWait();
            Controller.Write(Settings.ClockPin, PinValue.Low);
            PulseWait();
        }

        public void Latch()
        {
            Controller.Write(Settings.LatchPin, PinValue.High);
            PulseWait();
            Controller.Write(Settings.LatchPin, PinValue.Low);
            PulseWait();
        }

        public void SetOutputEnabled(bool enabled)
        {
            Controller.Write(Settings.EnablePin, enabled ? PinValue.Low : PinValue.High);
            PulseWait();
        }

        protected virtual void PulseWait()
        {
            Helpers.WaitMicro(10);
        }

        #region IDisposable

        private bool _isDisposed = false;

        ~LedDriver()
        {
            System.Diagnostics.Debug.Assert(_isDisposed, "Dispose not called");
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isDisposed)
                {
                    TearDownController();
                    _isDisposed = true;
                }
            }
        }

        #endregion
    }
}
