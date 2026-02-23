using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;
//UC-5 Unit- To -Unit conversion
namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthConversionTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testConversion_FeetToInch()
        {
            double result = Quantity<LengthUnit>.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(12.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_InchToFeet()
        {
            double result = Quantity<LengthUnit>.Convert(24.0, LengthUnit.INCH, LengthUnit.FEET);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_YardToFeet()
        {
            double result = Quantity<LengthUnit>.Convert(3.0, LengthUnit.YARD, LengthUnit.FEET);
            Assert.AreEqual(9.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_YardToInch()
        {
            double result = Quantity<LengthUnit>.Convert(1.0, LengthUnit.YARD, LengthUnit.INCH);
            Assert.AreEqual(36.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_InchToYard()
        {
            double result = Quantity<LengthUnit>.Convert(72.0, LengthUnit.INCH, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_CentimetersToInch_2Point54cm_Is1Inch()
        {
            // Because your factor is rounded, use slightly looser tolerance
            double result = Quantity<LengthUnit>.Convert(2.54, LengthUnit.CENTIMETERS, LengthUnit.INCH);
            Assert.AreEqual(1.0, result, 1e-4);
        }

        [TestMethod]
        public void testConversion_InchToCentimeters_1Inch_Is2Point54cm()
        {
            double result = Quantity<LengthUnit>.Convert(1.0, LengthUnit.INCH, LengthUnit.CENTIMETERS);
            Assert.AreEqual(2.54, result, 1e-3);
        }

        [TestMethod]
        public void testConversion_FeetToYard()
        {
            double result = Quantity<LengthUnit>.Convert(6.0, LengthUnit.FEET, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = Quantity<LengthUnit>.Convert(5.0, LengthUnit.FEET, LengthUnit.FEET);
            Assert.AreEqual(5.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double result = Quantity<LengthUnit>.Convert(0.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(0.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_NegativeValue_PreservesSign()
        {
            double result = Quantity<LengthUnit>.Convert(-1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(-12.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 123.456;

            double toYard = Quantity<LengthUnit>.Convert(v, LengthUnit.FEET, LengthUnit.YARD);
            double backToFeet = Quantity<LengthUnit>.Convert(toYard, LengthUnit.YARD, LengthUnit.FEET);

            Assert.AreEqual(v, backToFeet, 1e-9);
        }

        [TestMethod]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<LengthUnit>.Convert(double.NaN, LengthUnit.FEET, LengthUnit.INCH)
            );
            
        }

        [TestMethod]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<LengthUnit>.Convert(double.PositiveInfinity, LengthUnit.FEET, LengthUnit.INCH)
            );
        }

        [TestMethod]
        public void testConversion_NegativeInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<LengthUnit>.Convert(double.NegativeInfinity, LengthUnit.FEET, LengthUnit.INCH)
            );
        }

        [TestMethod]
        public void testConversion_InvalidEnumValue_Throws()
        {
            LengthUnit bad = (LengthUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Quantity<LengthUnit>.Convert(1.0, bad, LengthUnit.INCH)
            );
        }
    }
}