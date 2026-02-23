using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeConversionTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testConversion_LitreToMillilitre()
        {
            double result = Quantity<VolumeUnit>.Convert(1.0, VolumeUnit.LITRE, VolumeUnit.MILLILITRE);
            Assert.AreEqual(1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_MillilitreToLitre()
        {
            double result = Quantity<VolumeUnit>.Convert(1000.0, VolumeUnit.MILLILITRE, VolumeUnit.LITRE);
            Assert.AreEqual(1.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_GallonToLitre()
        {
            double result = Quantity<VolumeUnit>.Convert(1.0, VolumeUnit.GALLON, VolumeUnit.LITRE);
            Assert.AreEqual(3.78541, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_LitreToGallon()
        {
            double result = Quantity<VolumeUnit>.Convert(1.0, VolumeUnit.LITRE, VolumeUnit.GALLON);
            Assert.AreEqual(0.264172, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_MillilitreToGallon()
        {
            double result = Quantity<VolumeUnit>.Convert(500.0, VolumeUnit.MILLILITRE, VolumeUnit.GALLON);
            // 0.5 L = 0.5 / 3.78541 gal
            Assert.AreEqual(0.132086, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = Quantity<VolumeUnit>.Convert(5.0, VolumeUnit.LITRE, VolumeUnit.LITRE);
            Assert.AreEqual(5.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double result = Quantity<VolumeUnit>.Convert(0.0, VolumeUnit.LITRE, VolumeUnit.MILLILITRE);
            Assert.AreEqual(0.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_NegativeValue_PreservesSign()
        {
            double result = Quantity<VolumeUnit>.Convert(-1.0, VolumeUnit.LITRE, VolumeUnit.MILLILITRE);
            Assert.AreEqual(-1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 123.456;

            double toGal = Quantity<VolumeUnit>.Convert(v, VolumeUnit.LITRE, VolumeUnit.GALLON);
            double backToLitre = Quantity<VolumeUnit>.Convert(toGal, VolumeUnit.GALLON, VolumeUnit.LITRE);

            Assert.AreEqual(v, backToLitre, 1e-6);
        }

        [TestMethod]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<VolumeUnit>.Convert(double.NaN, VolumeUnit.LITRE, VolumeUnit.MILLILITRE)
            );
        }

        [TestMethod]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                Quantity<VolumeUnit>.Convert(double.PositiveInfinity, VolumeUnit.LITRE, VolumeUnit.MILLILITRE)
            );
        }

        [TestMethod]
        public void testConversion_InvalidEnumValue_Throws()
        {
            VolumeUnit bad = (VolumeUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Quantity<VolumeUnit>.Convert(1.0, bad, VolumeUnit.LITRE)
            );
        }
    }
}
