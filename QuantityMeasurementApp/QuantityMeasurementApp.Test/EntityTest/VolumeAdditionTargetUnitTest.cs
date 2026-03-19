using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeAdditionTargetUnitTest
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
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum1 = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.GALLON);
            var sum2 = _arithmeticService.AddToSpecificUnit(b, a, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, sum1.Unit);
            Assert.AreEqual(VolumeUnit.GALLON, sum2.Unit);
            Assert.AreEqual(sum1.Value, sum2.Value, 1e-9);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullSecondOperand_Throws()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddToSpecificUnit(a, null!, VolumeUnit.LITRE)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            VolumeUnit badTarget = (VolumeUnit)999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddToSpecificUnit(a, b, badTarget)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_DifferentVolumes_WithZero()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_MultipleUnits()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var gallon = new Quantity<VolumeUnit>(0.5, VolumeUnit.GALLON);

            // Act - Add litre + ml to gallon
            var step1 = _arithmeticService.AddToSpecificUnit(litre, ml, VolumeUnit.GALLON);
            var final = _arithmeticService.AddToSpecificUnit(step1, gallon, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, final.Unit);
            Assert.IsTrue(final.Value > 0);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_PreservesOriginalQuantities()
        {
            // Arrange
            var originalA = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var originalB = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var a = new Quantity<VolumeUnit>(originalA.Value, originalA.Unit);
            var b = new Quantity<VolumeUnit>(originalB.Value, originalB.Unit);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            // Assert - Original quantities unchanged
            Assert.AreEqual(originalA.Value, a.Value);
            Assert.AreEqual(originalA.Unit, a.Unit);
            Assert.AreEqual(originalB.Value, b.Value);
            Assert.AreEqual(originalB.Unit, b.Unit);

            // Sum is correct
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1000.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, sum.Unit);
            Assert.AreEqual(528.344, sum.Value, 1e-3); // 2000 litres ≈ 528.344 gallons
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameUnitAsOperands()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(3.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(8.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithNegativeValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(-3.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddToSpecificUnit(a, b, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(7.0, sum.Value, Eps);
        }
    }
}