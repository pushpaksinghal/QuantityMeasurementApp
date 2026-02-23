using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightAndVolumeDIvisionTests
    {
        private const double Eps = 1e-9;

        [TestMethod]
        public void testDivision_Weight_CrossUnit_GramDividedByKilogram()
        {
            var a = new Quantity<WeightUnit>(2000.0, WeightUnit.GRAM);
            var b = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(2.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_Volume_CrossUnit_MillilitreDividedByLitre()
        {
            var a = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(1.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_Volume_RatioGreaterThanOne()
        {
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(2.0, ratio, Eps);
        }
    }
}