using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeAdditionTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            var sum = a.AddUnitTO(b);

            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var a = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            var sum = a.AddUnitTO(b);

            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(1000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var sum = litre.AddUnitTO(ml);

            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            var sum = ml.AddUnitTO(litre);

            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            var gal = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var litre = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            var sum = gal.AddUnitTO(litre);

            Assert.AreEqual(VolumeUnit.GALLON, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var litre = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var zeroMl = new Quantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);

            var sum = litre.AddUnitTO(zeroMl);

            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(-2000.0, VolumeUnit.MILLILITRE);

            var sum = a.AddUnitTO(b);

            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InBaseLitre()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var sum1 = a.AddUnitTO(b);
            var sum2 = b.AddUnitTO(a);

            // Convert both to litres and compare
            double s1L = Quantity<VolumeUnit>.Convert(sum1.Value, sum1.Unit, VolumeUnit.LITRE);
            double s2L = Quantity<VolumeUnit>.Convert(sum2.Value, sum2.Unit, VolumeUnit.LITRE);

            Assert.AreEqual(s1L, s2L, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            Assert.Throws<ArgumentException>(() => a.AddUnitTO(null));
        }
    }
}