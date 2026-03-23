using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthAdditionTests
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
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(a,b);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_InchPlusInch()
        {
            var a = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(a,b);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(12.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(feet,inches);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(inches,feet);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(24.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            var yards = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feet = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(yards,feet);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusYard()
        {
            var inches = new Quantity<LengthUnit>(36.0, LengthUnit.INCH);
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(inches,yard);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(72.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            var cm = new Quantity<LengthUnit>(2.54, LengthUnit.CENTIMETERS);  // ~1 inch
            var inch = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(cm,inch);

            Assert.AreEqual(LengthUnit.CENTIMETERS, sum.Unit);
            Assert.AreEqual(5.08, sum.Value, 1e-3); // looser tolerance due to rounded cm factor
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var feet = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var zeroInches = new Quantity<LengthUnit>(0.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(feet,zeroInches);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(-2.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(a,b);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InInchesBase()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Convert both results to inches and compare
            double sum1InInches = _conversionService.Convert(_arithmeticService.AddUnit(a,b).Value, _arithmeticService.AddUnit(a,b).Unit, LengthUnit.INCH);
            double sum2InInches = _conversionService.Convert(_arithmeticService.AddUnit(b,a).Value, _arithmeticService.AddUnit(b,a).Unit, LengthUnit.INCH);

            Assert.AreEqual(sum1InInches, sum2InInches, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            Assert.Throws<ArgumentException>(() => _arithmeticService.AddUnit(a,null));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            var a = new Quantity<LengthUnit>(1e6, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(1e6, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(a,b);

            Assert.AreEqual(2e6, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var a = new Quantity<LengthUnit>(0.001, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(0.002, LengthUnit.FEET);

            Quantity<LengthUnit> sum = _arithmeticService.AddUnit(a,b);

            Assert.AreEqual(0.003, sum.Value, 1e-12);
        }
    }
}