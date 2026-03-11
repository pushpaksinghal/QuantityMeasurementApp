using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightTest
    {
        [TestMethod]
        public void TestEquality_KilogramToGram_EquivalentValue()
        {
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            Assert.IsTrue(kg.Equals(g));
        }

        [TestMethod]
        public void TestEquality_PoundToKilogram_EquivalentValue()
        {
            // accurate pound value for 1 kg
            var lb = new Quantity<WeightUnit>(2.2046226218, WeightUnit.POUND);
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(lb.Equals(kg));
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            object other = "not weight";

            Assert.IsFalse(a.Equals(other));
        }
    }
}