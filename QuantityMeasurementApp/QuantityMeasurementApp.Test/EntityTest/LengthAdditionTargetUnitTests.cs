using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;
using static QuantityMeasurementApp.Core.Entity.Length;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthAdditionTargetUnitTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(12.0, LengthUnit.INCH);

            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inch()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(12.0, LengthUnit.INCH);

            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(24.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yard()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(12.0, LengthUnit.INCH);

            // 1 ft + 12 in = 24 in = 2 ft = 2/3 yd = 0.666666...
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            var a = new Length(1.0, LengthUnit.INCH);
            var b = new Length(1.0, LengthUnit.INCH);

            // 2 inches = 5.08 cm (approximately)
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.CENTIMETERS);

            Assert.AreEqual(LengthUnit.CENTIMETERS, sum.Unit);
            Assert.AreEqual(5.08, sum.Value, 1e-3); // looser due to cm factor rounding
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            var a = new Length(1.0, LengthUnit.YARD);
            var b = new Length(3.0, LengthUnit.FEET);

            // 1 yd + 3 ft = 1 yd + 1 yd = 2 yd
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            var a = new Length(2.0, LengthUnit.YARD);
            var b = new Length(3.0, LengthUnit.FEET);

            // 2 yd = 6 ft; +3 ft => 9 ft
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.FEET);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(9.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(12.0, LengthUnit.INCH);

            Length sum1 = Length.AddToSpecificUnit(a, b, LengthUnit.YARD);
            Length sum2 = Length.AddToSpecificUnit(b, a, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum1.Unit);
            Assert.AreEqual(LengthUnit.YARD, sum2.Unit);
            Assert.AreEqual(sum1.Value, sum2.Value, 1e-9);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            var a = new Length(5.0, LengthUnit.FEET);
            var b = new Length(0.0, LengthUnit.INCH);

            // 5 ft => 5/3 yd = 1.666666...
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(5.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            var a = new Length(5.0, LengthUnit.FEET);
            var b = new Length(-2.0, LengthUnit.FEET);

            // 3 ft = 36 in
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(36.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            var a = new Length(1000.0, LengthUnit.FEET);
            var b = new Length(500.0, LengthUnit.FEET);

            // 1500 ft = 18000 in
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.INCH);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(18000.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            var a = new Length(12.0, LengthUnit.INCH);
            var b = new Length(12.0, LengthUnit.INCH);

            // 24 inches = 2 feet = 2/3 yard
            Length sum = Length.AddToSpecificUnit(a, b, LengthUnit.YARD);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0 / 3.0, sum.Value, 1e-6);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullSecondOperand_Throws()
        {
            var a = new Length(1.0, LengthUnit.FEET);

            Assert.Throws<ArgumentException>(() =>
                Length.AddToSpecificUnit(a, null, LengthUnit.FEET)
            );
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_InvalidTargetUnit_Throws()
        {
            var a = new Length(1.0, LengthUnit.FEET);
            var b = new Length(1.0, LengthUnit.FEET);

            LengthUnit badTarget = (LengthUnit)999;

            Assert.Throws<ArgumentException>(() =>
                Length.AddToSpecificUnit(a, b, badTarget)
            );
        }
    }
}