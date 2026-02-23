using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace QuantityMeasurementApp.Core.Entity
{
    //UC-3 making a generic class for all the length 
    public sealed class Length
    {
        private readonly double value;
        private readonly LengthUnit unit;

        public enum LengthUnit
        {
            FEET,
            INCH,
            YARD,
            CENTIMETERS
        }

        private static double GetConversionFactorToInches(LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => 12.0,   // 1 feet = 12 inches
                LengthUnit.INCH => 1.0,  // 1 inch = 1 inch
                LengthUnit.YARD => 36.0, // 1 yard = 36 inches
                LengthUnit.CENTIMETERS => 0.393701, // 1 centimeters = 0.393701 inches
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "This Unit is not yet supported.")
            };
        }
        public Length(double value, LengthUnit unit)
        {
            this.value = value;
            this.unit = unit;
        }

        public double Value => value;
        public LengthUnit Unit => unit;

        private double ConvertToBaseUnitInches()
        {
            return value * GetConversionFactorToInches(unit);
        }
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != GetType()) return false;

            Length other = (Length)obj;
            return ConvertToBaseUnitInches().CompareTo(other.ConvertToBaseUnitInches()) == 0;
        }
        //UC-5
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", nameof(value));

            // enums are value-types, but this validates against invalid enum values (casts)
            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit))
                throw new ArgumentException("Source unit is not supported.", nameof(sourceUnit));

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Target unit is not supported.", nameof(targetUnit));

            // Base unit = inches
            double sourceFactor = GetConversionFactorToInches(sourceUnit);
            double targetFactor = GetConversionFactorToInches(targetUnit);

            // Convert: value -> inches -> target
            double inInches = value * sourceFactor;
            double inTarget = inInches / targetFactor;

            return inTarget;
        }
        public double ConvertTo(LengthUnit targetUnit)
        {
            double convertedValue = Convert(this.value, this.unit, targetUnit);
            return convertedValue;
        }
        //UC-6
        public static Length AddUnit(Length l1,Length l2)
        {
            if (double.IsNaN(l1.Value) || double.IsInfinity(l1.Value))
                throw new ArgumentException("Value must be a finite number.", nameof(l1.Value));

            if (double.IsNaN(l2.Value) || double.IsInfinity(l2.Value))
                throw new ArgumentException("Value must be a finite number.", nameof(l2.Value));

            LengthUnit target = l1.Unit;
            LengthUnit source = l2.Unit;

            double l2lengthinl1unit = Convert(l2.Value, source, target);
            return new Length((l1.Value + l2lengthinl1unit), target);
        }
        public Length AddUnitTO(Length l2)
        {
            if (l2 is null) throw new ArgumentException("First operand cannot be null.", nameof(l2));
            Length result = AddUnit(new Length(this.Value, this.Unit), l2);
            return result;
        }

        public static Length AddToSpecificUnit(Length l1, Length l2 , LengthUnit u1)
        {
            var v1 = l1.AddUnitTO(l2);
            double result = v1.ConvertTo(u1);
            return new Length(result, u1);
        }
        public Length AddSpicificTo(Length l2, LengthUnit u1)
        {
            if (l2 is null) throw new ArgumentException("First operand cannot be null.", nameof(l2));
            Length result = AddToSpecificUnit(new Length(this.Value, this.Unit), l2,u1);
            return result;
        }
        public override int GetHashCode()
        {
            return ConvertToBaseUnitInches().GetHashCode();
        }
        public override string ToString()
        {
            return "Length(value: "+value+", unit: "+unit+")";
        }

    }
}
