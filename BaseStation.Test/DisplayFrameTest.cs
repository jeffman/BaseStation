using NUnit.Framework;
using BaseStation;

namespace BaseStation.Test
{
    public class DisplayFrameTest
    {
        [Test]
        public void Decimal_Zero()
        {
            var frame = CreateFrameDecimal(0m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(0),
                frame);
        }

        [Test]
        public void Decimal_ZeroPointZero()
        {
            // The decimal type distinguishes from 0, 0.0, 0.00, and so on
            var frame = CreateFrameDecimal(0.0m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(0),
                frame);
        }

        [Test]
        public void Decimal_ZeroPointZeroZero()
        {
            // The decimal type distinguishes from 0, 0.0, 0.00, and so on. But we don't show the decimal places
            var frame = CreateFrameDecimal(0.00m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(0),
                frame);
        }

        [Test]
        public void Decimal_NegativeZero()
        {
            // While decimal distinguishes between 0 and -0, we display them without the sign always
            var frame = CreateFrameDecimal(-0m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(0),
                frame);
        }

        [Test]
        public void Decimal_PositiveOneIntegralOneFractional()
        {
            var frame = CreateFrameDecimal(1.2m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(1, true),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_PositiveOneIntegralTwoFractionals()
        {
            var frame = CreateFrameDecimal(1.23m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromDigit(1, true),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_PositiveTwoIntegralZeroFractionals()
        {
            var frame = CreateFrameDecimal(12m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_PositiveTwoIntegralOneFractional()
        {
            var frame = CreateFrameDecimal(12.3m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2, true),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_PositiveThreeIntegral()
        {
            var frame = CreateFrameDecimal(123m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_PositiveZeroAndOneFractional()
        {
            var frame = CreateFrameDecimal(0.2m);
            AssertFrame(false, false, false,
                DisplayCharacter.Empty,
                DisplayCharacter.FromDigit(0, true),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_PositiveZeroAndTwoFractionals()
        {
            var frame = CreateFrameDecimal(0.23m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromDigit(0, true),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_NegativeOneIntegralOneFractional()
        {
            var frame = CreateFrameDecimal(-1.2m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromSymbol('-'),
                DisplayCharacter.FromDigit(1, true),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_NegativeOneIntegralTwoFractionals()
        {
            var frame = CreateFrameDecimal(-1.23m);
            AssertFrame(false, false, true,
                DisplayCharacter.FromDigit(1, true),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_NegativeTwoIntegralZeroFractionals()
        {
            var frame = CreateFrameDecimal(-12m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromSymbol('-'),
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_NegativeTwoIntegralOneFractional()
        {
            var frame = CreateFrameDecimal(-12.3m);
            AssertFrame(false, false, true,
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2, true),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_NegativeThreeIntegral()
        {
            var frame = CreateFrameDecimal(-123m);
            AssertFrame(false, false, true,
                DisplayCharacter.FromDigit(1),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        [Test]
        public void Decimal_NegativeZeroAndOneFractional()
        {
            var frame = CreateFrameDecimal(-0.2m);
            AssertFrame(false, false, false,
                DisplayCharacter.FromSymbol('-'),
                DisplayCharacter.FromDigit(0, true),
                DisplayCharacter.FromDigit(2),
                frame);
        }

        [Test]
        public void Decimal_NegativeZeroAndTwoFractionals()
        {
            var frame = CreateFrameDecimal(-0.23m);
            AssertFrame(false, false, true,
                DisplayCharacter.FromDigit(0, true),
                DisplayCharacter.FromDigit(2),
                DisplayCharacter.FromDigit(3),
                frame);
        }

        private static void AssertFrame(
            bool expectedRed,
            bool expectedGreen,
            bool expectedBlue,
            DisplayCharacter expectedLeft,
            DisplayCharacter expectedMiddle,
            DisplayCharacter expectedRight,
            DisplayFrame actual
        )
        {
            Assert.AreEqual(expectedRed, actual.StatusLeds[StatusLed.Red]);
            Assert.AreEqual(expectedGreen, actual.StatusLeds[StatusLed.Green]);
            Assert.AreEqual(expectedBlue, actual.StatusLeds[StatusLed.Blue]);
            Assert.AreEqual(expectedLeft.Value, actual.Characters[2].Value);
            Assert.AreEqual(expectedMiddle.Value, actual.Characters[1].Value);
            Assert.AreEqual(expectedRight.Value, actual.Characters[0].Value);
        }

        private static DisplayFrame CreateFrameDecimal(decimal value)
            => DisplayFrame.FromDecimal(value, StatusLed.Blue);
    }
}
