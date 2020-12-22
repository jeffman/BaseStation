using System;
using System.Device.Gpio;

namespace BaseStation
{
    public class LedDriver : IDisposable
    {
        public IGpioController Controller { get; }
        public LedDriverSettings Settings { get; }
        private bool ownsController = false;

        public LedDriver(IGpioController controller)
            : this(controller, new LedDriverSettings())
        { }

        public LedDriver(IGpioController controller, LedDriverSettings settings)
        {
            controller.ThrowIfNull(nameof(controller));
            settings.ThrowIfNull(nameof(settings));
            Controller = controller;
            Settings = settings;
            SetUpController();
        }

        public static LedDriver Create() => Create(new LedDriverSettings());

        public static LedDriver Create(LedDriverSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));
            var controller = new GpioControllerWrapper(new GpioController(PinNumberingScheme.Logical));
            var driver = new LedDriver(controller, settings);
            driver.ownsController = true;
            return driver;
        }

        protected virtual void SetUpController()
        {
            Controller.OpenPin(Settings.DataInPin, PinMode.Output);
            Controller.OpenPin(Settings.ClockPin, PinMode.Output);
            Controller.OpenPin(Settings.LatchPin, PinMode.Output);
            Controller.OpenPin(Settings.EnablePin, PinMode.Output);
            SetOutputEnabled(true);
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

        protected virtual void SetOutputEnabled(bool enabled)
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
                    if (ownsController)
                    {
                        Controller.Dispose();
                    }
                    _isDisposed = true;
                }
            }
        }

        #endregion
    }
}
