using QuantityMeasurementApp.ModelLayer.Interfaces;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Text;
//UC-11 Added Volume UNit in Quantity class
namespace QuantityMeasurementApp.BusinessLayer.EnumAdaptors
{
    public class VolumeUnitAdapter : IMeasurableUnit
    {
        private readonly VolumeUnit unit;

        public VolumeUnitAdapter(VolumeUnit unit) => this.unit = unit;

        public static VolumeUnitAdapter From(VolumeUnit unit) => new (unit);

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
