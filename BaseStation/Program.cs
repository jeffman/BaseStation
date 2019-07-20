using System;
using System.Linq;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ledDriver = new LedDriver(() => new GpioController(PinNumberingScheme.Logical));
            ledDriver.SetOutputEnabled(true);

            var displayDriver = new DisplayDriver(ledDriver);

            using (var loopController = new DisplayLoopController())
            {
                await loopController.StartNewLoop(new BusyDisplayLoop(displayDriver, 100));
                await Task.Delay(3000);
                await loopController.StartNewLoop(DisplayFrame.FromDecimal(1.23m, StatusLed.Blue).ToDisplayLoop(displayDriver));
                await Task.Delay(1000);
                await loopController.StartNewLoop(new BusyDisplayLoop(displayDriver, 200));
                await Task.Delay(2000);
                await loopController.StartNewLoop(DisplayFrame.FromString("yay").ToDisplayLoop(displayDriver));
                await Task.Delay(1000);
            }

            displayDriver.ClearDisplay();

            ledDriver.Dispose();
        }

        static void Log(string str)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] {str}");
        }
    }
}
