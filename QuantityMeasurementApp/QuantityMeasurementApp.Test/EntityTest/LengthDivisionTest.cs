using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthDivisionTest
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
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(5.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_CrossUnit_InchesDividedByFeet_Equals1()
        {
            var a = new Quantity<LengthUnit>(24.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(1.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_RatioLessThanOne()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);

            double ratio = _arithmeticService.DivideUnit(a,b);

            Assert.AreEqual(0.5, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_NonCommutative()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            double ab = _arithmeticService.DivideUnit(a,b);
            double ba = _arithmeticService.DivideUnit(b,a);

            Assert.AreEqual(2.0, ab, Eps);
            Assert.AreEqual(0.5, ba, Eps);
        }

        [TestMethod]
        public void testDivision_ByZero_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var zero = new Quantity<LengthUnit>(0.0, LengthUnit.FEET);

            Assert.Throws<DivideByZeroException>(() => _arithmeticService.DivideUnit(a,zero));
        }

        [TestMethod]
        public void testDivision_NullOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            Assert.Throws<ArgumentException>(() => _arithmeticService.DivideUnit(a,null));
        }
    }
}