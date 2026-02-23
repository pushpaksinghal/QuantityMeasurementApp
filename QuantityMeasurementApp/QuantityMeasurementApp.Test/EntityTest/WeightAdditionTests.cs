using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightAdditionTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(2.0, WeightUnit.KILOGRAM);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_GramPlusGram()
        {
            var a = new Weight(500.0, WeightUnit.GRAM);
            var b = new Weight(500.0, WeightUnit.GRAM);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(1000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_PoundPlusPound()
        {
            var a = new Weight(1.0, WeightUnit.POUND);
            var b = new Weight(1.0, WeightUnit.POUND);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);
            var g = new Weight(1000.0, WeightUnit.GRAM);

            Weight sum = kg.AddUnitTO(g);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_GramPlusKilogram()
        {
            var g = new Weight(1000.0, WeightUnit.GRAM);
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Weight sum = g.AddUnitTO(kg);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            var lb = new Weight(2.2046226218, WeightUnit.POUND);
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Weight sum = lb.AddUnitTO(kg);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(4.4092452436, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var kg = new Weight(5.0, WeightUnit.KILOGRAM);
            var zeroG = new Weight(0.0, WeightUnit.GRAM);

            Weight sum = kg.AddUnitTO(zeroG);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var a = new Weight(5.0, WeightUnit.KILOGRAM);
            var b = new Weight(-2000.0, WeightUnit.GRAM);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InKilogramsBase()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);

            double sum1InKg = Weight.Convert(a.AddUnitTO(b).Value, a.AddUnitTO(b).Unit, WeightUnit.KILOGRAM);
            double sum2InKg = Weight.Convert(b.AddUnitTO(a).Value, b.AddUnitTO(a).Unit, WeightUnit.KILOGRAM);

            Assert.AreEqual(sum1InKg, sum2InKg, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.Throws<ArgumentException>(() => a.AddUnitTO(null));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            var a = new Weight(1e6, WeightUnit.KILOGRAM);
            var b = new Weight(1e6, WeightUnit.KILOGRAM);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(2e6, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var a = new Weight(0.001, WeightUnit.KILOGRAM);
            var b = new Weight(0.002, WeightUnit.KILOGRAM);

            Weight sum = a.AddUnitTO(b);

            Assert.AreEqual(0.003, sum.Value, 1e-12);
        }
    }
}