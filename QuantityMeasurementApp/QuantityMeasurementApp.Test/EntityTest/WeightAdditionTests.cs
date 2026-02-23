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
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_GramPlusGram()
        {
            var a = new Quantity<WeightUnit>(500.0, WeightUnit.GRAM);
            var b = new Quantity<WeightUnit>(500.0, WeightUnit.GRAM);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(1000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_PoundPlusPound()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.POUND);
            var b = new Quantity<WeightUnit>(1.0, WeightUnit.POUND);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            Quantity<WeightUnit> sum = kg.AddUnitTO(g);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_GramPlusKilogram()
        {
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            Quantity<WeightUnit> sum = g.AddUnitTO(kg);

            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            var lb = new Quantity<WeightUnit>(2.2046226218, WeightUnit.POUND);
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            Quantity<WeightUnit> sum = lb.AddUnitTO(kg);

            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
            Assert.AreEqual(4.4092452436, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var kg = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);
            var zeroG = new Quantity<WeightUnit>(0.0, WeightUnit.GRAM);

            Quantity<WeightUnit> sum = kg.AddUnitTO(zeroG);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var a = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(-2000.0, WeightUnit.GRAM);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InKilogramsBase()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            double sum1InKg = Quantity<WeightUnit>.Convert(a.AddUnitTO(b).Value, a.AddUnitTO(b).Unit, WeightUnit.KILOGRAM);
            double sum2InKg = Quantity<WeightUnit>.Convert(b.AddUnitTO(a).Value, b.AddUnitTO(a).Unit, WeightUnit.KILOGRAM);

            Assert.AreEqual(sum1InKg, sum2InKg, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            Assert.Throws<ArgumentException>(() => a.AddUnitTO(null));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            var a = new Quantity<WeightUnit>(1e6, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(1e6, WeightUnit.KILOGRAM);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(2e6, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var a = new Quantity<WeightUnit>(0.001, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(0.002, WeightUnit.KILOGRAM);

            Quantity<WeightUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(0.003, sum.Value, 1e-12);
        }
    }
}