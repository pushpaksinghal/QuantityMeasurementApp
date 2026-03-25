using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class QuantityEqualityComparer<T> : IEqualityComparer<Quantity<T>> where T : struct, Enum
    {
        private readonly UnitAdapterFactory _adapterFactory;
        private readonly QuantityValidationService _validator;
        private const double Epsilon = 1e-6;

        public QuantityEqualityComparer(
            UnitAdapterFactory adapterFactory,
            QuantityValidationService validator)
        {
            _adapterFactory = adapterFactory;
            _validator = validator;
        }

        public bool Equals(Quantity<T>? x, Quantity<T>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            var adapterX = _adapterFactory.CreateAdapter(x.Unit);
            var adapterY = _adapterFactory.CreateAdapter(y.Unit);

            double baseX = adapterX.ConvertToBaseUnit(x.Value);
            double baseY = adapterY.ConvertToBaseUnit(y.Value);

            return _validator.AreEqual(baseX, baseY);
        }

        public int GetHashCode(Quantity<T> obj)
        {
            var adapter = _adapterFactory.CreateAdapter(obj.Unit);
            double baseValue = adapter.ConvertToBaseUnit(obj.Value);

            // Round to handle epsilon equality
            return HashCode.Combine(Math.Round(baseValue / Epsilon));
        }
    }
}
