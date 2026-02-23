using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightAdditionTargetUnitTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.KILOGRAM);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gram()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.GRAM);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Pound()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.POUND);

            // 2 kg in pounds ~ 4.4092452436
            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(4.4092452436, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var a = new Weight(2.0, WeightUnit.POUND);
            var b = new Weight(453.592, WeightUnit.GRAM);

            // 2 lb + 1 lb = 3 lb (approximately)
            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.POUND);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, 1e-3);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var a = new Weight(2.0, WeightUnit.KILOGRAM);
            var b = new Weight(4.0, WeightUnit.POUND);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.KILOGRAM);

            // 4 lb = 1.814368 kg, total = 3.814368 kg
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(3.81436948, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);

            Weight sum1 = Weight.AddToSpecificUnit(a, b, WeightUnit.POUND);
            Weight sum2 = Weight.AddToSpecificUnit(b, a, WeightUnit.POUND);

            Assert.AreEqual(WeightUnit.POUND, sum1.Unit);
            Assert.AreEqual(WeightUnit.POUND, sum2.Unit);
            Assert.AreEqual(sum1.Value, sum2.Value, 1e-9);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            var a = new Weight(5.0, WeightUnit.KILOGRAM);
            var b = new Weight(0.0, WeightUnit.GRAM);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.POUND);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(11.023113109, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            var a = new Weight(5.0, WeightUnit.KILOGRAM);
            var b = new Weight(-2000.0, WeightUnit.GRAM);

            Weight sum = Weight.AddToSpecificUnit(a, b, WeightUnit.GRAM);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(3000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullSecondOperand_Throws()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.Throws<ArgumentException>(() =>
                Weight.AddToSpecificUnit(a, null, WeightUnit.KILOGRAM)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1.0, WeightUnit.KILOGRAM);

            WeightUnit badTarget = (WeightUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Weight.AddToSpecificUnit(a, b, badTarget)
            );
        }
    }
}