using QuantityMeasurementApp.ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class QuantityValidationService
    {
        private const double Epsilon = 1e-6;

        public void ValidateFinite(double value, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Value must be a finite number.", paramName);
        }

        public void ValidateEnumUnit<T>(T unit, string paramName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), unit))
                throw new ArgumentException($"Unit {unit} is not supported.", paramName);
        }

        public void ValidateNotNull<T>(T? obj, string paramName) where T : class
        {
            if (obj is null)
                throw new ArgumentException($"{paramName} cannot be null.", paramName);
        }

        public void ValidateSameCategory<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum
        {
            if (q1.Unit.GetType() != q2.Unit.GetType())
                throw new ArgumentException("Cannot operate on quantities of different categories.");
        }

        public bool AreEqual(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Epsilon;
        }
    }
}
