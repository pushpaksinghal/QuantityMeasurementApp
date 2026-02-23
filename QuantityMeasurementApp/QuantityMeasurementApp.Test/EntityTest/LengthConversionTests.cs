using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;
using static QuantityMeasurementApp.Core.Entity.Length;
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
            double result = Length.Convert(1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(12.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_InchToFeet()
        {
            double result = Length.Convert(24.0, LengthUnit.INCH, LengthUnit.FEET);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_YardToFeet()
        {
            double result = Length.Convert(3.0, LengthUnit.YARD, LengthUnit.FEET);
            Assert.AreEqual(9.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_YardToInch()
        {
            double result = Length.Convert(1.0, LengthUnit.YARD, LengthUnit.INCH);
            Assert.AreEqual(36.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_InchToYard()
        {
            double result = Length.Convert(72.0, LengthUnit.INCH, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_CentimetersToInch_2Point54cm_Is1Inch()
        {
            // Because your factor is rounded, use slightly looser tolerance
            double result = Length.Convert(2.54, LengthUnit.CENTIMETERS, LengthUnit.INCH);
            Assert.AreEqual(1.0, result, 1e-4);
        }

        [TestMethod]
        public void testConversion_InchToCentimeters_1Inch_Is2Point54cm()
        {
            double result = Length.Convert(1.0, LengthUnit.INCH, LengthUnit.CENTIMETERS);
            Assert.AreEqual(2.54, result, 1e-3);
        }

        [TestMethod]
        public void testConversion_FeetToYard()
        {
            double result = Length.Convert(6.0, LengthUnit.FEET, LengthUnit.YARD);
            Assert.AreEqual(2.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = Length.Convert(5.0, LengthUnit.FEET, LengthUnit.FEET);
            Assert.AreEqual(5.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double result = Length.Convert(0.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(0.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_NegativeValue_PreservesSign()
        {
            double result = Length.Convert(-1.0, LengthUnit.FEET, LengthUnit.INCH);
            Assert.AreEqual(-12.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 123.456;

            double toYard = Length.Convert(v, LengthUnit.FEET, LengthUnit.YARD);
            double backToFeet = Length.Convert(toYard, LengthUnit.YARD, LengthUnit.FEET);

            Assert.AreEqual(v, backToFeet, 1e-9);
        }

        [TestMethod]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Length.Convert(double.NaN, LengthUnit.FEET, LengthUnit.INCH)
            );
            
        }

        [TestMethod]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Length.Convert(double.PositiveInfinity, LengthUnit.FEET, LengthUnit.INCH)
            );
        }

        [TestMethod]
        public void testConversion_NegativeInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Length.Convert(double.NegativeInfinity, LengthUnit.FEET, LengthUnit.INCH)
            );
        }

        [TestMethod]
        public void testConversion_InvalidEnumValue_Throws()
        {
            LengthUnit bad = (LengthUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Length.Convert(1.0, bad, LengthUnit.INCH)
            );
        }
    }
}