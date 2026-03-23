using System;

namespace QuantityMeasurementApp.ModelLayer.Interfaces
{
    // UC10: common contract for unit conversions (to/from base)
    public interface IMeasurableUnit
    {
        string UnitName { get; }

        // Convert value in this unit to base unit
        double ConvertToBaseUnit(double value);

        // Convert value from base unit to this unit
        double ConvertFromBaseUnit(double baseValue);
        //Arithemetic Support option
        bool SupportsArithmetic() => true;

        void ValidateOperationSupport(string operation)
        {
            if (!SupportsArithmetic())
                throw new NotSupportedException($"{UnitName} does not support {operation} operation.");
        }
    }
}