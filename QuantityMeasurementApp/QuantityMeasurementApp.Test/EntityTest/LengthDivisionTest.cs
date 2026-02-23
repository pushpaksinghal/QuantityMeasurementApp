using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class LengthDivisionTest
    {
        private const double Eps = 1e-9;

        [TestMethod]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(5.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_CrossUnit_InchesDividedByFeet_Equals1()
        {
            var a = new Quantity<LengthUnit>(24.0, LengthUnit.INCH);
            var b = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(1.0, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_RatioLessThanOne()
        {
            var a = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);

            double ratio = a.DivideUnitTo(b);

            Assert.AreEqual(0.5, ratio, Eps);
        }

        [TestMethod]
        public void testDivision_NonCommutative()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.FEET);

            double ab = a.DivideUnitTo(b);
            double ba = b.DivideUnitTo(a);

            Assert.AreEqual(2.0, ab, Eps);
            Assert.AreEqual(0.5, ba, Eps);
        }

        [TestMethod]
        public void testDivision_ByZero_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var zero = new Quantity<LengthUnit>(0.0, LengthUnit.FEET);

            Assert.Throws<DivideByZeroException>(() => a.DivideUnitTo(zero));
        }

        [TestMethod]
        public void testDivision_NullOperand_Throws()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            Assert.Throws<ArgumentException>(() => a.DivideUnitTo(null));
        }
    }
}