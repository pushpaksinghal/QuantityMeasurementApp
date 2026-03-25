using QuantityMeasurementApp.ModelLayer.Enums;
using QuantityMeasurementApp.ModelLayer.Interfaces;
using QuantityMeasurementApp.BusinessLayer.EnumAdaptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Factories
{
    public class UnitAdapterFactory
    {
        public IMeasurableUnit CreateAdapter<T>(T unit) where T : struct, Enum
        {
            return unit switch
            {
                LengthUnit lengthUnit => new LengthUnitAdapter(lengthUnit),
                WeightUnit weightUnit => new WeightUnitAdapter(weightUnit),
                VolumeUnit volumeUnit => new VolumeUnitAdapter(volumeUnit),
                TemperatureUnit temperatureUnit => new TemperatureUnitAdapter(temperatureUnit),
                _ => throw new NotSupportedException($"No adapter registered for unit type: {typeof(T).Name}")
            };
        }
    }
}
