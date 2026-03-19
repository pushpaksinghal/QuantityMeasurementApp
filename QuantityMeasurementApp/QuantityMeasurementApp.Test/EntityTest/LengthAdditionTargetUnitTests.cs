using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthAdditionTargetUnitTests
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
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inch()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(24.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yard()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // 1 ft + 12 in = 24 in = 2 ft = 2/3 yd = 0.666666...
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);

            // 2 inches = 5.08 cm (approximately)
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.CENTIMETERS);

            Assert.AreEqual(LengthUnit.CENTIMETERS, sum.Unit);
            Assert.AreEqual(5.08, sum.Value, 1e-3); // looser due to cm factor rounding
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var b = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);

            // 1 yd + 3 ft = 1 yd + 1 yd = 2 yd
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var a = new Quantity<LengthUnit>(2.0, LengthUnit.YARD);
            var b = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);

            // 2 yd = 6 ft; +3 ft => 9 ft
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(9.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum1 = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.YARD);
            Quantity<LengthUnit> sum2 = _arithmeticService.AddToSpecificUnit(b, a, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum1.Unit);
            Assert.AreEqual(LengthUnit.YARD, sum2.Unit);
            Assert.AreEqual(sum1.Value, sum2.Value, 1e-9);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(0.0, LengthUnit.INCH);

            // 5 ft => 5/3 yd = 1.666666...
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(5.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(-2.0, LengthUnit.FEET);

            // 3 ft = 36 in
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(36.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            var a = new Quantity<LengthUnit>(1000.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(500.0, LengthUnit.FEET);

            // 1500 ft = 18000 in
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(18000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            var a = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // 24 inches = 2 feet = 2/3 yard
            Quantity<LengthUnit> sum = _arithmeticService.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullSecondOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddToSpecificUnit(a, null, LengthUnit.FEET)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            LengthUnit badTarget = (LengthUnit)999;

            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.AddToSpecificUnit(a, b, badTarget)
            );
        }
    }
}