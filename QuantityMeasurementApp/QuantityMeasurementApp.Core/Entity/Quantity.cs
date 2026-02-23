using QuantityMeasurementApp.Core.Interface;
using System;

namespace QuantityMeasurementApp.Core.Entity
{
    // UC10: One generic Quantity for all categories
    public sealed class Quantity<U> where U : struct, Enum
    {
        private readonly double value;
        private readonly U unit;

        public Quantity(double value, U unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(U), unit))
                throw new ArgumentException("Unit is not supported.", nameof(unit));

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public U Unit => unit;

        private static IMeasurableUnit AdapterFor(U unit)
        {
            // UC10: route to correct adapter (category safety)
            if (typeof(U) == typeof(LengthUnit))
                return LengthUnitAdapter.From((LengthUnit)(object)unit);

            if (typeof(U) == typeof(WeightUnit))
                return WeightUnitAdapter.From((WeightUnit)(object)unit);

            if (typeof(U) == typeof(VolumeUnit))
                return VolumeUnitAdapter.From((VolumeUnit)(object)unit);// added route to volume unit

            throw new NotSupportedException($"No unit adapter registered for enum type: {typeof(U).Name}");
        }

        private double ConvertToBase()
        {
            return AdapterFor(unit).ConvertToBaseUnit(value);
        }

        private const double Epsilon = 1e-6;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != GetType()) return false;

            var other = (Quantity<U>)obj;

            double a = this.ConvertToBase();
            double b = other.ConvertToBase();

            return Math.Abs(a - b) <= Epsilon;
        }

        public override int GetHashCode()
        {
            return ConvertToBase().GetHashCode();
        }

        // UC5-equivalent: Convert between units (value -> base -> target)
        public static double Convert(double value, U sourceUnit, U targetUnit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(U), sourceUnit))
                throw new ArgumentException("Source unit is not supported.", nameof(sourceUnit));

            if (!Enum.IsDefined(typeof(U), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            var sourceAdapter = AdapterFor(sourceUnit);
            var targetAdapter = AdapterFor(targetUnit);

            double baseValue = sourceAdapter.ConvertToBaseUnit(value);
            return targetAdapter.ConvertFromBaseUnit(baseValue);
        }

        public double ConvertTo(U targetUnit)
        {
            return Convert(this.value, this.unit, targetUnit);
        }

        // UC6-equivalent: Add and return in first operand unit
        public static Quantity<U> AddUnit(Quantity<U> q1, Quantity<U> q2)
        {
            if (q1 is null) throw new ArgumentException("First operand cannot be null.", nameof(q1));
            if (q2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(q2));

            var unit1Adapter = AdapterFor(q1.unit);
            var unit2Adapter = AdapterFor(q2.unit);

            double baseSum = unit1Adapter.ConvertToBaseUnit(q1.value) + unit2Adapter.ConvertToBaseUnit(q2.value);
            double sumInUnit1 = unit1Adapter.ConvertFromBaseUnit(baseSum);

            return new Quantity<U>(sumInUnit1, q1.unit);
        }

        public Quantity<U> AddUnitTO(Quantity<U> other)
        {
            if (other is null) throw new ArgumentException("Second operand cannot be null.", nameof(other));
            return AddUnit(this, other);
        }

        // UC7-equivalent: Add and return in explicit target unit
        public static Quantity<U> AddToSpecificUnit(Quantity<U> q1, Quantity<U> q2, U targetUnit)
        {
            if (q1 is null) throw new ArgumentException("First operand cannot be null.", nameof(q1));
            if (q2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(q2));

            if (!Enum.IsDefined(typeof(U), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            var unit1Adapter = AdapterFor(q1.unit);
            var unit2Adapter = AdapterFor(q2.unit);
            var targetAdapter = AdapterFor(targetUnit);

            double baseSum = unit1Adapter.ConvertToBaseUnit(q1.value) + unit2Adapter.ConvertToBaseUnit(q2.value);
            double sumInTarget = targetAdapter.ConvertFromBaseUnit(baseSum);

            return new Quantity<U>(sumInTarget, targetUnit);
        }

        public Quantity<U> AddSpicificTo(Quantity<U> other, U targetUnit)
        {
            if (other is null) throw new ArgumentException("Second operand cannot be null.", nameof(other));
            return AddToSpecificUnit(this, other, targetUnit);
        }

        public override string ToString()
        {
            return $"Quantity(value: {value}, unit: {unit})";
        }
    }
}