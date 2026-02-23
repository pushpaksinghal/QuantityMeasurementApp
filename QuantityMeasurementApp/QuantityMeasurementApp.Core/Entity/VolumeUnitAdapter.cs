using QuantityMeasurementApp.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;
//UC-11 Added Volume UNit in Quantity class
namespace QuantityMeasurementApp.Core.Entity
{
    internal sealed class VolumeUnitAdapter : IMeasurableUnit
    {
        private readonly VolumeUnit unit;

        private VolumeUnitAdapter(VolumeUnit unit) => this.unit = unit;

        public static VolumeUnitAdapter From(VolumeUnit unit) => new VolumeUnitAdapter(unit);

        public string UnitName => unit.ToString();

        private double FactorTolitre => unit switch
        {
            VolumeUnit.LITRE => 1.0,
            VolumeUnit.MILLILITRE => 0.001,
            VolumeUnit.GALLON => 3.78541, 
            _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not supported.")
        };

        public double ConvertToBaseUnit(double value) => value * FactorTolitre;

        public double ConvertFromBaseUnit(double baseValue) => baseValue / FactorTolitre;
    }
}
