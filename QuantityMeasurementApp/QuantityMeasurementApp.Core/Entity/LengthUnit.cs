using System;

namespace QuantityMeasurementApp.Core.Entity
{
    // UC8: Standalone enum
    public enum LengthUnit
    {
        FEET,
        INCH,
        YARD,
        CENTIMETERS
    }

    // UC8: Conversion responsibility moved here
    public static class LengthUnitExtensions
    {
        // Base unit = INCHES (kept same as your original design for backward compatibility)

        private static double GetFactorToInches(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => 12.0,
                LengthUnit.INCH => 1.0,
                LengthUnit.YARD => 36.0,
                LengthUnit.CENTIMETERS => 0.393701,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
            };
        }

        // Convert current unit → base (inches)
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            Validate(value);
            return value * unit.GetFactorToInches();
        }

        // Convert base (inches) → target unit
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            Validate(baseValue);
            return baseValue / unit.GetFactorToInches();
        }

        private static void Validate(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.");
        }
    }
}