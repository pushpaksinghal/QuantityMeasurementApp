using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeSubtractionTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(7.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_LitreMinusMillilitre()
        {
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(4.5, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            var diff = a.SubtractUnitTo(b, VolumeUnit.MILLILITRE);

            Assert.AreEqual(VolumeUnit.MILLILITRE, diff.Unit);
            Assert.AreEqual(3000.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(0.0, diff.Value, Eps);
        }
    }
}