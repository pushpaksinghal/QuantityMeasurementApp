using QuantityMeasurementApp.Core.Interface;
using System;

namespace QuantityMeasurementApp.Core.Entity
{
    internal sealed class LengthUnitAdapter : IMeasurableUnit
    {
        private readonly LengthUnit unit;

        private LengthUnitAdapter(LengthUnit unit) => this.unit = unit;

        public static LengthUnitAdapter From(LengthUnit unit) => new LengthUnitAdapter(unit);

        public string UnitName => unit.ToString();

        private double FactorToInches => unit switch
        {
            LengthUnit.FEET => 12.0,
            LengthUnit.INCH => 1.0,
            LengthUnit.YARD => 36.0,
            LengthUnit.CENTIMETERS => 0.393701,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
        };

        public double ConvertToBaseUnit(double value) => value * FactorToInches;

        public double ConvertFromBaseUnit(double baseValue) => baseValue / FactorToInches;
    }
}