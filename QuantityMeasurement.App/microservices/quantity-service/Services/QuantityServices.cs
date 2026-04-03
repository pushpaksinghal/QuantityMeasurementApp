using QuantityService.Adapters;
using QuantityService.Models;
using Shared.Contracts;

namespace QuantityService.Services;

// ── Interfaces ────────────────────────────────────────────────────────────
public interface IConversionService
{
    Quantity<T> Convert<T>(Quantity<T> quantity, T targetUnit) where T : struct, Enum;
}

public interface IArithmeticService
{
    Quantity<T> Add<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum;
    Quantity<T> AddToTarget<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum;
    Quantity<T> Subtract<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum;
    double Divide<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum;
    bool AreEqual<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum;
}

// ── Conversion Service ────────────────────────────────────────────────────
public class ConversionService : IConversionService
{
    private readonly UnitAdapterFactory _factory;
    public ConversionService(UnitAdapterFactory factory) => _factory = factory;

    public Quantity<T> Convert<T>(Quantity<T> quantity, T targetUnit) where T : struct, Enum
    {
        var srcAdapter    = _factory.CreateAdapter(quantity.Unit);
        var targetAdapter = _factory.CreateAdapter(targetUnit);
        double baseValue  = srcAdapter.ConvertToBaseUnit(quantity.Value);
        double converted  = targetAdapter.ConvertFromBaseUnit(baseValue);
        return new Quantity<T>(converted, targetUnit);
    }
}

// ── Arithmetic Service ────────────────────────────────────────────────────
public class ArithmeticService : IArithmeticService
{
    private const double Epsilon = 1e-10;
    private readonly UnitAdapterFactory _factory;
    public ArithmeticService(UnitAdapterFactory factory) => _factory = factory;

    private double ToBase<T>(Quantity<T> q) where T : struct, Enum =>
        _factory.CreateAdapter(q.Unit).ConvertToBaseUnit(q.Value);

    private double FromBase<T>(double baseVal, T unit) where T : struct, Enum =>
        _factory.CreateAdapter(unit).ConvertFromBaseUnit(baseVal);

    public Quantity<T> Add<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum
    {
        double result = ToBase(q1) + ToBase(q2);
        return new Quantity<T>(FromBase(result, q1.Unit), q1.Unit);
    }

    public Quantity<T> AddToTarget<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum
    {
        double result = ToBase(q1) + ToBase(q2);
        return new Quantity<T>(FromBase(result, targetUnit), targetUnit);
    }

    public Quantity<T> Subtract<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum
    {
        double result = ToBase(q1) - ToBase(q2);
        return new Quantity<T>(FromBase(result, targetUnit), targetUnit);
    }

    public double Divide<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum
    {
        double b2 = ToBase(q2);
        if (Math.Abs(b2) < Epsilon) throw new DivideByZeroException("Cannot divide by zero quantity.");
        return ToBase(q1) / b2;
    }

    public bool AreEqual<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum =>
        Math.Abs(ToBase(q1) - ToBase(q2)) < 1e-6;
}

// ── Orchestrator (replaces QuantityApplicationService) ───────────────────
public class QuantityOrchestrator
{
    private readonly IConversionService _conv;
    private readonly IArithmeticService _arith;
    private readonly IHistoryClient     _history;
    private readonly ILogger<QuantityOrchestrator> _logger;

    public QuantityOrchestrator(
        IConversionService conv,
        IArithmeticService arith,
        IHistoryClient history,
        ILogger<QuantityOrchestrator> logger)
    {
        _conv    = conv;
        _arith   = arith;
        _history = history;
        _logger  = logger;
    }

    public QuantityResultDto ConvertUnit<T>(ConversionRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("Convert<{Type}>", typeof(T).Name);
        var src    = Parse<T>(req.SourceQuantity);
        var target = ParseEnum<T>(req.TargetUnit);
        var result = _conv.Convert(src, target);

        PostHistory(typeof(T), "Conversion", src, null, target.ToString(), result);

        return new QuantityResultDto(result.Value, result.Unit.ToString(),
            $"{src.Value} {src.Unit} = {result.Value:G6} {result.Unit}");
    }

