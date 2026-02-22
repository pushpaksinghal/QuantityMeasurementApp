using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entity;
using static QuantityMeasurementApp.Core.Entity.Length;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthTest
    {
        [TestMethod]
        public void TestEquality_FeetToFeet_SameValue()
        {
            // Given
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(1.0, LengthUnit.FEET);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, FEET) should equal Quantity(1.0, FEET).");
        }

        [TestMethod]
        public void TestEquality_InchesToInches_SameValue()
        {
            var a = new Length(1.0, LengthUnit.INCHES);
            var b = new Length(1.0, LengthUnit.INCHES);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, INCHES) should equal Quantity(1.0, INCHES).");
        }

        [TestMethod]
        public void TestEquality_FeetToInches_EquivalentValue()
        {
            var feet = new Length(1.0, LengthUnit.FEET);
            var inches = new Length(12.0, LengthUnit.INCHES);

            Assert.IsTrue(feet.Equals(inches), "Quantity(1.0, FEET) should equal Quantity(12.0, INCHES).");
        }

        [TestMethod]
        public void TestEquality_InchesToFeet_EquivalentValue()
        {
            var inches = new Length(12.0, LengthUnit.INCHES);
            var feet = new Length(1.0, LengthUnit.FEET);

            Assert.IsTrue(inches.Equals(feet), "Quantity(12.0, INCHES) should equal Quantity(1.0, FEET) (symmetry).");
        }

        [TestMethod]
        public void TestEquality_FeetToFeet_DifferentValue()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(2.0, LengthUnit.FEET);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, FEET) should not equal Quantity(2.0, FEET).");
        }

        [TestMethod]
        public void TestEquality_InchesToInches_DifferentValue()
        {
            var a = new Length(1.0, LengthUnit.INCHES);
            var b = new Length(2.0, LengthUnit.INCHES);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, INCHES) should not equal Quantity(2.0, INCHES).");
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var a = new Length(1.0, LengthUnit.FEET);

            Assert.IsTrue(a.Equals(a), "Same reference must be equal (reflexive).");
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var a = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(a.Equals(null), "A Length object should not equal null.");
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            object other = "not a length";

            Assert.IsFalse(a.Equals(other), "Length should not equal an object of a different type.");
        }

        [TestMethod]
        public void TestUnitParsing_InvalidUnit_ShouldFail()
        {
            bool ok = Enum.TryParse("METER", ignoreCase: true, out LengthUnit _);

            Assert.IsFalse(ok, "Parsing an unsupported unit string should fail.");
        }

        [TestMethod]
        public void TestUnitParsing_ValidUnits_ShouldPass()
        {
            bool okFeet = Enum.TryParse("FEET", ignoreCase: true, out LengthUnit u1);
            bool okInch = Enum.TryParse("INCHES", ignoreCase: true, out LengthUnit u2);

            Assert.IsTrue(okFeet, "Parsing FEET should succeed.");
            Assert.AreEqual(LengthUnit.FEET, u1, "Parsed unit should be FEET.");

            Assert.IsTrue(okInch, "Parsing INCHES should succeed.");
            Assert.AreEqual(LengthUnit.INCHES, u2, "Parsed unit should be INCHES.");
        }
    }
}