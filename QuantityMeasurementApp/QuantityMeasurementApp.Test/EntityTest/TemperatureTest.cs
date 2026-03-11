using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Core.Entity;


namespace QuantityMeasurementApp.Test.EntityTest
{
    [TestClass]
    public class TemperatureTest
    {
        private const double Eps = 1e-6;
        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.FAHRENHEIT);
            var b = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");

        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool result = a.Equals(b);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_SymmetricProperty()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);
            var b = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.FAHRENHEIT);

            // When
            bool rea = a.Equals(b);
            bool reb = b.Equals(a);

            bool result = rea.Equals(reb);
            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            // Given
            var a = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);

            // When
            bool result = a.Equals(a);

            // Then
            Assert.IsTrue(result, "Should Be Equal");
        }

        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            double result = Quantity<TemperatureUnit>.Convert(50.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(122.0, result, Eps);

            double result1 = Quantity<TemperatureUnit>.Convert(-20.0, TemperatureUnit.CELSIUS, TemperatureUnit.FAHRENHEIT);
            Assert.AreEqual(-4.0, result1, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            double result = Quantity<TemperatureUnit>.Convert(122.0, TemperatureUnit.FAHRENHEIT, TemperatureUnit.CELSIUS);
            Assert.AreEqual(50.0, result, Eps);

            double result1 = Quantity<TemperatureUnit>.Convert(-4.0, TemperatureUnit.FAHRENHEIT, TemperatureUnit.CELSIUS);
            Assert.AreEqual(-20.0, result1, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            // Arrange
            var tempC = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            // Act
            double toF = tempC.ConvertTo(TemperatureUnit.FAHRENHEIT);
            double backToC = new Quantity<TemperatureUnit>(toF,TemperatureUnit.FAHRENHEIT).ConvertTo(TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(tempC.Value, backToC);
        }

        [TestMethod]
        public void testTemperatureConversion_SameUnit()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(37.0, TemperatureUnit.CELSIUS);

            // Act
            var result = temp.ConvertTo(TemperatureUnit.CELSIUS);

            // Assert
            Assert.AreEqual(37.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_ZeroValue()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            // Act
            var result = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(32.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_NegativeValues()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.CELSIUS);

            // Act
            var result = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(-40.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureConversion_LargeValues()
        {
            // Arrange
            var temp = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.CELSIUS);

            // Act
            var result = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            // Assert
            Assert.AreEqual(1832.0, result, Eps);
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Add()
        {
            // Arrange
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act + Assert
            var ex = Assert.Throws<InvalidOperationException>(() => t1.AddUnitTO(t2));
            StringAssert.Contains(ex.Message.ToLower(),"Invalid Input");
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Subtract()
        {
            // Arrange
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act + Assert
            var ex = Assert.Throws<InvalidOperationException>(() => t1.SubtractUnitTo(t2));
            StringAssert.Contains(ex.Message.ToLower(), "subtract");
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_Divide()
        {
            // Arrange
            var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            // Act + Assert
            var ex = Assert.Throws<InvalidOperationException>(() => t1.DivideUnitTo(t2));
            StringAssert.Contains(ex.Message.ToLower(), "divide");
        }

        [TestMethod]
        public void testTemperatureUnsupportedOperation_ErrorMessage()
        {
            // Arrange
            var t1 = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(10.0, TemperatureUnit.CELSIUS);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => t1.AddUnitTO(t2));

            // Assert
            StringAssert.Contains(ex.Message.ToLower(), "temperature");
        }

        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var length = new Quantity<LengthUnit>(100.0, LengthUnit.FEET);

            // Act + Assert
            Assert.IsFalse(temperature.Equals(length));
        }

        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var weight = new Quantity<WeightUnit>(50.0, WeightUnit.KILOGRAM);

            // Act + Assert
            Assert.IsFalse(temperature.Equals(weight));
        }

        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            // Arrange
            var temperature = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.CELSIUS);
            var volume = new Quantity<VolumeUnit>(25.0, VolumeUnit.LITRE);

            // Act + Assert
            Assert.IsFalse(temperature.Equals(volume));
        }

        [TestMethod]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            // Existing units should still behave correctly
            var length1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var length2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);

            var sum = length1.AddUnitTO(length2);

            Assert.AreEqual(2.0, sum.Value, Eps);
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void testTemperatureUnit_NonLinearConversion()
        {
            // If conversion were linear-only by multiplication, 0C would not become 32F
            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            var converted = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(32.0, converted, Eps);
        }

        [TestMethod]
        public void testTemperatureUnit_AllConstants()
        {
            Assert.AreEqual("CELSIUS", TemperatureUnit.CELSIUS.ToString());
            Assert.AreEqual("FAHRENHEIT", TemperatureUnit.FAHRENHEIT.ToString());
        }

       

        [TestMethod]
        public void testTemperatureNullUnitValidation()
        {
            // For generic enum constraints, null usually cannot be passed directly.
            // This test is only meaningful if your constructor validates boxed/default invalid unit values.
            Assert.Throws<ArgumentException>(() =>
            {
                var invalidUnit = (TemperatureUnit)999;
                _ = new Quantity<TemperatureUnit>(100.0, invalidUnit);
            });
        }

        [TestMethod]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            var temp = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(temp.Equals(null));
        }

        [TestMethod]
        public void testTemperatureDifferentValuesInequality()
        {
            var t1 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            Assert.IsFalse(t1.Equals(t2));
        }

        [TestMethod]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            var length1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var length2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCH);
            var lengthSum = length1.AddUnitTO(length2);

            Assert.AreEqual(2.0, lengthSum.Value, Eps);

            var weight1 = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);
            var weight2 = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(weight1.Equals(weight2));

            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var tempF = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(32.0, tempF, Eps);
        }

        

        [TestMethod]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            var temp1 = new Quantity<TemperatureUnit>(25.000001, TemperatureUnit.CELSIUS);
            var temp2 = new Quantity<TemperatureUnit>(25.000002, TemperatureUnit.CELSIUS);

            Assert.IsFalse(temp1.Equals(temp2));
        }

        [TestMethod]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            var temp = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
            var converted = temp.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(212.0, converted, Eps);
        }
    }
}
