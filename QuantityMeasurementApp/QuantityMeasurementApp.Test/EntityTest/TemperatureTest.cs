using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class TemperatureTest
    {
        private const double Eps = 1e-6;
        private IQuantityConversionService _conversionService;
        private IQuantityArithmeticService _arithmeticService;
        private QuantityEqualityComparer<TemperatureUnit> _equalityComparer;
        private QuantityValidationService _validator;
        private UnitAdapterFactory _adapterFactory;

        [TestInitialize]
        public void Setup()
        {
            _adapterFactory = new UnitAdapterFactory();
            _validator = new QuantityValidationService();
            _conversionService = new QuantityConversionService(_adapterFactory, _validator);
            _arithmeticService = new QuantityArithmeticService(_adapterFactory, _validator);
            _equalityComparer = new QuantityEqualityComparer<TemperatureUnit>(_adapterFactory, _validator);
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.FAHRENHEIT);
            var b = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = _equalityComparer.Equals(a, b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_SymmetricProperty()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result1 = _equalityComparer.Equals(a, b);
            bool result2 = _equalityComparer.Equals(b, a);

            // Then
            Assert.AreEqual(result1, result2, "Equality should be symmetric");
        }

        [TestMethod]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);

            // When
            bool result = _equalityComparer.Equals(a, a);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            double result = _conversionService.Convert(50.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(122.0, result, Eps);

            double result1 = _conversionService.Convert(-20.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(-4.0, result1, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            double result = _conversionService.Convert(122.0, TemperatureUnit.FAHRENHEIT, TemperatureUnit.CELSIUS);
            Assert.AreEqual(50.0, result, Eps);

            double result1 = _conversionService.Convert(-4.0, TemperatureUnit.FAHRENHEIT, TemperatureUnit.CELSIUS);
            Assert.AreEqual(-20.0, result1, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            // Arrange
            var tempC = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            double toF = _conversionService.Convert(tempC.Value, tempC.Unit, TemperatureUnit.FAHRENHEIT);
            double backToC = _conversionService.Convert(toF, TemperatureUnit.FAHRENHEIT, TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(tempC.Value, backToC, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_SameUnit()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(37.0, TemperatureUnit.CELSIUS);

            // Act
            double result = _conversionService.Convert(temp.Value, temp.Unit, TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(37.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_ZeroValue()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // Act
            double result = _conversionService.Convert(temp.Value, temp.Unit, TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(32.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_NegativeValues()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);

            // Act
            double result = _conversionService.Convert(temp.Value, temp.Unit, TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(-40.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_LargeValues()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.CELSIUS);

            // Act
            double result = _conversionService.Convert(temp.Value, temp.Unit, TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(1832.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var length = new Quantity<LengthUnit>(100.0, LengthUnit.FEET);

            // Act & Assert - This should throw or return false when comparing different types
            // Since we can't directly compare different generic types, we'll test that they're not equal
            Assert.IsFalse(temperature.Equals(length));
        }

        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var weight = new Quantity<WeightUnit>(50.0, WeightUnit.KILOGRAM);

            // Act & Assert
            Assert.IsFalse(temperature.Equals(weight));
        }

        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.CELSIUS);
            var volume = new Quantity<VolumeUnit>(25.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.IsFalse(temperature.Equals(volume));
        }

        [TestMethod]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            // Existing units should still behave correctly
            var length1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var length2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Use the actual AddUnit method from the service
            var sum = _arithmeticService.AddUnit(length1, length2);

            Assert.AreEqual(2.0, sum.Value, Eps);
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void testTemperatureUnit_NonLinearConversion()
        {
            // If conversion were linear-only by multiplication, 0C would not become 32F
            double converted = _conversionService.Convert(0.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(32.0, converted, Eps);
        }

        [TestMethod]
        public void testTemperatureUnit_AllConstants()
        {
            Assert.AreEqual("CELSIUS", TemperatureUnit.CELSIUS.ToString());
            Assert.AreEqual("FAHRENHEIT", TemperatureUnit.FAHRENHEIT.ToString());
        }

        [TestMethod]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            var temp = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(temp.Equals(null));
        }

        [TestMethod]
        public void testTemperatureDifferentValuesInequality()
        {
            var t1 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(_equalityComparer.Equals(t1, t2));
        }

        [TestMethod]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            var length1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var length2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            var lengthSum = _arithmeticService.AddUnit(length1, length2);

            Assert.AreEqual(2.0, lengthSum.Value, Eps);

            var weight1 = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var weight2 = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            var weightComparer = new QuantityEqualityComparer<WeightUnit>(_adapterFactory, _validator);
            Assert.IsTrue(weightComparer.Equals(weight1, weight2));

            double tempF = _conversionService.Convert(0.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(32.0, tempF, Eps);
        }
        
    }
}