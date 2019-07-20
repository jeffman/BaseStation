using System;
using System.Device.Gpio;

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

        public static StaticDisplayLoop ToDisplayLoop(this DisplayFrame frame, DisplayDriver driver)
            => new StaticDisplayLoop(driver, frame);

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
        /// <param name="digitIndex">digit index. Positive indices count leftward from the decimal point;
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
    }
}
