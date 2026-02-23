using System;

namespace QuantityMeasurementApp.Core.Entity
{
    // UC9: Standalone enum for Weight units
    public enum WeightUnit
    {
        KILOGRAM,
        GRAM,
        POUND
    }

    // UC9: Conversion responsibility lives with the unit (base unit = KILOGRAM)
    public static class WeightUnitExtensions
    {
        
        private static double GetFactorToKilogram(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.KILOGRAM => 1.0,
                WeightUnit.GRAM => 0.001,          // 1 g = 0.001 kg
                WeightUnit.POUND => 0.45359237,      // 1 lb = 0.453592 kg
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not yet supported.")
            };
        }

        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            ValidateFinite(value, nameof(value));
            return value * unit.GetFactorToKilogram();
        }

        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValueInKilogram)
        {
            ValidateFinite(baseValueInKilogram, nameof(baseValueInKilogram));
            return baseValueInKilogram / unit.GetFactorToKilogram();
        }

        private static void ValidateFinite(double value, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", paramName);
        }
    }
}