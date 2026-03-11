using QuantityMeasurementApp.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.Core.Entity
{
    internal sealed class TemperatureUnitAdapter : IMeasurableUnit
    {
        private readonly TemperatureUnit unit;

        private TemperatureUnitAdapter(TemperatureUnit unit)
        {
            this.unit = unit;
        }

        public static TemperatureUnitAdapter From(TemperatureUnit unit) => new TemperatureUnitAdapter(unit);

        public string UnitName => unit.ToString();

        // Base unit = CELSIUS
        public double ConvertToBaseUnit(double value)
        {
            return unit switch
            {
                TemperatureUnit.CELSIUS => value,
                TemperatureUnit.FAHRENHEIT => (value - 32.0) * 5.0 / 9.0,
                TemperatureUnit.KELVIN => value - 273.15,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
            };
        }

        public double ConvertFromBaseUnit(double baseValue)
        {
            return unit switch
            {
                TemperatureUnit.CELSIUS => baseValue,
                TemperatureUnit.FAHRENHEIT => (baseValue * 9.0 / 5.0) + 32.0,
                TemperatureUnit.KELVIN => baseValue + 273.15,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
            };
        }

        // temperature does not support arithmetic
        public bool SupportsArithmetic() => false;

        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException(
                $"Temperature does not support {operation} operation in this application.");
        }
    }
}
