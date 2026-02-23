//using QuantityMeasurementApp.Core.Entity;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace QuantityMeasurementApp.Test.EntityTest
//{
//    //making a testing class for inches to test all the edge cases
//    [TestClass]
//    public class InchesTests
//    {
//        [TestMethod]
//        public void TestEquality_SameValue()
//        {
//            var f1 = new Inches(1.0);
//            var f2 = new Inches(1.0);

//            Assert.IsTrue(f1.Equals(f2));
//        }

//        [TestMethod]
//        public void TestEquality_DifferentValue()
//        {
//            var f1 = new Inches(1.0);
//            var f2 = new Inches(2.0);

//            Assert.IsFalse(f1.Equals(f2));
//        }

//        [TestMethod]
//        public void TestEquality_Null()
//        {
//            var f1 = new Inches(1.0);

//            Assert.IsFalse(f1.Equals(null));
//        }

//        [TestMethod]
//        public void TestEquality_SameReference()
//        {
//            var f1 = new Inches(1.0);

//            Assert.IsTrue(f1.Equals(f1));
//        }

//        [TestMethod]
//        public void TestEquality_DifferentType()
//        {
//            var f1 = new Inches(1.0);
//            object obj = "test";

//            Assert.IsFalse(f1.Equals(obj));
//        }
//    }
//}
