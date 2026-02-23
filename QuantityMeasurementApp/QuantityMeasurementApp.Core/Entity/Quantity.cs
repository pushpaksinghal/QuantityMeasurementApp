using QuantityMeasurementApp.Core.Interface;
using System;

namespace QuantityMeasurementApp.Core.Entity
{
    // UC10 + UC13: One generic Quantity for all categories with DRY arithmetic logic
    public sealed class Quantity<U> where U : struct, Enum
    {
        private readonly double value;
        private readonly U unit;

        private const double Epsilon = 1e-6;

        public Quantity(double value, U unit)
        {
            ValidateFinite(value, nameof(value));
            ValidateEnumUnit(unit, nameof(unit));

            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public U Unit => unit;

        // --------- UC10: Adapter routing (category safety) ----------
        private static IMeasurableUnit AdapterFor(U unit)
        {
            if (typeof(U) == typeof(LengthUnit))
                return LengthUnitAdapter.From((LengthUnit)(object)unit);

            if (typeof(U) == typeof(WeightUnit))
                return WeightUnitAdapter.From((WeightUnit)(object)unit);

            if (typeof(U) == typeof(VolumeUnit))
                return VolumeUnitAdapter.From((VolumeUnit)(object)unit);

            throw new NotSupportedException($"No unit adapter registered for enum type: {typeof(U).Name}");
        }

        private double ConvertToBase() => AdapterFor(unit).ConvertToBaseUnit(value);

        // --------- Equality (epsilon) ----------
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
            // For better hash stability with epsilon equality, hash rounded base value.
            double baseValue = ConvertToBase();
            return HashCode.Combine(baseValue);
        }

        public override string ToString() => $"Quantity(value: {value}, unit: {unit})";

        // --------- UC5-equivalent: Convert between units ----------
        public static double Convert(double value, U sourceUnit, U targetUnit)
        {
            ValidateFinite(value, nameof(value));
            ValidateEnumUnit(sourceUnit, nameof(sourceUnit));
            ValidateEnumUnit(targetUnit, nameof(targetUnit));

            var sourceAdapter = AdapterFor(sourceUnit);
            var targetAdapter = AdapterFor(targetUnit);

            double baseValue = sourceAdapter.ConvertToBaseUnit(value);
            return targetAdapter.ConvertFromBaseUnit(baseValue);
        }

        public double ConvertTo(U targetUnit) => Convert(this.value, this.unit, targetUnit);

        // ===================== UC13: Centralized arithmetic =====================

        private enum ArithmeticOperation
        {
            Add,
            Subtract,
            Divide
        }

        private static void ValidateFinite(double v, string paramName)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
                throw new ArgumentException("Value must be a finite number.", paramName);
        }

        private static void ValidateEnumUnit(U u, string paramName)
        {
            if (!Enum.IsDefined(typeof(U), u))
                throw new ArgumentException("Unit is not supported.", paramName);
        }

        private static void ValidateOperandNotNull(Quantity<U>? other, string paramName)
        {
            if (other is null)
                throw new ArgumentException("Second operand cannot be null.", paramName);
        }


        private static void ValidateArithmeticOperands(
            Quantity<U> left,
            Quantity<U>? right,
            U? targetUnit,
            bool targetUnitRequired)
        {
            if (left is null)
                throw new ArgumentException("First operand cannot be null.", nameof(left));

            ValidateOperandNotNull(right, nameof(right));

            // These are already validated in ctor, but keep consistency for future changes.
            ValidateFinite(left.value, nameof(left.value));
            ValidateFinite(right!.value, nameof(right.value));

            ValidateEnumUnit(left.unit, nameof(left.unit));
            ValidateEnumUnit(right.unit, nameof(right.unit));

            if (targetUnitRequired)
            {
                if (!targetUnit.HasValue)
                    throw new ArgumentException("Target unit cannot be null.", nameof(targetUnit));

                ValidateEnumUnit(targetUnit.Value, nameof(targetUnit));
            }

            // Same category check (generic U already enforces it, but keep for clarity)
            if (left.unit.GetType() != right.unit.GetType())
                throw new ArgumentException("Cannot operate on quantities of different categories.");
        }

        private static double PerformBaseArithmetic(Quantity<U> left, Quantity<U> right, ArithmeticOperation op)
        {
            var leftAdapter = AdapterFor(left.unit);
            var rightAdapter = AdapterFor(right.unit);

            double aBase = leftAdapter.ConvertToBaseUnit(left.value);
            double bBase = rightAdapter.ConvertToBaseUnit(right.value);

            return op switch
            {
                ArithmeticOperation.Add => aBase + bBase,
                ArithmeticOperation.Subtract => aBase - bBase,
                ArithmeticOperation.Divide => bBase == 0.0
                    ? throw new DivideByZeroException("Cannot divide by zero quantity.")
                    : aBase / bBase,
                _ => throw new ArgumentOutOfRangeException(nameof(op), "Unsupported arithmetic operation.")
            };
        }

        private static Quantity<U> FromBaseToTarget(double baseResult, U targetUnit)
        {
            var targetAdapter = AdapterFor(targetUnit);
            double valueInTarget = targetAdapter.ConvertFromBaseUnit(baseResult);
            return new Quantity<U>(valueInTarget, targetUnit);
        }

        // --------- Add (UC6 / UC7) ----------
        public static Quantity<U> AddUnit(Quantity<U> q1, Quantity<U> q2)
        {
            ValidateArithmeticOperands(q1, q2, targetUnit: null, targetUnitRequired: false);

            double baseResult = PerformBaseArithmetic(q1, q2, ArithmeticOperation.Add);
            return FromBaseToTarget(baseResult, q1.unit);
        }

        public Quantity<U> AddUnitTO(Quantity<U> other)
            => AddUnit(this, other);

        public static Quantity<U> AddToSpecificUnit(Quantity<U> q1, Quantity<U> q2, U targetUnit)
        {
            ValidateArithmeticOperands(q1, q2, targetUnit, targetUnitRequired: true);

            double baseResult = PerformBaseArithmetic(q1, q2, ArithmeticOperation.Add);
            return FromBaseToTarget(baseResult, targetUnit);
        }

        public Quantity<U> AddSpicificTo(Quantity<U> other, U targetUnit)
            => AddToSpecificUnit(this, other, targetUnit);

        // --------- Subtract (UC12) ----------
        public static Quantity<U> SubtractUnit(Quantity<U> q1, Quantity<U> q2, U targetUnit)
        {
            ValidateArithmeticOperands(q1, q2, targetUnit, targetUnitRequired: true);

            double baseResult = PerformBaseArithmetic(q1, q2, ArithmeticOperation.Subtract);
            return FromBaseToTarget(baseResult, targetUnit);
        }

        public Quantity<U> SubtractUnitTo(Quantity<U> other, U targetUnit)
            => SubtractUnit(this, other, targetUnit);

        public Quantity<U> SubtractUnitTo(Quantity<U> other)
            => SubtractUnit(this, other, this.unit);

        // --------- Divide (UC12) ----------
        // Division returns a dimensionless scalar (double)
        public static double DivideUnit(Quantity<U> q1, Quantity<U> q2)
        {
            ValidateArithmeticOperands(q1, q2, targetUnit: null, targetUnitRequired: false);
            return PerformBaseArithmetic(q1, q2, ArithmeticOperation.Divide);
        }

        public double DivideUnitTo(Quantity<U> other)
            => DivideUnit(this, other);
    }
}