using QuantityMeasurementApp.Core.Interface;
using System;

namespace QuantityMeasurementApp.Core.Entity
{
    internal sealed class WeightUnitAdapter : IMeasurableUnit
    {
        private readonly WeightUnit unit;

        private WeightUnitAdapter(WeightUnit unit) => this.unit = unit;

        public static WeightUnitAdapter From(WeightUnit unit) => new WeightUnitAdapter(unit);

        public string UnitName => unit.ToString();

        private double FactorToKg => unit switch
        {
            WeightUnit.KILOGRAM => 1.0,
            WeightUnit.GRAM => 0.001,
            WeightUnit.POUND => 0.45359237, // precise
            _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
        };

        public double ConvertToBaseUnit(double value) => value * FactorToKg;

        public double ConvertFromBaseUnit(double baseValue) => baseValue / FactorToKg;
    }
}