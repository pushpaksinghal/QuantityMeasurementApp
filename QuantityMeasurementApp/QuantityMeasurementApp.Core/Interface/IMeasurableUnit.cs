using System;

namespace QuantityMeasurementApp.Core.Interface
{
    // UC10: common contract for unit conversions (to/from base)
    public interface IMeasurableUnit
    {
        string UnitName { get; }

        // Convert value in this unit to base unit
        double ConvertToBaseUnit(double value);

        // Convert value from base unit to this unit
        double ConvertFromBaseUnit(double baseValue);
    }
}