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
            WriteFrame(DisplayFrame.FromString(str));
        }

        public async Task ScrollString(string str, int duration)
        {
            var displayString = DisplayFrame.GetString(str).ToArray();
            int scrollInterval = duration / (displayString.Length + 6);

            // Start with a blank frame
            var frame = DisplayFrame.Empty;
            WriteFrame(frame);
            await Task.Delay(scrollInterval);

            // Push each character in from the right, one at a time
            foreach (var c in displayString)
            {
                frame = frame.WithPushedCharacter(c);
                WriteFrame(frame);
                await Task.Delay(scrollInterval);
            }

            // Push three more blank characters to end off with a blank screen
            for (int i = 0; i < 3; i++)
            {
                frame = frame.WithPushedCharacter(DisplayCharacter.Empty);
                WriteFrame(frame);
                await Task.Delay(scrollInterval);
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