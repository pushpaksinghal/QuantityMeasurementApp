using QuantityMeasurementApp.ModelLayer.Interfaces;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.BusinessLayer.EnumAdaptors
{
   public class WeightUnitAdapter : IMeasurableUnit
    {
        private readonly WeightUnit unit;

        public WeightUnitAdapter(WeightUnit unit) => this.unit = unit;

        public static WeightUnitAdapter From(WeightUnit unit) => new(unit);

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