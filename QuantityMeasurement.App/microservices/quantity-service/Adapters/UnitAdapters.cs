using QuantityService.Models;

namespace QuantityService.Adapters;

// ── Length ────────────────────────────────────────
// Base unit: Meter
public class LengthUnitAdapter : IUnitAdapter
{
    private readonly LengthUnit _unit;
    private static readonly Dictionary<LengthUnit, double> ToMeter = new()
    {
        [LengthUnit.Millimeter] = 0.001,
        [LengthUnit.Centimeter] = 0.01,
        [LengthUnit.Meter]      = 1.0,
        [LengthUnit.Kilometer]  = 1000.0,
        [LengthUnit.Inch]       = 0.0254,
        [LengthUnit.Foot]       = 0.3048,
        [LengthUnit.Yard]       = 0.9144,
        [LengthUnit.Mile]       = 1609.344
    };
    public LengthUnitAdapter(LengthUnit unit) => _unit = unit;
    public double ConvertToBaseUnit(double v)   => v * ToMeter[_unit];
    public double ConvertFromBaseUnit(double v) => v / ToMeter[_unit];
}

// ── Weight ────────────────────────────────────────
// Base unit: Kilogram
public class WeightUnitAdapter : IUnitAdapter
{
    private readonly WeightUnit _unit;
    private static readonly Dictionary<WeightUnit, double> ToKg = new()
    {
        [WeightUnit.Milligram] = 0.000001,
        [WeightUnit.Gram]      = 0.001,
        [WeightUnit.Kilogram]  = 1.0,
        [WeightUnit.Ounce]     = 0.0283495,
        [WeightUnit.Pound]     = 0.453592
    };
    public WeightUnitAdapter(WeightUnit unit) => _unit = unit;
    public double ConvertToBaseUnit(double v)   => v * ToKg[_unit];
    public double ConvertFromBaseUnit(double v) => v / ToKg[_unit];
}

// ── Volume ────────────────────────────────────────
// Base unit: Liter
public class VolumeUnitAdapter : IUnitAdapter
{
    private readonly VolumeUnit _unit;
    private static readonly Dictionary<VolumeUnit, double> ToLiter = new()
    {
        [VolumeUnit.Milliliter]  = 0.001,
        [VolumeUnit.Liter]       = 1.0,
        [VolumeUnit.CubicMeter]  = 1000.0,
        [VolumeUnit.FluidOunce]  = 0.0295735,
        [VolumeUnit.Cup]         = 0.236588,
        [VolumeUnit.Pint]        = 0.473176,
        [VolumeUnit.Quart]       = 0.946353,
        [VolumeUnit.Gallon]      = 3.78541
    };
    public VolumeUnitAdapter(VolumeUnit unit) => _unit = unit;
    public double ConvertToBaseUnit(double v)   => v * ToLiter[_unit];
    public double ConvertFromBaseUnit(double v) => v / ToLiter[_unit];
}

// ── Temperature ───────────────────────────────────
// Base unit: Celsius (special non-linear conversions)
public class TemperatureUnitAdapter : IUnitAdapter
{
    private readonly TemperatureUnit _unit;
    public TemperatureUnitAdapter(TemperatureUnit unit) => _unit = unit;

    public double ConvertToBaseUnit(double v) => _unit switch
    {
        TemperatureUnit.Celsius    => v,
        TemperatureUnit.Fahrenheit => (v - 32) * 5.0 / 9.0,
        TemperatureUnit.Kelvin     => v - 273.15,
        _ => throw new ArgumentOutOfRangeException()
    };

    public double ConvertFromBaseUnit(double v) => _unit switch
    {
        TemperatureUnit.Celsius    => v,
        TemperatureUnit.Fahrenheit => v * 9.0 / 5.0 + 32,
        TemperatureUnit.Kelvin     => v + 273.15,
        _ => throw new ArgumentOutOfRangeException()
    };
}

// ── Factory ───────────────────────────────────────
public class UnitAdapterFactory
{
    public IUnitAdapter CreateAdapter<T>(T unit) where T : struct, Enum
    {
        return unit switch
        {
            LengthUnit      lu => new LengthUnitAdapter(lu),
            WeightUnit      wu => new WeightUnitAdapter(wu),
            VolumeUnit      vu => new VolumeUnitAdapter(vu),
            TemperatureUnit tu => new TemperatureUnitAdapter(tu),
            _ => throw new ArgumentException($"Unknown unit type: {typeof(T).Name}")
        };
    }
}
