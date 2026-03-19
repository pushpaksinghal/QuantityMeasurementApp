using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightTest
    {
        private const double Eps = 1e-6;
        private IQuantityConversionService _conversionService;
        private IQuantityArithmeticService _arithmeticService;
        private QuantityEqualityComparer<WeightUnit> _equalityComparer;
        private QuantityValidationService _validator;
        private UnitAdapterFactory _adapterFactory;

        [TestInitialize]
        public void Setup()
        {
            _adapterFactory = new UnitAdapterFactory();
            _validator = new QuantityValidationService();
            _conversionService = new QuantityConversionService(_adapterFactory, _validator);
            _arithmeticService = new QuantityArithmeticService(_adapterFactory, _validator);
            _equalityComparer = new QuantityEqualityComparer<WeightUnit>(_adapterFactory, _validator);
        }

        [TestMethod]
        public void TestEquality_KilogramToGram_EquivalentValue()
        {
            // Arrange
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            bool result = _equalityComparer.Equals(kg, g);

            // Assert
            Assert.IsTrue(result, "1 Kilogram should equal 1000 Grams");
        }

        [TestMethod]
        public void TestEquality_KilogramToGram_EquivalentValue_Symmetric()
        {
            // Arrange
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            bool result1 = _equalityComparer.Equals(kg, g);
            bool result2 = _equalityComparer.Equals(g, kg);

            // Assert
            Assert.IsTrue(result1, "Forward equality should hold");
            Assert.IsTrue(result2, "Reverse equality should hold (symmetric property)");
        }

        [TestMethod]
        public void TestEquality_PoundToKilogram_EquivalentValue()
        {
            // Arrange - accurate pound value for 1 kg
            var lb = new Quantity<WeightUnit>(2.2046226218, WeightUnit.POUND);
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool result = _equalityComparer.Equals(lb, kg);

            // Assert
            Assert.IsTrue(result, "2.2046226218 Pounds should equal 1 Kilogram");
        }

        [TestMethod]
        public void TestEquality_KilogramToGram_NonEquivalentValue()
        {
            // Arrange
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(500.0, WeightUnit.GRAM);

            // Act
            bool result = _equalityComparer.Equals(kg, g);

            // Assert
            Assert.IsFalse(result, "1 Kilogram should not equal 500 Grams");
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            // Arrange
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            object other = "not weight";

            // Act
            bool result = a.Equals(other);

            // Assert
            Assert.IsFalse(result, "Weight should not equal an object of different type");
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            // Arrange
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool result = _equalityComparer.Equals(a, null);

            // Assert
            Assert.IsFalse(result, "Weight should not equal null");
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            // Arrange
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            bool result = _equalityComparer.Equals(a, a);

            // Assert
            Assert.IsTrue(result, "Same reference should be equal");
        }


        

        [TestMethod]
        public void TestEquality_WithEpsilonPrecision()
        {
            // Arrange - Very close values within epsilon tolerance
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(1.0000005, WeightUnit.KILOGRAM);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert - Should be considered equal within epsilon
            Assert.IsTrue(result, "Values within epsilon should be considered equal");
        }

        [TestMethod]
        public void TestEquality_WithValuesJustBeyondEpsilon()
        {
            // Arrange - Values just beyond epsilon tolerance
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(1.0001, WeightUnit.KILOGRAM);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert - Should not be considered equal
            Assert.IsFalse(result, "Values beyond epsilon should not be equal");
        }

        [TestMethod]
        public void TestConversion_KilogramToGram()
        {
            // Arrange
            var kg = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            // Act
            double grams = _conversionService.Convert(kg.Value, kg.Unit, WeightUnit.GRAM);

            // Assert
            Assert.AreEqual(1000.0, grams, Eps, "1 Kilogram should convert to 1000 Grams");
        }

        [TestMethod]
        public void TestConversion_GramToKilogram()
        {
            // Arrange
            var g = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            // Act
            double kg = _conversionService.Convert(g.Value, g.Unit, WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(1.0, kg, Eps, "1000 Grams should convert to 1 Kilogram");
        }

        [TestMethod]
        public void TestConversion_PoundToKilogram()
        {
            // Arrange
            var lb = new Quantity<WeightUnit>(2.20462, WeightUnit.POUND);

            // Act
            double kg = _conversionService.Convert(lb.Value, lb.Unit, WeightUnit.KILOGRAM);

            // Assert
            Assert.AreEqual(1.0, kg, 1e-4, "2.20462 Pounds should convert to approximately 1 Kilogram");
        }

        
    }
}