using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthSubtrationTest
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_FeetMinusInches_ImplicitTargetFeet()
        {
            var feet = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var inches = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            var diff = feet.SubtractUnitTo(inches);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(9.5, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_CrossUnit_InchesMinusFeet_ImplicitTargetInch()
        {
            var inches = new Quantity<LengthUnit>(120.0, LengthUnit.INCH);
            var feet = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var diff = inches.SubtractUnitTo(feet);

            Assert.AreEqual(LengthUnit.INCH, diff.Unit);
            Assert.AreEqual(60.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            var diff = a.SubtractUnitTo(b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, diff.Unit);
            Assert.AreEqual(114.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(-5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(120.0, LengthUnit.INCH); // 10 feet

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(0.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_WithZeroOperand()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(0.0, LengthUnit.INCH);

            var diff = a.SubtractUnitTo(b);

            Assert.AreEqual(LengthUnit.FEET, diff.Unit);
            Assert.AreEqual(5.0, diff.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            var ab = a.SubtractUnitTo(b);
            var ba = b.SubtractUnitTo(a);

            Assert.AreNotEqual(ab.Value, ba.Value, "Subtraction must be non-commutative.");
            Assert.AreEqual(5.0, ab.Value, Eps);
            Assert.AreEqual(-5.0, ba.Value, Eps);
        }

        [TestMethod]
        public void testSubtraction_NullOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            Assert.Throws<ArgumentException>(() => a.SubtractUnitTo(null));
        }
    }
}