    public QuantityResultDto Add<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("Add<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        var result   = _arith.Add(q1, q2);
        PostHistory(typeof(T), "Addition", q1, q2, null, result);
        return ToDto(q1, "+", q2, result);
    }

    public QuantityResultDto AddToTarget<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("AddToTarget<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        var target   = ParseEnum<T>(req.TargetUnit ?? throw new ArgumentException("TargetUnit required."));
        var result   = _arith.AddToTarget(q1, q2, target);
        PostHistory(typeof(T), "Addition", q1, q2, target.ToString(), result);
        return ToDto(q1, "+", q2, result);
    }

    public QuantityResultDto Subtract<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("Subtract<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        var result   = _arith.Subtract(q1, q2, q1.Unit);
        PostHistory(typeof(T), "Subtraction", q1, q2, null, result);
        return ToDto(q1, "-", q2, result);
    }

    public QuantityResultDto SubtractToTarget<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("SubtractToTarget<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        var target   = ParseEnum<T>(req.TargetUnit ?? throw new ArgumentException("TargetUnit required."));
        var result   = _arith.Subtract(q1, q2, target);
        PostHistory(typeof(T), "Subtraction", q1, q2, target.ToString(), result);
        return ToDto(q1, "-", q2, result);
    }

    public DivisionResultDto Divide<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("Divide<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        double value = _arith.Divide(q1, q2);
        PostHistory(typeof(T), "Division", q1, q2, null,
            new Quantity<T>(value, q1.Unit) /* dimensionless placeholder */);
        return new DivisionResultDto(value,
            $"{q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {value:F4} (dimensionless)");
    }

    public EqualityResultDto CheckEquality<T>(BinaryQuantityRequestDto req) where T : struct, Enum
    {
        _logger.LogInformation("CheckEquality<{Type}>", typeof(T).Name);
        var (q1, q2) = ParseBinary<T>(req);
        bool equal   = _arith.AreEqual(q1, q2);
        return new EqualityResultDto(equal,
            $"{q1.Value} {q1.Unit} {(equal ? "==" : "!=")} {q2.Value} {q2.Unit}");
    }

    // ── helpers ───────────────────────────────────────────────────────────
    private static Quantity<T> Parse<T>(QuantityDto dto) where T : struct, Enum =>
        new(dto.Value, ParseEnum<T>(dto.Unit));

    private static T ParseEnum<T>(string s) where T : struct, Enum =>
        Enum.TryParse<T>(s, true, out var val)
            ? val
            : throw new ArgumentException($"'{s}' is not a valid {typeof(T).Name}.");

    private static (Quantity<T>, Quantity<T>) ParseBinary<T>(BinaryQuantityRequestDto req)
        where T : struct, Enum =>
        (Parse<T>(req.FirstQuantity), Parse<T>(req.SecondQuantity));

    private static QuantityResultDto ToDto<T>(Quantity<T> q1, string op, Quantity<T> q2, Quantity<T> result)
        where T : struct, Enum =>
        new(result.Value, result.Unit.ToString(),
            $"{q1.Value} {q1.Unit} {op} {q2.Value} {q2.Unit} = {result.Value:G6} {result.Unit}");

    private void PostHistory<T>(Type enumType, string operation,
        Quantity<T> q1, Quantity<T>? q2, string? targetUnit, Quantity<T> result)
        where T : struct, Enum
    {
        string category = enumType.Name.Replace("Unit", string.Empty);
        _ = _history.AddRecordAsync(new CreateHistoryRecordDto(
            category, operation,
            q1.Value, q1.Unit.ToString(),
            q2?.Value, q2?.Unit.ToString(),
            targetUnit,
            result.Value, result.Unit.ToString()));
    }
}
