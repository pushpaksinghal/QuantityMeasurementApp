using System;

namespace QuantityMeasurementApp.Core.Entity
{
    // UC9: Weight quantity class mirroring your Length class style
    public sealed class Weight
    {
        private const double Epsilon = 1e-6;
        private readonly double value;
        private readonly WeightUnit unit;

        public Weight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(WeightUnit), unit))
                throw new ArgumentException("Unit is not supported.", nameof(unit));

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public WeightUnit Unit => unit;

        private double ConvertToBaseUnitKilogram()
        {
            return unit.ConvertToBaseUnit(value);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != GetType()) return false;

            Weight other = (Weight)obj;
            double a = ConvertToBaseUnitKilogram();
            double b = other.ConvertToBaseUnitKilogram();

            return Math.Abs(a - b) <= Epsilon;
        }

        public override int GetHashCode()
        {
            double baseKg = ConvertToBaseUnitKilogram();
            double stable = Math.Round(baseKg, 6);
            return stable.GetHashCode();
        }

        public static double Convert(double value, WeightUnit sourceUnit, WeightUnit targetUnit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            if (!Enum.IsDefined(typeof(WeightUnit), sourceUnit))
                throw new ArgumentException("Source unit is not supported.", nameof(sourceUnit));

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            double baseKg = sourceUnit.ConvertToBaseUnit(value);
            double inTarget = targetUnit.ConvertFromBaseUnit(baseKg);
            return inTarget;
        }

        public double ConvertTo(WeightUnit targetUnit)
        {
            return Convert(this.value, this.unit, targetUnit);
        }

        // Default addition: result in first operand's unit 
        public static Weight AddUnit(Weight w1, Weight w2)
        {
            if (w1 is null) throw new ArgumentException("First operand cannot be null.", nameof(w1));
            if (w2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(w2));

            double baseSumKg =
                w1.Unit.ConvertToBaseUnit(w1.Value) +
                w2.Unit.ConvertToBaseUnit(w2.Value);

            double sumInW1Unit = w1.Unit.ConvertFromBaseUnit(baseSumKg);
            return new Weight(sumInW1Unit, w1.Unit);
        }

        public Weight AddUnitTO(Weight w2)
        {
            if (w2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(w2));
            return AddUnit(this, w2);
        }

        // Addition with explicit target unit
        public static Weight AddToSpecificUnit(Weight w1, Weight w2, WeightUnit targetUnit)
        {
            if (w1 is null) throw new ArgumentException("First operand cannot be null.", nameof(w1));
            if (w2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(w2));

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            double baseSumKg =
                w1.Unit.ConvertToBaseUnit(w1.Value) +
                w2.Unit.ConvertToBaseUnit(w2.Value);

            double sumInTarget = targetUnit.ConvertFromBaseUnit(baseSumKg);
            return new Weight(sumInTarget, targetUnit);
        }

        public Weight AddSpicificTo(Weight w2, WeightUnit targetUnit)
        {
            if (w2 is null) throw new ArgumentException("Second operand cannot be null.", nameof(w2));
            return AddToSpecificUnit(this, w2, targetUnit);
        }

        public override string ToString()
        {
            return "Weight(value: " + value + ", unit: " + unit + ")";
        }
    }
}