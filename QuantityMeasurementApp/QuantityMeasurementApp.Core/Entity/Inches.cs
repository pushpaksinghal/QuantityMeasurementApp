using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.Core.Entity
{
    //UC-2
    public sealed class Inches
    {
        private readonly double value;

        public Inches(double value)
        {
            this.value = value;
        }

        public double GetInches()
        {
            return value;
        }
        // overriding the equals method to check for all the edge cases 
        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || this.GetType() != obj.GetType()) return false;
            Inches other = (Inches)obj;
            return value.CompareTo(other.value) == 0;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
