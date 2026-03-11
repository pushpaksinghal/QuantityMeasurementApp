using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeSubtractionTest
    {
        private const double Eps = 1e-6;
        private IQuantityConversionService _conversionService;
        private IQuantityArithmeticService _arithmeticService;
        private QuantityEqualityComparer<VolumeUnit> _equalityComparer;
        private QuantityValidationService _validator;
        private UnitAdapterFactory _adapterFactory;

        [TestInitialize]
        public void Setup()
        {
            _adapterFactory = new UnitAdapterFactory();
            _validator = new QuantityValidationService();
            _conversionService = new QuantityConversionService(_adapterFactory, _validator);
            _arithmeticService = new QuantityArithmeticService(_adapterFactory, _validator);
            _equalityComparer = new QuantityEqualityComparer<VolumeUnit>(_adapterFactory, _validator);
        }

        [TestMethod]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(7.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_LitreMinusMillilitre_ResultInLitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(4.5, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_MillilitreMinusLitre_ResultInMillilitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5000.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, diff.Unit);
            Assert.AreEqual(4000.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, diff.Unit);
            Assert.AreEqual(3000.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Gallon()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(7.57082, VolumeUnit.LITRE); // 2 gallons
            var b = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);  // 1 gallon

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, diff.Unit);
            Assert.AreEqual(1.0, diff.Value, 1e-5);
        }

        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(0.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(5000.0, VolumeUnit.MILLILITRE); // 5 litres

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(-2.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_WithZero()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);

            // Act
            var ab = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);
            var ba = _arithmeticService.SubtractUnit(b, a, VolumeUnit.LITRE);

            // Assert
            Assert.AreNotEqual(ab.Value, ba.Value, "Subtraction must be non-commutative.");
            Assert.AreEqual(7.0, ab.Value, Eps);
            Assert.AreEqual(-7.0, ba.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NullFirstOperand_Throws()
        {
            // Arrange
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.SubtractUnit(null!, b, VolumeUnit.LITRE));
        }

        [TestMethod]
        public void testSubtraction_NullSecondOperand_Throws()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.SubtractUnit(a, null!, VolumeUnit.LITRE));
        }

        [TestMethod]
        public void testSubtraction_InvalidTargetUnit_Throws()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);
            VolumeUnit badTarget = (VolumeUnit)999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.SubtractUnit(a, b, badTarget));
        }

        [TestMethod]
        public void testSubtraction_PreservesOriginalQuantities()
        {
            // Arrange
            var originalA = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var originalB = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var a = new Quantity<VolumeUnit>(originalA.Value, originalA.Unit);
            var b = new Quantity<VolumeUnit>(originalB.Value, originalB.Unit);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert - Original quantities unchanged
            Assert.AreEqual(originalA.Value, a.Value);
            Assert.AreEqual(originalA.Unit, a.Unit);
            Assert.AreEqual(originalB.Value, b.Value);
            Assert.AreEqual(originalB.Unit, b.Unit);

            // Difference is correct
            Assert.AreEqual(4.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_LargeValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(500.0, VolumeUnit.LITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(500.0, diff.Value, Eps); // 1000L - 500L = 500L
        }

        [TestMethod]
        public void testSubtraction_FractionalValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(2.5, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(750.0, VolumeUnit.MILLILITRE);

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, diff.Unit);
            Assert.AreEqual(1.75, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_GallonAndLitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(2.0, VolumeUnit.GALLON);
            var b = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE); // 1 gallon

            // Act
            var diff = _arithmeticService.SubtractUnit(a, b, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, diff.Unit);
            Assert.AreEqual(1.0, diff.Value, 1e-5);
        }
    }
}