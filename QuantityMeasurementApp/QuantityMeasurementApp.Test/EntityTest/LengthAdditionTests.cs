using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthAdditionTests
    {
        private const double Eps = 1e-6;

        [TestMethod]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SameUnit_InchPlusInch()
        {
            var a = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(6.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(12.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = feet.AddUnitTO(inches);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            var inches = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);
            var feet = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = inches.AddUnitTO(feet);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(24.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            var yards = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);
            var feet = new Quantity<LengthUnit>(3.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = yards.AddUnitTO(feet);

            Assert.AreEqual(LengthUnit.YARD, sum.Unit);
            Assert.AreEqual(2.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_InchPlusYard()
        {
            var inches = new Quantity<LengthUnit>(36.0, LengthUnit.INCH);
            var yard = new Quantity<LengthUnit>(1.0, LengthUnit.YARD);

            Quantity<LengthUnit> sum = inches.AddUnitTO(yard);

            Assert.AreEqual(LengthUnit.INCH, sum.Unit);
            Assert.AreEqual(72.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            var cm = new Quantity<LengthUnit>(2.54, LengthUnit.CENTIMETERS);  // ~1 inch
            var inch = new Quantity<LengthUnit>(1.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = cm.AddUnitTO(inch);

            Assert.AreEqual(LengthUnit.CENTIMETERS, sum.Unit);
            Assert.AreEqual(5.08, sum.Value, 1e-3); // looser tolerance due to rounded cm factor
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var feet = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var zeroInches = new Quantity<LengthUnit>(0.0, LengthUnit.INCH);

            Quantity<LengthUnit> sum = feet.AddUnitTO(zeroInches);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(5.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(-2.0, LengthUnit.FEET);

            Quantity<LengthUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
            Assert.AreEqual(3.0, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_Commutativity_InInchesBase()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            // Convert both results to inches and compare
            double sum1InInches = Quantity<LengthUnit>.Convert(a.AddUnitTO(b).Value, a.AddUnitTO(b).Unit, LengthUnit.INCH);
            double sum2InInches = Quantity<LengthUnit>.Convert(b.AddUnitTO(a).Value, b.AddUnitTO(a).Unit, LengthUnit.INCH);

            Assert.AreEqual(sum1InInches, sum2InInches, 1e-9);
        }

        [TestMethod]
        public void testAddition_NullSecondOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            Assert.Throws<ArgumentException>(() => a.AddUnitTO(null));
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            var a = new Quantity<LengthUnit>(1e6, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(1e6, LengthUnit.FEET);

            Quantity<LengthUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(2e6, sum.Value, Eps);
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var a = new Quantity<LengthUnit>(0.001, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(0.002, LengthUnit.FEET);

            Quantity<LengthUnit> sum = a.AddUnitTO(b);

            Assert.AreEqual(0.003, sum.Value, 1e-12);
        }
    }
}