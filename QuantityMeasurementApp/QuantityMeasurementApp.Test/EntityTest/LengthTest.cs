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
            var a = new Length(1.0, LengthUnit.INCH);
            var b = new Length(1.0, LengthUnit.INCH);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, INCH) should equal Quantity(1.0, INCH).");
        }

        [TestMethod]
        public void TestEquality_YardToyard_SameValue()
        {
            var a = new Length(1.0, LengthUnit.YARD);
            var b = new Length(1.0, LengthUnit.YARD);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, YARD) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void TestEquality_CentimetersToCentimeters_SameValue()
        {
            var a = new Length(1.0, LengthUnit.CENTIMETERS);
            var b = new Length(1.0, LengthUnit.CENTIMETERS);

            Assert.IsTrue(a.Equals(b), "Quantity(1.0, Centimeters) should equal Quantity(1.0, Centimeters).");
        }
        //------------------------------------------------------------------------------------------//
        [TestMethod]
        public void TestEquality_FeetToInches_EquivalentValue()
        {
            var feet = new Length(1.0, LengthUnit.FEET);
            var inches = new Length(12.0, LengthUnit.INCH);

            Assert.IsTrue(feet.Equals(inches), "Quantity(1.0, FEET) should equal Quantity(12.0, INCHES).");
        }

        [TestMethod]
        public void TestEquality_InchesToFeet_EquivalentValue()
        {
            var inches = new Length(12.0, LengthUnit.INCH);
            var feet = new Length(1.0, LengthUnit.FEET);

            Assert.IsTrue(inches.Equals(feet), "Quantity(12.0, INCHES) should equal Quantity(1.0, FEET) (symmetry).");
        }
        [TestMethod]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            var yard = new Length(1.0, LengthUnit.YARD);
            var feet = new Length(3.0, LengthUnit.FEET);

            Assert.IsTrue(yard.Equals(feet), "Quantity(1.0, YARD) should equal Quantity(3.0, FEET).");
        }

        [TestMethod]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            var feet = new Length(3.0, LengthUnit.FEET);
            var yard = new Length(1.0, LengthUnit.YARD);

            Assert.IsTrue(feet.Equals(yard), "Quantity(3.0, FEET) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void testEquality_YardToInches_EquivalentValue()
        {
            var yard = new Length(1.0, LengthUnit.YARD);
            var inches = new Length(36.0, LengthUnit.INCH);

            Assert.IsTrue(yard.Equals(inches), "Quantity(1.0, YARD) should equal Quantity(36.0, INCH).");
        }

        [TestMethod]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            var inches = new Length(36.0, LengthUnit.INCH);
            var yard = new Length(1.0, LengthUnit.YARD);

            Assert.IsTrue(inches.Equals(yard), "Quantity(36.0, INCH) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void testEquality_CentimetersToInches_EquivalentValue()
        {
            // Given: 1 cm = 0.393701 inches (as per UC4)
            var cm = new Length(1.0, LengthUnit.CENTIMETERS);
            var inches = new Length(0.393701, LengthUnit.INCH);

            Assert.IsTrue(cm.Equals(inches), "Quantity(1.0, CENTIMETERS) should equal Quantity(0.393701, INCH).");
        }

        [TestMethod]
        public void testEquality_InchesToCentimeters_EquivalentValue()
        {
            var inches = new Length(0.393701, LengthUnit.INCH);
            var cm = new Length(1.0, LengthUnit.CENTIMETERS);

            Assert.IsTrue(inches.Equals(cm), "Quantity(0.393701, INCH) should equal Quantity(1.0, CENTIMETERS).");
        }
        //-------------------------------------------------------------------------------------------------------//
        [TestMethod]
        public void testEquality_CentimetersToFeet_NonEquivalentValue()
        {
            var cm = new Length(1.0, LengthUnit.CENTIMETERS);
            var feet = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(cm.Equals(feet), "Quantity(1.0, CENTIMETERS) should not equal Quantity(1.0, FEET).");
        }

        [TestMethod]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            var yard = new Length(1.0, LengthUnit.YARD);
            var feet = new Length(2.0, LengthUnit.FEET);

            Assert.IsFalse(yard.Equals(feet), "Quantity(1.0, YARDS) should not equal Quantity(2.0, FEET).");
        }

        //----------------------------------------------------------------------------------------------------//
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
            var a = new Length(1.0, LengthUnit.INCH);
            var b = new Length(2.0, LengthUnit.INCH);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, INCHES) should not equal Quantity(2.0, INCHES).");
        }
        [TestMethod]
        public void testEquality_YardToYard_DifferentValue()
        {
            var a = new Length(1.0, LengthUnit.YARD);
            var b = new Length(2.0, LengthUnit.YARD);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, YARD) should not equal Quantity(2.0, YARD).");
        }
        [TestMethod]
        public void testEquality_CentimetersToCentimeters_DifferentValue()
        {
            var a = new Length(1.0, LengthUnit.CENTIMETERS);
            var b = new Length(2.0, LengthUnit.CENTIMETERS);

            Assert.IsFalse(a.Equals(b), "Quantity(1.0, CENTIMETERS) should not equal Quantity(2.0, CENTIMETERS).");
        }
        //---------------------------------------------------------------------------------------------//

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
            bool okInch = Enum.TryParse("INCH", ignoreCase: true, out LengthUnit u2);

            Assert.IsTrue(okFeet, "Parsing FEET should succeed.");
            Assert.AreEqual(LengthUnit.FEET, u1, "Parsed unit should be FEET.");

            Assert.IsTrue(okInch, "Parsing INCH should succeed.");
            Assert.AreEqual(LengthUnit.INCH, u2, "Parsed unit should be INCH.");
        }

        [TestMethod]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            var a = new Length(1.0, LengthUnit.YARD);     // 36 inches
            var b = new Length(3.0, LengthUnit.FEET);      // 36 inches
            var c = new Length(36.0, LengthUnit.INCH);   // 36 inches

            Assert.IsTrue(a.Equals(b), "A should equal B for transitive setup.");
            Assert.IsTrue(b.Equals(c), "B should equal C for transitive setup.");
            Assert.IsTrue(a.Equals(c), "Transitive property violated: if A=B and B=C then A must equal C.");
        }

        [TestMethod]
        public void testEquality_AllUnits_ComplexScenario()
        {
            var yard2 = new Length(2.0, LengthUnit.YARD);     // 72 inches
            var feet6 = new Length(6.0, LengthUnit.FEET);      // 72 inches
            var inch72 = new Length(72.0, LengthUnit.INCH);  // 72 inches

            Assert.IsTrue(yard2.Equals(feet6), "Quantity(2.0, YARDS) should equal Quantity(6.0, FEET).");
            Assert.IsTrue(feet6.Equals(inch72), "Quantity(6.0, FEET) should equal Quantity(72.0, INCHES).");
            Assert.IsTrue(yard2.Equals(inch72), "Quantity(2.0, YARDS) should equal Quantity(72.0, INCHES).");
        }

        [TestMethod]
        public void testEquality_SameReference_Yards()
        {
            var a = new Length(1.0, LengthUnit.YARD);

            Assert.IsTrue(a.Equals(a), "Same reference must be equal (reflexive).");
        }

        [TestMethod]
        public void testEquality_NullComparison_Centimeters()
        {
            var a = new Length(1.0, LengthUnit.CENTIMETERS);

            Assert.IsFalse(a.Equals(null), "Length should not equal null.");
        }
    }
}