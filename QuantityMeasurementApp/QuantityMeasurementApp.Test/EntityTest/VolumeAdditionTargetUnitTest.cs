using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeAdditionTargetUnitTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var sum = Quantity<VolumeUnit>.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var sum = Quantity<VolumeUnit>.AddToSpecificUnit(a, b, VolumeUnit.MILLILITRE);

            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            var a = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            var sum = Quantity<VolumeUnit>.AddToSpecificUnit(a, b, VolumeUnit.GALLON);

            Assert.AreEqual(VolumeUnit.GALLON, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var sum1 = Quantity<VolumeUnit>.AddToSpecificUnit(a, b, VolumeUnit.GALLON);
            var sum2 = Quantity<VolumeUnit>.AddToSpecificUnit(b, a, VolumeUnit.GALLON);

            Assert.AreEqual(VolumeUnit.GALLON, sum1.Unit);
            Assert.AreEqual(VolumeUnit.GALLON, sum2.Unit);
            Assert.AreEqual(sum1.Value, sum2.Value, 1e-9);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullSecondOperand_Throws()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            Assert.Throws<ArgumentException>(() =>
                Quantity<VolumeUnit>.AddToSpecificUnit(a, null, VolumeUnit.LITRE)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            VolumeUnit badTarget = (VolumeUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Quantity<VolumeUnit>.AddToSpecificUnit(a, b, badTarget)
            );
        }
    }
}