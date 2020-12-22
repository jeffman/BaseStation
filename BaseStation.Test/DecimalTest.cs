using NUnit.Framework;

namespace BaseStation.Test
{
    public class DecimalTest
    {
        [Test]
        public void NumIntegerDigits_ZeroReturnsOne()
        {
            Assert.AreEqual(1, 0m.NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_NineReturnsOne()
        {
            Assert.AreEqual(1, 9m.NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_TenReturnsTwo()
        {
            Assert.AreEqual(2, 10m.NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_NineWithDecimalsReturnsOne()
        {
            Assert.AreEqual(1, 9.9m.NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_NineWithManyDecimalsReturnsOne()
        {
            Assert.AreEqual(1, 9.999999999999999999999999999m.NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_AllPowersOfTen([Range(0, 28, 1)] int exponent)
        {
            Assert.AreEqual(exponent + 1, 10m.Pow(exponent).NumIntegerDigits());
        }

        [Test]
        public void NumIntegerDigits_AllPowersOfTenMinusOne([Range(1, 28, 1)] int exponent)
        {
            Assert.AreEqual(exponent, (10m.Pow(exponent) - 1).NumIntegerDigits());
        }

        [Test]
        public void NumFractionalDigits_ZeroReturnsZero()
        {
            Assert.AreEqual(0, 0m.NumFractionalDigits());
        }

        [Test]
        public void NumFractionalDigits_OneReturnsZero()
        {
            Assert.AreEqual(0, 1m.NumFractionalDigits());
        }

        [Test]
        public void NumFractionalDigits_0_1_ReturnsOne()
        {
            Assert.AreEqual(1, 0.1m.NumFractionalDigits());
        }

        [Test]
        public void NumFractionalDigits_0_10_ReturnsOne()
        {
            Assert.AreEqual(1, 0.10m.NumFractionalDigits());
        }

        [Test]
        public void NumFractionalDigits_0_01_ReturnsTwo()
        {
            Assert.AreEqual(2, 0.01m.NumFractionalDigits());
        }

        [Test]
        public void NumFractionalDigits_0_1001_ReturnsFour()
        {
            Assert.AreEqual(4, 0.1001m.NumFractionalDigits());
        }

        [Test]
        public void GetDigit_0_0_ReturnsZero()
        {
            Assert.AreEqual(0, 0m.GetDigit(0));
        }

        [Test]
        public void GetDigit_0_1_ReturnsZero()
        {
            Assert.AreEqual(0, 0m.GetDigit(1));
        }

        [Test]
        public void GetDigit_0_n1_ReturnsZero()
        {
            Assert.AreEqual(0, 0m.GetDigit(-1));
        }

        [Test]
        public void GetDigit_1_0_ReturnsOne()
        {
            Assert.AreEqual(1, 1m.GetDigit(0));
        }

        [Test]
        public void GetDigit_10_0_ReturnsZero()
        {
            Assert.AreEqual(0, 10m.GetDigit(0));
        }

        [Test]
        public void GetDigit_10_1_ReturnsOne()
        {
            Assert.AreEqual(1, 10m.GetDigit(1));
        }

        [Test]
        public void GetDigit_0d1_0_ReturnsZero()
        {
            Assert.AreEqual(0, 0.1m.GetDigit(0));
        }

        [Test]
        public void GetDigit_0d1_n1_ReturnsOne()
        {
            Assert.AreEqual(1, 0.1m.GetDigit(-1));
        }

        [Test]
        public void GetDigit_0d1_n2_ReturnsZero()
        {
            Assert.AreEqual(0, 0.1m.GetDigit(-2));
        }

        [Test]
        public void GetDigit_0d01_n2_ReturnsOne()
        {
            Assert.AreEqual(1, 0.01m.GetDigit(-2));
        }
    }
}