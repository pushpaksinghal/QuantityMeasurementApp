using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightSubtractionTest
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
        public void testSubtraction_SameUnit_KgMinusKg()
        {
            var a = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            var diff = _arithmeticService.SubtractUnit(a,b, WeightUnit.KILOGRAM);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_KgMinusGram_ImplicitTargetKg()
        {
            var kg = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(5000.0, WeightUnit.GRAM);

            var diff = _arithmeticService.SubtractUnit(kg, g, WeightUnit.KILOGRAM);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Gram()
        {
            var kg = new Quantity<WeightUnit>(10.0, WeightUnit.KILOGRAM);
            var g = new Quantity<WeightUnit>(5000.0, WeightUnit.GRAM);

            var diff = _arithmeticService.SubtractUnit(kg,g, WeightUnit.GRAM);

            Assert.AreEqual(WeightUnit.GRAM, diff.Unit);
            Assert.AreEqual(5000.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var a = new Quantity<WeightUnit>(2.0, WeightUnit.KILOGRAM);
            var b = new Quantity<WeightUnit>(5.0, WeightUnit.KILOGRAM);

            var diff = _arithmeticService.SubtractUnit(a, b, WeightUnit.KILOGRAM);

            Assert.AreEqual(WeightUnit.KILOGRAM, diff.Unit);
            Assert.AreEqual(-3.0, diff.Value, Eps);
        }
    }
}