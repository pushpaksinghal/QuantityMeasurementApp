using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace QuantityMeasurementApp.Core.Entity
{
    //UC-1
    public sealed class Feet
    {
        private readonly double value;

        public Feet(double value)
        {
            this.value = value;
        }

        public double GetFeet()
        {
            return value;
        }
        // overriding the equals method to check for all the edge cases 
        public override bool Equals(object? obj)
        {
            if(this == obj) return true;
            if (obj == null || this.GetType() != obj.GetType()) return false;
            Feet other = (Feet)obj;
            return value.CompareTo(other.value) == 0;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
