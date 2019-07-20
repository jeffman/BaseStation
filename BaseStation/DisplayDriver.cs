using System;
using System.Linq;

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

        public void ShowFrame(DisplayFrame frame)
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

        public void ClearDisplay()
        {
            ShowFrame(new DisplayFrame());
        }
    }
}