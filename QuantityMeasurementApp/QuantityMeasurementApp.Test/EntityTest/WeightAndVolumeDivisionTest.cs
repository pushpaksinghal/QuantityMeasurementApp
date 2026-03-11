using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class WeightAndVolumeDIvisionTests
    {
        private const double Eps = 1e-9;
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
        public void testDivision_Weight_CrossUnit_GramDividedByKilogram()
        {
            var a = new Quantity<WeightUnit>(2000.0, WeightUnit.GRAM);
            var b = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(2.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_Volume_CrossUnit_MillilitreDividedByLitre()
        {
            var a = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(1.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_Volume_RatioGreaterThanOne()
        {
            var a = new Quantity<VolumeUnit>(10.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(5.0, VolumeUnit.LITRE);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(2.0, ratio, Eps);
        }
    }
}