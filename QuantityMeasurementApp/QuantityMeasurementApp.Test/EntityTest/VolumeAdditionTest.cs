using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeAdditionTest
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
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(1000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddUnit(litre, ml);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            // Arrange
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddUnit(ml, litre);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(2000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            // Arrange
            var gal = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var litre = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddUnit(gal, litre);

            // Assert
            Assert.AreEqual(VolumeUnit.GALLON, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var zeroMl = new Quantity<VolumeUnit>(0.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddUnit(litre, zeroMl);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(-2000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InBaseLitre()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            var sum1 = _arithmeticService.AddUnit(a, b);
            var sum2 = _arithmeticService.AddUnit(b, a);

            // Assert - Convert both to litres and compare
            double s1L = _conversionService.Convert(sum1.Value, sum1.Unit, VolumeUnit.LITRE);
            double s2L = _conversionService.Convert(sum2.Value, sum2.Unit, VolumeUnit.LITRE);

            Assert.AreEqual(s1L, s2L, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddUnit(a, null!));
        }

        [TestMethod]
        public void testAddition_NullFirstOperand_Throws()
        {
            // Arrange
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddUnit(null!, b));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.LITRE);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert
            Assert.AreEqual(VolumeUnit.MILLILITRE, sum.Unit);
            Assert.AreEqual(2000000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_FractionalValues()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(0.5, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(750.0, VolumeUnit.MILLILITRE);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(1.25, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_PreservesOriginalQuantities()
        {
            // Arrange
            var originalA = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var originalB = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var a = new Quantity<VolumeUnit>(originalA.Value, originalA.Unit);
            var b = new Quantity<VolumeUnit>(originalB.Value, originalB.Unit);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert - Original quantities unchanged
            Assert.AreEqual(originalA.Value, a.Value);
            Assert.AreEqual(originalA.Unit, a.Unit);
            Assert.AreEqual(originalB.Value, b.Value);
            Assert.AreEqual(originalB.Unit, b.Unit);

            // Sum is correct
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_WithAllVolumeUnits()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);
            var gallon = new Quantity<VolumeUnit>(0.264172, VolumeUnit.GALLON); // ≈ 1 litre

            // Act
            var step1 = _arithmeticService.AddUnit(litre, ml);
            var step2 = _arithmeticService.AddUnit(step1, gallon);

            // Assert
            Assert.AreEqual(VolumeUnit.LITRE, step2.Unit);
            Assert.AreEqual(2.5, step2.Value, 1e-3);
        }

        [TestMethod]
        public void testAddition_DifferentUnits_ResultInFirstUnit()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);  // 1 gallon
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            // Act
            var sum = _arithmeticService.AddUnit(a, b);

            // Assert - Result should be in litres (first operand's unit)
            Assert.AreEqual(VolumeUnit.LITRE, sum.Unit);
            Assert.AreEqual(7.57082, sum.Value, 1e-5);
        }
    }
}