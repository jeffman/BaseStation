using NUnit.Framework;
using BaseStation;
using Moq;
using System.Device.Gpio;

namespace BaseStation.Test
{
    public class LedDriverTest
    {
        private Mock<IGpioController> mock;

        private static LedDriverSettings driverSettings = new LedDriverSettings
        {
            DataInPin = 1,
            ClockPin = 2,
            LatchPin = 3,
            EnablePin = 4
        };

        [SetUp]
        public void SetUp()
        {
            mock = CreateMock();
        }

        [TearDown]
        public void TearDown()
        {
            mock.Object.Dispose();
        }

        [Test]
        public void StartupOpensPinsForOutput()
        {
            mock.Setup(g => g.OpenPin(driverSettings.DataInPin, PinMode.Output)).Verifiable();
            mock.Setup(g => g.OpenPin(driverSettings.ClockPin, PinMode.Output)).Verifiable();
            mock.Setup(g => g.OpenPin(driverSettings.LatchPin, PinMode.Output)).Verifiable();
            mock.Setup(g => g.OpenPin(driverSettings.EnablePin, PinMode.Output)).Verifiable();
            using (var driver = new LedDriver(mock.Object, driverSettings))
            {
                mock.Verify();
            }
        }

        [Test]
        public void StartupEnablesOutput()
        {
            mock.Setup(g => g.Write(driverSettings.EnablePin, PinValue.Low)).Verifiable();
            using (var driver = new LedDriver(mock.Object, driverSettings))
            {
                mock.Verify();
            }
        }

        [Test]
        public void DisposeClosesPins()
        {
            mock.Setup(g => g.ClosePin(driverSettings.DataInPin)).Verifiable();
            mock.Setup(g => g.ClosePin(driverSettings.ClockPin)).Verifiable();
            mock.Setup(g => g.ClosePin(driverSettings.LatchPin)).Verifiable();
            mock.Setup(g => g.ClosePin(driverSettings.EnablePin)).Verifiable();
            using (var driver = new LedDriver(mock.Object, driverSettings)) { }
            mock.Verify();
        }

        [Test]
        public void DisposeDisablesOutput()
        {
            mock.Setup(g => g.Write(driverSettings.EnablePin, PinValue.High)).Verifiable();
            using (var driver = new LedDriver(mock.Object, driverSettings)) { }
            mock.Verify();
        }

        private static Mock<IGpioController> CreateMock()
        {
            var mock = new Mock<IGpioController>();
            return mock;
        }
    }
}
