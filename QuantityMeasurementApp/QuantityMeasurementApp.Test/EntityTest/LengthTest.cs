using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthTest
    {
        private const double Eps = 1e-6;
        private IQuantityConversionService _conversionService;
        private IQuantityArithmeticService _arithmeticService;
        private QuantityEqualityComparer<LengthUnit> _equalityComparer;
        private QuantityValidationService _validator;
        private UnitAdapterFactory _adapterFactory;

        [TestInitialize]
        public void Setup()
        {
            _adapterFactory = new UnitAdapterFactory();
            _validator = new QuantityValidationService();
            _conversionService = new QuantityConversionService(_adapterFactory, _validator);
            _arithmeticService = new QuantityArithmeticService(_adapterFactory, _validator);
            _equalityComparer = new QuantityEqualityComparer<LengthUnit>(_adapterFactory, _validator);
        }

        [TestMethod]
        public void TestEquality_FeetToFeet_SameValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, FEET) should equal Quantity(1.0, FEET).");
        }

        [TestMethod]
        public void TestEquality_InchesToInches_SameValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, INCH) should equal Quantity(1.0, INCH).");
        }

        [TestMethod]
        public void TestEquality_YardToyard_SameValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, YARD) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void TestEquality_CentimetersToCentimeters_SameValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, Centimeters) should equal Quantity(1.0, Centimeters).");
        }

        [TestMethod]
        public void TestEquality_FeetToInches_EquivalentValue()
        {
            // Given
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // When
            bool result = _equalityComparer.Equals(feet, inches);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, FEET) should equal Quantity(12.0, INCHES).");
        }

        [TestMethod]
        public void TestEquality_InchesToFeet_EquivalentValue()
        {
            // Given
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(inches, feet);

            // Then
            Assert.IsTrue(result, "Quantity(12.0, INCHES) should equal Quantity(1.0, FEET) (symmetry).");
        }

        [TestMethod]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            // Given
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feet = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(yard, feet);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, YARD) should equal Quantity(3.0, FEET).");
        }

        [TestMethod]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            // Given
            var feet = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            // When
            bool result = _equalityComparer.Equals(feet, yard);

            // Then
            Assert.IsTrue(result, "Quantity(3.0, FEET) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void testEquality_YardToInches_EquivalentValue()
        {
            // Given
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var inches = new Quantity<LengthUnit>(36.0, LengthUnit.INCH);

            // When
            bool result = _equalityComparer.Equals(yard, inches);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, YARD) should equal Quantity(36.0, INCH).");
        }

        [TestMethod]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            // Given
            var inches = new Quantity<LengthUnit>(36.0, LengthUnit.INCH);
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            // When
            bool result = _equalityComparer.Equals(inches, yard);

            // Then
            Assert.IsTrue(result, "Quantity(36.0, INCH) should equal Quantity(1.0, YARD).");
        }

        [TestMethod]
        public void testEquality_CentimetersToInches_EquivalentValue()
        {
            // Given: 1 cm = 0.393701 inches
            var cm = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);
            var inches = new Quantity<LengthUnit>(0.393701, LengthUnit.INCH);

            // When
            bool result = _equalityComparer.Equals(cm, inches);

            // Then
            Assert.IsTrue(result, "Quantity(1.0, CENTIMETERS) should equal Quantity(0.393701, INCH).");
        }

        [TestMethod]
        public void testEquality_InchesToCentimeters_EquivalentValue()
        {
            // Given
            var inches = new Quantity<LengthUnit>(0.393701, LengthUnit.INCH);
            var cm = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);

            // When
            bool result = _equalityComparer.Equals(inches, cm);

            // Then
            Assert.IsTrue(result, "Quantity(0.393701, INCH) should equal Quantity(1.0, CENTIMETERS).");
        }

        [TestMethod]
        public void testEquality_CentimetersToFeet_NonEquivalentValue()
        {
            // Given
            var cm = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(cm, feet);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, CENTIMETERS) should not equal Quantity(1.0, FEET).");
        }

        [TestMethod]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            // Given
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feet = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(yard, feet);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, YARDS) should not equal Quantity(2.0, FEET).");
        }

        [TestMethod]
        public void TestEquality_FeetToFeet_DifferentValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, FEET) should not equal Quantity(2.0, FEET).");
        }

        [TestMethod]
        public void TestEquality_InchesToInches_DifferentValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.INCH);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, INCHES) should not equal Quantity(2.0, INCHES).");
        }

        [TestMethod]
        public void testEquality_YardToYard_DifferentValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.YARD);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, YARD) should not equal Quantity(2.0, YARD).");
        }

        [TestMethod]
        public void testEquality_CentimetersToCentimeters_DifferentValue()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.CENTIMETERS);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsFalse(result, "Quantity(1.0, CENTIMETERS) should not equal Quantity(2.0, CENTIMETERS).");
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(a, a);

            // Then
            Assert.IsTrue(result, "Same reference must be equal (reflexive).");
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(a, null);

            // Then
            Assert.IsFalse(result, "A Length object should not equal null.");
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            object other = "not a length";

            // When - Using object.Equals for different types
            bool result = a.Equals(other);

            // Then
            Assert.IsFalse(result, "Length should not equal an object of a different type.");
        }

        [TestMethod]
        public void TestUnitParsing_InvalidUnit_ShouldFail()
        {
            // When
            bool ok = Enum.TryParse("METER", ignoreCase: true, out LengthUnit _);

            // Then
            Assert.IsFalse(ok, "Parsing an unsupported unit string should fail.");
        }

        [TestMethod]
        public void TestUnitParsing_ValidUnits_ShouldPass()
        {
            // When
            bool okFeet = Enum.TryParse("FEET", ignoreCase: true, out LengthUnit u1);
            bool okInch = Enum.TryParse("INCH", ignoreCase: true, out LengthUnit u2);

            // Then
            Assert.IsTrue(okFeet, "Parsing FEET should succeed.");
            Assert.AreEqual(LengthUnit.FEET, u1, "Parsed unit should be FEET.");

            Assert.IsTrue(okInch, "Parsing INCH should succeed.");
            Assert.AreEqual(LengthUnit.INCH, u2, "Parsed unit should be INCH.");
        }

        [TestMethod]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);     // 36 inches
            var b = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);      // 36 inches
            var c = new Quantity<LengthUnit>(36.0, LengthUnit.INCH);   // 36 inches

            // When
            bool aEqualsB = _equalityComparer.Equals(a, b);
            bool bEqualsC = _equalityComparer.Equals(b, c);
            bool aEqualsC = _equalityComparer.Equals(a, c);

            // Then
            Assert.IsTrue(aEqualsB, "A should equal B for transitive setup.");
            Assert.IsTrue(bEqualsC, "B should equal C for transitive setup.");
            Assert.IsTrue(aEqualsC, "Transitive property violated: if A=B and B=C then A must equal C.");
        }

        [TestMethod]
        public void testEquality_AllUnits_ComplexScenario()
        {
            // Given
            var yard2 = new Quantity<LengthUnit>(2.0, LengthUnit.YARD);     // 72 inches
            var feet6 = new Quantity<LengthUnit>(6.0, LengthUnit.FEET);      // 72 inches
            var inch72 = new Quantity<LengthUnit>(72.0, LengthUnit.INCH);  // 72 inches

            // When
            bool yardEqualsFeet = _equalityComparer.Equals(yard2, feet6);
            bool feetEqualsInch = _equalityComparer.Equals(feet6, inch72);
            bool yardEqualsInch = _equalityComparer.Equals(yard2, inch72);

            // Then
            Assert.IsTrue(yardEqualsFeet, "Quantity(2.0, YARDS) should equal Quantity(6.0, FEET).");
            Assert.IsTrue(feetEqualsInch, "Quantity(6.0, FEET) should equal Quantity(72.0, INCHES).");
            Assert.IsTrue(yardEqualsInch, "Quantity(2.0, YARDS) should equal Quantity(72.0, INCHES).");
        }

        [TestMethod]
        public void testEquality_SameReference_Yards()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            // When
            bool result = _equalityComparer.Equals(a, a);

            // Then
            Assert.IsTrue(result, "Same reference must be equal (reflexive).");
        }

        [TestMethod]
        public void testEquality_NullComparison_Centimeters()
        {
            // Given
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.CENTIMETERS);

            // When
            bool result = _equalityComparer.Equals(a, null);

            // Then
            Assert.IsFalse(result, "Length should not equal null.");
        }

        

        [TestMethod]
        public void testEquality_WithValuesJustBeyondEpsilon()
        {
            // Given - Values just beyond epsilon tolerance
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(1.0001, LengthUnit.FEET);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then - Should not be considered equal
            Assert.IsFalse(result, "Values beyond epsilon should not be equal.");
        }
    }
}