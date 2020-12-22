using System;
using System.Linq;
using System.Threading.Tasks;

namespace BaseStation
{
    public class DisplayDriver
    {
        public LedDriver LedDriver { get; }

        public DisplayDriver(LedDriver ledDriver)
        {
            ledDriver.ThrowIfNull(nameof(ledDriver));
            LedDriver = ledDriver;
        }

        public void WriteFrame(DisplayFrame frame)
        {
            frame.ThrowIfNull(nameof(frame));

            foreach (var character in frame.Characters)
            {
                LedDriver.WriteByte(character.Value);
            }

            for (int i = 0; i < 5; i++)
            {
                LedDriver.WriteBit(false);
            }

            LedDriver.WriteBit(frame.StatusLeds[StatusLed.Green]);
            LedDriver.WriteBit(frame.StatusLeds[StatusLed.Blue]);
            LedDriver.WriteBit(frame.StatusLeds[StatusLed.Red]);

            LedDriver.Latch();
        }

        public void WriteString(string str)
        {
            WriteFrame(DisplayFrame.FromString(str.PadLeft(3, ' ')));
        }

        public async Task ScrollString(string str, int interval)
        {
            if (str.Length < 4)
            {
                WriteString(str);
                return;
            }

            // Three frames of intro
            for (int i = 0; i < 3; i++)
            {
                string intro = (new string(' ', 3 - i)) + str.Substring(0, i);
                WriteString(intro);
                await Task.Delay(interval);
            }

            // Show each character
            for (int i = 0; i < str.Length - 2; i++)
            {
                string chunk = str.Substring(i, 3);
                WriteString(chunk);
                await Task.Delay(interval);
            }

            // Three frames of outtro
            for (int i = 0; i < 3; i++)
            {
                string outtro = str.Substring(str.Length - 2 + i) + (new string(' ', i + 1));
                WriteString(outtro);
                await Task.Delay(interval);
            }
        }

        public void WriteDecimal(decimal value, StatusLed signLed = StatusLed.Blue)
        {
            WriteFrame(DisplayFrame.FromDecimal(value, signLed));
        }

        public async Task BusyLoop(int interval, int duration)
        {
            using (var controller = new DisplayLoopController())
            {
                await controller.StartNewLoop(new BusyDisplayLoop(this, interval));
                await Task.Delay(duration);
            }
        }

        public void Clear()
        {
            WriteFrame(DisplayFrame.Empty);
        }
    }
}