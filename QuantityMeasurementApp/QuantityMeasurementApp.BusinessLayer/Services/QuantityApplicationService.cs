using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.BusinessLayer.Mappers;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;

public class QuantityApplicationService : IQuantityApplicationService
{
    private readonly IQuantityConversionService _conversionService;
    private readonly IQuantityArithmeticService _arithmeticService;
    private readonly IQuantityHistoryRepository _historyRepository;
    private readonly RedisCacheService _cacheService;
    private readonly ILogger<QuantityApplicationService> _logger;

    public QuantityApplicationService(
        IQuantityConversionService conversionService,
        IQuantityArithmeticService arithmeticService,
        IQuantityHistoryRepository historyRepository,
        RedisCacheService cacheService,
        ILogger<QuantityApplicationService> logger)
    {
        _conversionService = conversionService;
        _arithmeticService = arithmeticService;
        _historyRepository = historyRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    // -------------------- EQUALITY --------------------

    public EqualityResultDto CheckEquality<T>(
        BinaryQuantityRequestDto request,
        QuantityEqualityComparer<T> equalityComparer) where T : struct, Enum
    {
        _logger.LogInformation("Checking equality");

        if (request is null)
        {
            _logger.LogError("Equality request is null");
            throw new ArgumentNullException(nameof(request));
        }

        var q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
        var q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

        bool areEqual = equalityComparer.Equals(q1, q2);

        _logger.LogInformation("Equality result: {Result}", areEqual);

        return new EqualityResultDto
        {
            AreEqual = areEqual,
            Message = $"{q1.Value} {q1.Unit} {(areEqual ? "==" : "!=")} {q2.Value} {q2.Unit}"
        };
    }

    // -------------------- CONVERT --------------------

    public QuantityResultDto ConvertUnit<T>(ConversionRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("Convert operation started");

        if (request is null)
        {
            _logger.LogError("Convert request is null");
            throw new ArgumentNullException(nameof(request));
        }

        var source = QuantityDtoMapper.ToEntity<T>(request.SourceQuantity);
        var targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);

        var result = _conversionService.ConvertTo(source, targetUnit);

        _logger.LogInformation("Conversion completed: {Value} {Unit} -> {Result}",
            source.Value, source.Unit, result.Value);

        SaveHistory(source, null, targetUnit.ToString(), result, "Conversion");

        return QuantityDtoMapper.ToResultDto(
            result,
            $"{source.Value} {source.Unit} = {result.Value} {result.Unit}");
    }

    // -------------------- ADD --------------------

    public QuantityResultDto AddUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("Add operation started");

        var (q1, q2) = ValidateBinaryRequest<T>(request);

        var result = _arithmeticService.AddUnit(q1, q2);

        _logger.LogInformation("Addition result: {Result}", result.Value);

        SaveHistory(q1, q2, null, result, "Addition");

        return QuantityDtoMapper.ToResultDto(
            result,
            $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
    }

    public QuantityResultDto AddUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("AddToTarget operation started");

        var (q1, q2) = ValidateBinaryRequest<T>(request);

        if (string.IsNullOrWhiteSpace(request.TargetUnit))
        {
            _logger.LogError("Target unit missing in AddToTarget");
            throw new ArgumentException("Target unit is required.");
        }

