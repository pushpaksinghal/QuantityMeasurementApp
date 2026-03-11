using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public interface IQuantityConversionService
    {
        double Convert<T>(double value, T sourceUnit, T targetUnit) where T : struct, Enum;
        Quantity<T> ConvertTo<T>(Quantity<T> quantity, T targetUnit) where T : struct, Enum;
    }

    public class QuantityConversionService : IQuantityConversionService
    {
        private readonly UnitAdapterFactory _adapterFactory;
        private readonly QuantityValidationService _validator;

        public QuantityConversionService(
            UnitAdapterFactory adapterFactory,
            QuantityValidationService validator)
        {
            _adapterFactory = adapterFactory;
            _validator = validator;
        }

        public double Convert<T>(double value, T sourceUnit, T targetUnit) where T : struct, Enum
        {
            _validator.ValidateFinite(value, nameof(value));
            _validator.ValidateEnumUnit(sourceUnit, nameof(sourceUnit));
            _validator.ValidateEnumUnit(targetUnit, nameof(targetUnit));

            var sourceAdapter = _adapterFactory.CreateAdapter(sourceUnit);
            var targetAdapter = _adapterFactory.CreateAdapter(targetUnit);

            double baseValue = sourceAdapter.ConvertToBaseUnit(value);
            return targetAdapter.ConvertFromBaseUnit(baseValue);
        }

        public Quantity<T> ConvertTo<T>(Quantity<T> quantity, T targetUnit) where T : struct, Enum
        {
            _validator.ValidateNotNull(quantity, nameof(quantity));

            double convertedValue = Convert(quantity.Value, quantity.Unit, targetUnit);
            return new Quantity<T>(convertedValue, targetUnit);
        }
    }
}
