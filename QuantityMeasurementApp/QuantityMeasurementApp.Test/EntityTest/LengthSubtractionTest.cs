using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthSubtractionTest
    {
        private const double Eps = 1e-6;
        private IQuantityConversionService _conversionService;
        private IQuantityArithmeticService _arithmeticService;

        [TestInitialize]
        public void Setup()
        {
            // Create real instances with their dependencies
            var validator = new QuantityValidationService();
            var adapterFactory = new UnitAdapterFactory();

            _conversionService = new QuantityConversionService(adapterFactory, validator);
            _arithmeticService = new QuantityArithmeticService(adapterFactory, validator);
        }

        [TestMethod]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var diff = _arithmeticService.SubtractUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_FeetMinusInches_ImplicitTargetFeet()
        {
            var feet = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inches = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            var diff = _arithmeticService.SubtractUnit(feet, inches, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(9.5, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_InchesMinusFeet_ImplicitTargetInch()
        {
            var inches = new Quantity<LengthUnit>(120.0, LengthUnit.INCH);
            var feet = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var diff = _arithmeticService.SubtractUnit(inches, feet, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, diff.Unit);
            Assert.AreEqual(60.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            var diff = _arithmeticService.SubtractUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, diff.Unit);
            Assert.AreEqual(114.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);

            var diff = _arithmeticService.SubtractUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(-5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(120.0, LengthUnit.INCH); // 10 feet

            var diff = _arithmeticService.SubtractUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(0.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_WithZeroOperand()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(0.0, LengthUnit.INCH);

            var diff = _arithmeticService.SubtractUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var ab = _arithmeticService.SubtractUnit(a, b, LengthUnit.FEET);
            var ba = _arithmeticService.SubtractUnit(b, a, LengthUnit.FEET);

            Assert.AreNotEqual(ab.Value, ba.Value, "Subtraction must be non-commutative.");
            Assert.AreEqual(5.0, ab.Value, Eps);
            Assert.AreEqual(-5.0, ba.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NullOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);

            Assert.Throws<ArgumentException>(() =>
                _arithmeticService.SubtractUnit(a, null!, LengthUnit.FEET));
        }
    }
}