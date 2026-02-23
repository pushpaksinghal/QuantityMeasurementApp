using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightConversionTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testConversion_KilogramToGram()
        {
            double result = Quantity<WeightUnit>.Convert(1.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_GramToKilogram()
        {
            double result = Quantity<WeightUnit>.Convert(1000.0, WeightUnit.GRAM, WeightUnit.KILOGRAM);
            Assert.AreEqual(1.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            double result = Quantity<WeightUnit>.Convert(2.0, WeightUnit.POUND, WeightUnit.KILOGRAM);
            Assert.AreEqual(0.907184, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            double result = Quantity<WeightUnit>.Convert(1.0, WeightUnit.KILOGRAM, WeightUnit.POUND);
            Assert.AreEqual(2.2046226218, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_GramToPound()
        {
            double result = Quantity<WeightUnit>.Convert(500.0, WeightUnit.GRAM, WeightUnit.POUND);
            Assert.AreEqual(1.102311, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_PoundToGram()
        {
            double result = Quantity<WeightUnit>.Convert(1.0, WeightUnit.POUND, WeightUnit.GRAM);
            Assert.AreEqual(453.592, result, 1e-3);
        }

        [TestMethod]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = Quantity<WeightUnit>.Convert(5.0, WeightUnit.KILOGRAM, WeightUnit.KILOGRAM);
            Assert.AreEqual(5.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double result = Quantity<WeightUnit>.Convert(0.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(0.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_NegativeValue_PreservesSign()
        {
            double result = Quantity<WeightUnit>.Convert(-1.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(-1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 123.456;

            double toPound = Quantity<WeightUnit>.Convert(v, WeightUnit.KILOGRAM, WeightUnit.POUND);
            double backToKg = Quantity<WeightUnit>.Convert(toPound, WeightUnit.POUND, WeightUnit.KILOGRAM);

            Assert.AreEqual(v, backToKg, 1e-6);
        }

        [TestMethod]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<WeightUnit>.Convert(double.NaN, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<WeightUnit>.Convert(double.PositiveInfinity, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_NegativeInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<WeightUnit>.Convert(double.NegativeInfinity, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_InvalidEnumValue_Throws()
        {
            WeightUnit bad = (WeightUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Quantity<WeightUnit>.Convert(1.0, bad, WeightUnit.GRAM)
            );
        }
    }
}