        var targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);
        var result = _arithmeticService.AddToSpecificUnit(q1, q2, targetUnit);

        SaveHistory(q1, q2, targetUnit.ToString(), result, "Addition");

        return QuantityDtoMapper.ToResultDto(
            result,
            $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
    }

    // -------------------- SUBTRACT --------------------

    public QuantityResultDto SubtractUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("Subtract operation started");

        var (q1, q2) = ValidateBinaryRequest<T>(request);

        var result = _arithmeticService.SubtractUnit(q1, q2, q1.Unit);

        SaveHistory(q1, q2, null, result, "Subtraction");

        return QuantityDtoMapper.ToResultDto(
            result,
            $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
    }

    public QuantityResultDto SubtractUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("SubtractToTarget operation started");

        var (q1, q2) = ValidateBinaryRequest<T>(request);

        if (string.IsNullOrWhiteSpace(request.TargetUnit))
        {
            _logger.LogError("Target unit missing in SubtractToTarget");
            throw new ArgumentException("Target unit is required.");
        }

        var targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);
        var result = _arithmeticService.SubtractUnit(q1, q2, targetUnit);

        SaveHistory(q1, q2, targetUnit.ToString(), result, "Subtraction");

        return QuantityDtoMapper.ToResultDto(
            result,
            $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
    }

    // -------------------- DIVIDE --------------------

    public DivisionResultDto DivideUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
    {
        _logger.LogInformation("Divide operation started");

        var (q1, q2) = ValidateBinaryRequest<T>(request);

        var result = _arithmeticService.DivideUnit(q1, q2);

        SaveHistory(q1, q2, null, result, "Divide");

        _logger.LogInformation("Divide result: {Value}", result);

        return new DivisionResultDto
        {
            Value = result,
            Message = $"{q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {result:F4}"
        };
    }

    // -------------------- HISTORY --------------------

    public async Task<List<QuantityHistoryRecord>> GetAllRecordsAsync()
    {
        string cacheKey = "History:all";
        _logger.LogInformation("Checking cache for {Key}", cacheKey);

        var cached = await _cacheService.GetAsync<List<QuantityHistoryRecord>>(cacheKey);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit");
            return cached;
        }

        _logger.LogInformation("Cache miss → DB call");

        var records = _historyRepository.GetAllRecords();

        await _cacheService.SetAsync(cacheKey, records, 15, 5);

        return records;
    }

    public async Task<QuantityHistoryRecord?> GetRecordByIdAsync(int id)
    {
        string cacheKey = $"History:{id}";
        _logger.LogInformation("Checking cache for {Key}", cacheKey);

        var cached = await _cacheService.GetAsync<QuantityHistoryRecord>(cacheKey);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit");
            return cached;
        }

        var record = _historyRepository.GetRecordById(id);

        if (record != null)
        {
            await _cacheService.SetAsync(cacheKey, record, 15, 5);
        }
        else
        {
            _logger.LogWarning("Record not found: {Id}", id);
        }

        return record;
    }

    public async Task AddRecordAsync(QuantityHistoryRecord record)
    {
        _logger.LogInformation("Adding history record");

        _historyRepository.AddRecord(record);

        await _cacheService.RemoveAsync("History:all");
    }

    public async Task<bool> DeleteRecordAsync(int id)
    {
        _logger.LogInformation("Deleting record {Id}", id);

        bool deleted = _historyRepository.DeleteRecord(id);

        if (deleted)
        {
            await _cacheService.RemoveAsync($"History:{id}");
            await _cacheService.RemoveAsync("History:all");
        }
        else
        {
            _logger.LogWarning("Delete failed for {Id}", id);
        }

        return deleted;
    }

    // -------------------- HELPERS --------------------

    private (Quantity<T>, Quantity<T>) ValidateBinaryRequest<T>(BinaryQuantityRequestDto request)
        where T : struct, Enum
    {
        if (request is null)
        {
            _logger.LogError("Request is null");
            throw new ArgumentNullException(nameof(request));
        }

        var q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
        var q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

        return (q1, q2);
    }

    private void SaveHistory<T>(
        Quantity<T> q1,
        Quantity<T>? q2,
        string? targetUnit,
        dynamic result,
        string operation) where T : struct, Enum
    {
        _historyRepository.AddRecord(new QuantityHistoryRecord
        {
            Category = typeof(T).Name.Replace("Unit", ""),
            OperationType = operation,
            FirstValue = q1.Value,
            FirstUnit = q1.Unit.ToString(),
            SecondValue = q2?.Value,
            SecondUnit = q2?.Unit.ToString(),
            TargetUnit = targetUnit,
            ResultValue = result.Value,
            ResultUnit = result.Unit?.ToString() ?? "Dimensionless"
        });

        _logger.LogInformation("History saved for operation {Operation}", operation);
    }
}