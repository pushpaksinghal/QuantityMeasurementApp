using System;

namespace QuantityMeasurementApp.Core.Entity
{
    public sealed class Length
    {
        private readonly double value;
        private readonly LengthUnit unit;

        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Unit is not supported.", nameof(unit));

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public LengthUnit Unit => unit;

        // Base conversion now delegated
        private double ConvertToBaseUnit()
        {
            return unit.ConvertToBaseUnit(value);
        }

        // UC3 Equality
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != GetType()) return false;

            Length other = (Length)obj;
            return ConvertToBaseUnit().CompareTo(other.ConvertToBaseUnit()) == 0;
        }

        public override int GetHashCode()
        {
            return ConvertToBaseUnit().GetHashCode();
        }

        // UC5 Conversion
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit))
                throw new ArgumentException("Source unit is not supported.", nameof(sourceUnit));

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            double baseValue = sourceUnit.ConvertToBaseUnit(value);
            return targetUnit.ConvertFromBaseUnit(baseValue);
        }

        public double ConvertTo(LengthUnit targetUnit)
        {
            return Convert(this.value, this.unit, targetUnit);
        }

        // UC6 Add (default: result in first unit)
        public static Length AddUnit(Length l1, Length l2)
        {
            if (l1 is null) throw new ArgumentException("First operand cannot be null.", nameof(l1));
            if (l2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(l2));

            double baseSum =
                l1.Unit.ConvertToBaseUnit(l1.Value) +
                l2.Unit.ConvertToBaseUnit(l2.Value);

            double result = l1.Unit.ConvertFromBaseUnit(baseSum);

            return new Length(result, l1.Unit);
        }

        public Length AddUnitTO(Length l2)
        {
            return AddUnit(this, l2);
        }

        // UC7 Add with target unit
        public static Length AddToSpecificUnit(Length l1, Length l2, LengthUnit targetUnit)
        {
            if (l1 is null) throw new ArgumentException("First operand cannot be null.", nameof(l1));
            if (l2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(l2));

            double baseSum =
                l1.Unit.ConvertToBaseUnit(l1.Value) +
                l2.Unit.ConvertToBaseUnit(l2.Value);

            double result = targetUnit.ConvertFromBaseUnit(baseSum);

            return new Length(result, targetUnit);
        }

        public Length AddSpicificTo(Length l2, LengthUnit targetUnit)
        {
            return AddToSpecificUnit(this, l2, targetUnit);
        }

        public override string ToString()
        {
            return $"Length(value: {value}, unit: {unit})";
        }
    }
}