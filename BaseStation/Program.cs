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
                        if (options.ForceScroll || DisplayFrame.GetDisplayedStringLength(options.StringValue) > 3)
                        {
                            task = displayDriver.ScrollString(options.StringValue, options.Duration);
                        }
                        else
                        {
                            displayDriver.WriteString(options.StringValue);
                            task = Task.Delay(options.Duration);
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
                    else if (options.RawValue != null)
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            bool bit = ((options.RawValue.Value << i) & 0x80000000u) != 0u;
                            ledDriver.WriteBit(bit);
                        }
                        ledDriver.Latch();
                        task = Task.Delay(options.Duration);
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

        [Option('c', "force-scroll", HelpText = "Force scrolling for short strings.")]
        public bool ForceScroll { get; set; }

        [Option('d', "decimal", SetName = "cmd")]
        public decimal? DecimalValue { get; set; }

        [Option('b', "busy", SetName = "cmd")]
        public int? BusyDelay { get; set; }

        [Option('t', "time", Default = 3000, HelpText = "Duration, in milliseconds.")]
        public int Duration { get; set; }

        [Option('r', "raw", SetName = "cmd", HelpText = "Write raw value to shift registers. Value is a 32-bit unsigned decimal integer. Bits are pushed MSB to LSB.")]
        public uint? RawValue { get; set; }
    }
}
