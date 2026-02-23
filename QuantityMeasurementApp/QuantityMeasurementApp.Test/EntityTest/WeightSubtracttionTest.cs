using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightSubtractionTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testSubtraction_SameUnit_KgMinusKg()
        {
            var a = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_KgMinusGram_ImplicitTargetKg()
        {
            var kg = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(5000.0, WeightUnit.GRAM);

            var diff = kg.SubtractUnitTo(g);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Gram()
        {
            var kg = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(5000.0, WeightUnit.GRAM);

            var diff = kg.SubtractUnitTo(g, WeightUnit.GRAM);

            Assert.AreEqual(WeightUnit.GRAM, diff.Unit);
            Assert.AreEqual(5000.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var a = new Quantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(-3.0, diff.Value, Eps);
        }
    }
}