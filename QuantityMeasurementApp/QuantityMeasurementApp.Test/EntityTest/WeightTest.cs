using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightTest
    {
        private const double Eps = 1e-6;

        // ----------------------- Same-unit equality ----------------------- //

        [TestMethod]
        public void TestEquality_KilogramToKilogram_SameValue()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, KILOGRAM) should equal Quantity(1.0, KILOGRAM).");
        }

        [TestMethod]
        public void TestEquality_GramToGram_SameValue()
        {
            var a = new Weight(1.0, WeightUnit.GRAM);
            var b = new Weight(1.0, WeightUnit.GRAM);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, GRAM) should equal Quantity(1.0, GRAM).");
        }

        [TestMethod]
        public void TestEquality_PoundToPound_SameValue()
        {
            var a = new Weight(2.0, WeightUnit.POUND);
            var b = new Weight(2.0, WeightUnit.POUND);

            Assert.IsTrue(a.Equals(b), "Quantity(2.0, POUND) should equal Quantity(2.0, POUND).");
        }

        // ----------------------- Cross-unit equality ----------------------- //

        [TestMethod]
        public void TestEquality_KilogramToGram_EquivalentValue()
        {
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);
            var g = new Weight(1000.0, WeightUnit.GRAM);

            Assert.IsTrue(kg.Equals(g), "Quantity(1.0, KILOGRAM) should equal Quantity(1000.0, GRAM).");
        }

        [TestMethod]
        public void TestEquality_GramToKilogram_EquivalentValue()
        {
            var g = new Weight(1000.0, WeightUnit.GRAM);
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(g.Equals(kg), "Quantity(1000.0, GRAM) should equal Quantity(1.0, KILOGRAM) (symmetry).");
        }

        [TestMethod]
        public void TestEquality_PoundToKilogram_EquivalentValue()
        {
            var lb = new Weight(2.2046226218, WeightUnit.POUND);
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(lb.Equals(kg), "Quantity(~2.20462, POUND) should equal Quantity(1.0, KILOGRAM) (within precision).");
        }

        [TestMethod]
        public void TestEquality_GramToPound_EquivalentValue()
        {
            var g = new Weight(453.592, WeightUnit.GRAM);
            var lb = new Weight(1.0, WeightUnit.POUND);

            Assert.IsTrue(g.Equals(lb), "Quantity(~453.592, GRAM) should equal Quantity(1.0, POUND) (within precision).");
        }

        // ----------------------- Non-equality ----------------------- //

        [TestMethod]
        public void TestEquality_KilogramToKilogram_DifferentValue()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(2.0, WeightUnit.KILOGRAM);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, KILOGRAM) should not equal Quantity(2.0, KILOGRAM).");
        }

        [TestMethod]
        public void TestEquality_GramToGram_DifferentValue()
        {
            var a = new Weight(1.0, WeightUnit.GRAM);
            var b = new Weight(2.0, WeightUnit.GRAM);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, GRAM) should not equal Quantity(2.0, GRAM).");
        }

        [TestMethod]
        public void TestEquality_PoundToPound_DifferentValue()
        {
            var a = new Weight(1.0, WeightUnit.POUND);
            var b = new Weight(2.0, WeightUnit.POUND);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, POUND) should not equal Quantity(2.0, POUND).");
        }

        [TestMethod]
        public void TestEquality_KilogramToGram_NonEquivalentValue()
        {
            var kg = new Weight(1.0, WeightUnit.KILOGRAM);
            var g = new Weight(900.0, WeightUnit.GRAM);

            Assert.IsFalse(kg.Equals(g), "Quantity(1.0, KILOGRAM) should not equal Quantity(900.0, GRAM).");
        }

        // ----------------------- Object contract checks ----------------------- //

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsTrue(a.Equals(a), "Same reference must be equal (reflexive).");
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsFalse(a.Equals(null), "A Weight object should not equal null.");
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            object other = "not a weight";

            Assert.IsFalse(a.Equals(other), "Weight should not equal an object of a different type.");
        }

        [TestMethod]
        public void TestUnitParsing_InvalidUnit_ShouldFail()
        {
            bool ok = Enum.TryParse("TON", ignoreCase: true, out WeightUnit _);
            Assert.IsFalse(ok, "Parsing an unsupported unit string should fail.");
        }

        [TestMethod]
        public void TestUnitParsing_ValidUnits_ShouldPass()
        {
            bool okKg = Enum.TryParse("KILOGRAM", ignoreCase: true, out WeightUnit u1);
            bool okG = Enum.TryParse("GRAM", ignoreCase: true, out WeightUnit u2);
            bool okLb = Enum.TryParse("POUND", ignoreCase: true, out WeightUnit u3);

            Assert.IsTrue(okKg);
            Assert.AreEqual(WeightUnit.KILOGRAM, u1);

            Assert.IsTrue(okG);
            Assert.AreEqual(WeightUnit.GRAM, u2);

            Assert.IsTrue(okLb);
            Assert.AreEqual(WeightUnit.POUND, u3);
        }

        [TestMethod]
        public void TestEquality_TransitiveProperty()
        {
            var a = new Weight(1.0, WeightUnit.KILOGRAM);
            var b = new Weight(1000.0, WeightUnit.GRAM);
            var c = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b), "A should equal B for transitive setup.");
            Assert.IsTrue(b.Equals(c), "B should equal C for transitive setup.");
            Assert.IsTrue(a.Equals(c), "Transitive property violated.");
        }
    }
}