using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entity;

namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class VolumeTest
    {
        [TestMethod]
        public void TestEquality_LitreToLitre_SameValue()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void TestEquality_MillilitreToMillilitre_SameValue()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.MILLILITRE);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void TestEquality_GallonToGallon_SameValue()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var b = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void TestEquality_LitreToMillilitre_EquivalentValue()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(litre.Equals(ml));
        }

        [TestMethod]
        public void TestEquality_MillilitreToLitre_EquivalentValue()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            Assert.IsTrue(ml.Equals(litre));
        }

        [TestMethod]
        public void TestEquality_GallonToLitre_EquivalentValue()
        {
            // By definition: 1 gallon = 3.78541 litre
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.GALLON);
            var litre = new Quantity<VolumeUnit>(3.78541, VolumeUnit.LITRE);

            Assert.IsTrue(gallon.Equals(litre));
        }

        [TestMethod]
        public void TestEquality_LitreToGallon_EquivalentValue()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var gallon = new Quantity<VolumeUnit>(0.264172, VolumeUnit.GALLON); // approx 1/3.78541

            // epsilon equality expected
            Assert.IsTrue(litre.Equals(gallon));
        }

        [TestMethod]
        public void TestEquality_LitreToLitre_DifferentValue()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(2.0, VolumeUnit.LITRE);

            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void TestEquality_SameReference()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            Assert.IsTrue(a.Equals(a));
        }

        [TestMethod]
        public void TestEquality_NullComparison()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            Assert.IsFalse(a.Equals(null));
        }

        [TestMethod]
        public void TestEquality_DifferentType()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            object other = "not volume";

            Assert.IsFalse(a.Equals(other));
        }

        [TestMethod]
        public void TestUnitParsing_InvalidUnit_ShouldFail()
        {
            bool ok = System.Enum.TryParse("CUBICMETER", ignoreCase: true, out VolumeUnit _);
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void TestUnitParsing_ValidUnits_ShouldPass()
        {
            bool ok1 = System.Enum.TryParse("LITRE", ignoreCase: true, out VolumeUnit u1);
            bool ok2 = System.Enum.TryParse("MILLILITRE", ignoreCase: true, out VolumeUnit u2);

            Assert.IsTrue(ok1);
            Assert.AreEqual(VolumeUnit.LITRE, u1);

            Assert.IsTrue(ok2);
            Assert.AreEqual(VolumeUnit.MILLILITRE, u2);
        }

        [TestMethod]
        public void TestEquality_TransitiveProperty()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);
            var c = new Quantity<VolumeUnit>(0.264172, VolumeUnit.GALLON);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }
    }
}