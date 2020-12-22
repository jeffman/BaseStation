using System;
using System.Linq;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace BaseStation.Server
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var program = new Program();
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult<Options, Task<int>>(program.Run, errs => Task.FromResult(-1));
        }

        private async Task<int> Run(Options options)
        {
            try
            {
                using (var ledDriver = LedDriver.Create())
                {
                    var displayDriver = new DisplayDriver(ledDriver);
                    Task task;

                    if (options.StringValue != null)
                    {
                        if (options.StringValue.Length <= 3)
                        {
                            displayDriver.WriteString(options.StringValue);
                            task = Task.Delay(options.Duration);
                        }
                        else
                        {
                            // A scroll uses length+6 frames, so each frame should be duration/(length+6)
                            int scrollInterval = options.Duration / (options.StringValue.Length + 6);
                            task = displayDriver.ScrollString(options.StringValue, scrollInterval);
                        }
                    }
                    else if (options.DecimalValue != null)
                    {
                        displayDriver.WriteFrame(DisplayFrame.FromDecimal(options.DecimalValue.Value, StatusLed.Blue));
                        task = Task.Delay(options.Duration);
                    }
                    else if (options.BusyDelay != null)
                    {
                        task = displayDriver.BusyLoop(options.BusyDelay.Value, options.Duration);
                    }
                    else
                    {
                        Console.WriteLine("No option selected.");
                        return -1;
                    }

                    await task;
                    displayDriver.Clear();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return -1;
            }
        }
    }

    class Options
    {
        [Option('s', "string", SetName = "cmd")]
        public string StringValue { get; set; }

        [Option('d', "decimal", SetName = "cmd")]
        public decimal? DecimalValue { get; set; }

        [Option('b', "busy", SetName = "cmd")]
        public int? BusyDelay { get; set; }

        [Option('t', "time", Default = 3000, HelpText = "Duration, in milliseconds.")]
        public int Duration { get; set; }
    }
}
