using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using System.Linq;

namespace BaseStation
{
    public static class Extensions
    {
        public static void ThrowIfNull(this object obj, string name)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException(name);
        }

        public static void WriteByte(this LedDriver driver, byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                int bit = value & 0x80;
                value <<= 1;

                driver.WriteBit(bit != 0);
            }
        }

        public static void WriteBit(this LedDriver driver, bool value)
        {
            driver.WriteBit(value ? PinValue.High : PinValue.Low);
        }

        public static decimal Pow(this decimal value, int exponent)
        {
            if (exponent < 0)
                throw new ArgumentOutOfRangeException(nameof(exponent));

            decimal returnValue = 1m;
            while (exponent > 0)
            {
                returnValue *= value;
                exponent--;
            }

            return returnValue;
        }

        public static int NumIntegerDigits(this decimal value)
        {
            value = Math.Floor(Math.Abs(value));

            int digitCount = 1;

            while (value >= 10m)
            {
                digitCount++;
                value /= 10m;
            }

            return digitCount;
        }

        public static int NumFractionalDigits(this decimal value)
        {
            value = Math.Abs(value);
            value -= Math.Floor(value);

            int digitCount = 0;
            int zeroRun = 0;

            while (value != 0m)
            {
                value *= 10m;
                int integerPart = (int)Math.Floor(value);

                if (integerPart == 0m)
                {
                    zeroRun++;
                }
                else
                {
                    digitCount += zeroRun + 1;
                    zeroRun = 0;
                }

                value -= integerPart;
            }

            return digitCount;
        }

        /// <summary>
        /// Gets the value of a particular digit.
        /// </summary>
        /// <param name="value">value whose digit to get</param>
        /// <param name="digitIndex">digit index. Non-negative indices count leftward from the decimal point;
        /// negative indices count rightward from the decimal point.</param>
        /// <returns></returns>
        public static int GetDigit(this decimal value, int digitIndex)
        {
            if (digitIndex >= 0)
            {
                return (int)((Decimal.Floor(value) / 10m.Pow(digitIndex)) % 10m);
            }
            else
            {
                return (int)(Decimal.Floor((value - Decimal.Floor(value)) * 10m.Pow(-digitIndex)) % 10m);
            }
        }

        public static void WriteString(this DisplayDriver driver, string str)
        {
            driver.WriteFrame(DisplayFrame.FromString(str));
        }

        public static async Task ScrollString(this DisplayDriver driver, string str, int duration)
        {
            var displayString = DisplayFrame.GetString(str).ToArray();
            int scrollInterval = duration / (displayString.Length + 6);

            // Start with a blank frame
            var frame = DisplayFrame.Empty;
            driver.WriteFrame(frame);
            await Task.Delay(scrollInterval);

            // Push each character in from the right, one at a time
            foreach (var c in displayString)
            {
                frame = frame.WithPushedCharacter(c);
                driver.WriteFrame(frame);
                await Task.Delay(scrollInterval);
            }

            // Push three more blank characters to end off with a blank screen
            for (int i = 0; i < 3; i++)
            {
                frame = frame.WithPushedCharacter(DisplayCharacter.Empty);
                driver.WriteFrame(frame);
                await Task.Delay(scrollInterval);
            }
        }

        public static void WriteDecimal(this DisplayDriver driver, decimal value, StatusLed signLed = StatusLed.Blue)
        {
            driver.WriteFrame(DisplayFrame.FromDecimal(value, signLed));
        }

        public static async Task BusyLoop(this DisplayDriver driver, int interval, int duration)
        {
            using (var controller = new DisplayLoopController())
            {
                await controller.StartNewLoop(new BusyDisplayLoop(driver, interval));
                await Task.Delay(duration);
            }
        }

        public static void Clear(this DisplayDriver driver)
        {
            driver.WriteFrame(DisplayFrame.Empty);
        }
    }
}
