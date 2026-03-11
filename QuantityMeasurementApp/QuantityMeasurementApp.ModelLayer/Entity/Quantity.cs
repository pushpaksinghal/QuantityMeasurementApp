using QuantityMeasurementApp.ModelLayer.Interfaces;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.ModelLayer.Entity
{
    // UC10 + UC13: One generic Quantity for all categories with DRY arithmetic logic
    public sealed class Quantity<U> where U : struct, Enum
    {
        private readonly double value;
        private readonly U unit;

        private const double Epsilon = 1e-6;

        public Quantity(double value, U unit)
        {

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public U Unit => unit;
        public override string ToString() => $"Quantity(value: {value}, unit: {unit})";
    }

}