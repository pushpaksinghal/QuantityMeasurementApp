namespace QuantityService.Models;

// ── Units ───────────────────────────────────────────
public enum LengthUnit      { Millimeter, Centimeter, Meter, Kilometer, Inch, Foot, Yard, Mile }
public enum WeightUnit      { Milligram, Gram, Kilogram, Ounce, Pound }
public enum VolumeUnit      { Milliliter, Liter, CubicMeter, FluidOunce, Cup, Pint, Quart, Gallon }
public enum TemperatureUnit { Celsius, Fahrenheit, Kelvin }
public enum ArithmeticOperation { Add, Subtract, Divide }

// ── Domain Entity ───────────────────────────────────
public sealed class Quantity<U> where U : struct, Enum
{
    public double Value { get; }
    public U Unit { get; }
    public Quantity(double value, U unit) { Value = value; Unit = unit; }
    public override string ToString() => $"Quantity(value: {Value}, unit: {Unit})";
}

// ── Adapter Contract ────────────────────────────────
public interface IUnitAdapter
{
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
}
