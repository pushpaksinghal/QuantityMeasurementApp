using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeTest
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
        public void TestEquality_LitreToLitre_SameValue()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert
            Assert.IsTrue(result, "1 Litre should equal 1 Litre");
        }

        [TestMethod]
        public void TestEquality_MillilitreToMillilitre_SameValue()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.MILLILITRE);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert
            Assert.IsTrue(result, "1 Millilitre should equal 1 Millilitre");
        }

        [TestMethod]
        public void TestEquality_GallonToGallon_SameValue()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert
            Assert.IsTrue(result, "1 Gallon should equal 1 Gallon");
        }

        [TestMethod]
        public void TestEquality_LitreToMillilitre_EquivalentValue()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            bool result = _equalityComparer.Equals(litre, ml);

            // Assert
            Assert.IsTrue(result, "1 Litre should equal 1000 Millilitres");
        }

        [TestMethod]
        public void TestEquality_MillilitreToLitre_EquivalentValue()
        {
            // Arrange
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(ml, litre);

            // Assert
            Assert.IsTrue(result, "1000 Millilitres should equal 1 Litre (symmetric property)");
        }

        [TestMethod]
        public void TestEquality_GallonToLitre_EquivalentValue()
        {
            // Arrange - By definition: 1 gallon = 3.78541 litre
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var litre = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(gallon, litre);

            // Assert
            Assert.IsTrue(result, "1 Gallon should equal 3.78541 Litres");
        }

        [TestMethod]
        public void TestEquality_LitreToGallon_EquivalentValue()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var gallon = new Quantity<VolumeUnit>(0.264172, VolumeUnit.GALLON); // approx 1/3.78541

            // Act
            bool result = _equalityComparer.Equals(litre, gallon);

            // Assert
            Assert.IsTrue(result, "1 Litre should equal approximately 0.264172 Gallons");
        }

        [TestMethod]
        public void TestEquality_GallonToMillilitre_EquivalentValue()
        {
            // Arrange - 1 gallon = 3785.41 millilitres
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var ml = new Quantity<VolumeUnit>(3785.41, VolumeUnit.MILLILITRE);

            // Act
            bool result = _equalityComparer.Equals(gallon, ml);

            // Assert
            Assert.IsTrue(result, "1 Gallon should equal 3785.41 Millilitres");
        }

        [TestMethod]
        public void TestEquality_LitreToLitre_DifferentValue()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert
            Assert.IsFalse(result, "1 Litre should not equal 2 Litres");
        }

        [TestMethod]
        public void TestEquality_GallonToGallon_DifferentValue()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.GALLON);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert
            Assert.IsFalse(result, "1 Gallon should not equal 2 Gallons");
        }

        [TestMethod]
        public void TestEquality_LitreToMillilitre_NonEquivalentValue()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(500.0, VolumeUnit.MILLILITRE);

            // Act
            bool result = _equalityComparer.Equals(litre, ml);

            // Assert
            Assert.IsFalse(result, "1 Litre should not equal 500 Millilitres");
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, a);

            // Assert
            Assert.IsTrue(result, "Same reference should be equal");
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, null);

            // Assert
            Assert.IsFalse(result, "Volume should not equal null");
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            object other = "not volume";

            // Act
            bool result = a.Equals(other);

            // Assert
            Assert.IsFalse(result, "Volume should not equal an object of different type");
        }

        [TestMethod]
        public void TestUnitParsing_InvalidUnit_ShouldFail()
        {
            // Act
            bool ok = Enum.TryParse("CUBICMETER", ignoreCase: true, out VolumeUnit _);

            // Assert
            Assert.IsFalse(ok, "Parsing invalid unit 'CUBICMETER' should fail");
        }

        [TestMethod]
        public void TestUnitParsing_ValidUnits_ShouldPass()
        {
            // Act
            bool ok1 = Enum.TryParse("LITRE", ignoreCase: true, out VolumeUnit u1);
            bool ok2 = Enum.TryParse("MILLILITRE", ignoreCase: true, out VolumeUnit u2);
            bool ok3 = Enum.TryParse("GALLON", ignoreCase: true, out VolumeUnit u3);

            // Assert
            Assert.IsTrue(ok1, "Parsing LITRE should succeed");
            Assert.AreEqual(VolumeUnit.LITRE, u1, "Parsed unit should be LITRE");

            Assert.IsTrue(ok2, "Parsing MILLILITRE should succeed");
            Assert.AreEqual(VolumeUnit.MILLILITRE, u2, "Parsed unit should be MILLILITRE");

            Assert.IsTrue(ok3, "Parsing GALLON should succeed");
            Assert.AreEqual(VolumeUnit.GALLON, u3, "Parsed unit should be GALLON");
        }

        [TestMethod]
        public void TestEquality_TransitiveProperty()
        {
            // Arrange
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var c = new Quantity<VolumeUnit>(0.264172, VolumeUnit.GALLON);

            // Act
            bool aEqualsB = _equalityComparer.Equals(a, b);
            bool bEqualsC = _equalityComparer.Equals(b, c);
            bool aEqualsC = _equalityComparer.Equals(a, c);

            // Assert
            Assert.IsTrue(aEqualsB, "1 Litre should equal 1000 Millilitres");
            Assert.IsTrue(bEqualsC, "1000 Millilitres should equal 0.264172 Gallons");
            Assert.IsTrue(aEqualsC, "Transitive property: 1 Litre should equal 0.264172 Gallons");
        }

        [TestMethod]
        public void TestEquality_WithEpsilonPrecision()
        {
            // Arrange - Very close values within epsilon tolerance
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0000005, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert - Should be considered equal within epsilon
            Assert.IsTrue(result, "Values within epsilon should be considered equal");
        }

        [TestMethod]
        public void TestEquality_WithValuesJustBeyondEpsilon()
        {
            // Arrange - Values just beyond epsilon tolerance
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0001, VolumeUnit.LITRE);

            // Act
            bool result = _equalityComparer.Equals(a, b);

            // Assert - Should not be considered equal
            Assert.IsFalse(result, "Values beyond epsilon should not be equal");
        }

        [TestMethod]
        public void TestConversion_LitreToMillilitre()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            double ml = _conversionService.Convert(litre.Value, litre.Unit, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(1000.0, ml, Eps, "1 Litre should convert to 1000 Millilitres");
        }

        [TestMethod]
        public void TestConversion_MillilitreToLitre()
        {
            // Arrange
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            // Act
            double litre = _conversionService.Convert(ml.Value, ml.Unit, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(1.0, litre, Eps, "1000 Millilitres should convert to 1 Litre");
        }

        [TestMethod]
        public void TestConversion_GallonToLitre()
        {
            // Arrange
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            // Act
            double litre = _conversionService.Convert(gallon.Value, gallon.Unit, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(3.78541, litre, 1e-5, "1 Gallon should convert to 3.78541 Litres");
        }

        [TestMethod]
        public void TestConversion_LitreToGallon()
        {
            // Arrange
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            // Act
            double gallon = _conversionService.Convert(litre.Value, litre.Unit, VolumeUnit.GALLON);

            // Assert
            Assert.AreEqual(0.264172, gallon, 1e-6, "1 Litre should convert to approximately 0.264172 Gallons");
        }

        [TestMethod]
        public void TestConversion_GallonToMillilitre()
        {
            // Arrange
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            // Act
            double ml = _conversionService.Convert(gallon.Value, gallon.Unit, VolumeUnit.MILLILITRE);

            // Assert
            Assert.AreEqual(3785.41, ml, 1e-2, "1 Gallon should convert to 3785.41 Millilitres");
        }

        [TestMethod]
        public void TestConversion_RoundTrip_LitreToGallonToLitre()
        {
            // Arrange
            var original = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);

            // Act
            double toGallon = _conversionService.Convert(original.Value, original.Unit, VolumeUnit.GALLON);
            double backToLitre = _conversionService.Convert(toGallon, VolumeUnit.GALLON, VolumeUnit.LITRE);

            // Assert
            Assert.AreEqual(original.Value, backToLitre, 1e-5, "Round-trip conversion should preserve value");
        }

        
    }
}