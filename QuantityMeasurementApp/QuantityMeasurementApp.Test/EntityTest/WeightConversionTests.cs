using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightConversionTests
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
        public void testConversion_KilogramToGram()
        {
            double result = _conversionService.Convert(1.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_GramToKilogram()
        {
            double result = _conversionService.Convert(1000.0, WeightUnit.GRAM, WeightUnit.KILOGRAM);
            Assert.AreEqual(1.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            double result = _conversionService.Convert(2.0, WeightUnit.POUND, WeightUnit.KILOGRAM);
            Assert.AreEqual(0.907184, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            double result = _conversionService.Convert(1.0, WeightUnit.KILOGRAM, WeightUnit.POUND);
            Assert.AreEqual(2.2046226218, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_GramToPound()
        {
            double result = _conversionService.Convert(500.0, WeightUnit.GRAM, WeightUnit.POUND);
            Assert.AreEqual(1.102311, result, 1e-6);
        }

        [TestMethod]
        public void testConversion_PoundToGram()
        {
            double result = _conversionService.Convert(1.0, WeightUnit.POUND, WeightUnit.GRAM);
            Assert.AreEqual(453.592, result, 1e-3);
        }

        [TestMethod]
        public void testConversion_SameUnit_ReturnsSameValue()
        {
            double result = _conversionService.Convert(5.0, WeightUnit.KILOGRAM, WeightUnit.KILOGRAM);
            Assert.AreEqual(5.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            double result = _conversionService.Convert(0.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(0.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_NegativeValue_PreservesSign()
        {
            double result = _conversionService.Convert(-1.0, WeightUnit.KILOGRAM, WeightUnit.GRAM);
            Assert.AreEqual(-1000.0, result, Eps);
        }

        [TestMethod]
        public void testConversion_RoundTrip_PreservesValue()
        {
            double v = 123.456;

            double toPound = _conversionService.Convert(v, WeightUnit.KILOGRAM, WeightUnit.POUND);
            double backToKg = _conversionService.Convert(toPound, WeightUnit.POUND, WeightUnit.KILOGRAM);

            Assert.AreEqual(v, backToKg, 1e-6);
        }

        [TestMethod]
        public void testConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                _conversionService.Convert(double.NaN, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_PositiveInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                _conversionService.Convert(double.PositiveInfinity, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_NegativeInfinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                _conversionService.Convert(double.NegativeInfinity, WeightUnit.KILOGRAM, WeightUnit.GRAM)
            );
        }

        [TestMethod]
        public void testConversion_InvalidEnumValue_Throws()
        {
            WeightUnit bad = (WeightUnit)999;

            Assert.Throws<ArgumentException>(() =>
                _conversionService.Convert(1.0, bad, WeightUnit.GRAM)
            );
        }
    }
}