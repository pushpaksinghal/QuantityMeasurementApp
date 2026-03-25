using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.BusinessLayer.Mappers;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using QuantityMeasurementApp.RepositoryLayer.Utility;
using QuantityMeasurementApp.BusinessLayer.Interface;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class QuantityApplicationService : IQuantityApplicationService
    {
        private const string HistoryAllCacheKey = "History:all";

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

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            bool areEqual = equalityComparer.Equals(q1, q2);

            _logger.LogInformation("Equality result: {Result}", areEqual);

            return new EqualityResultDto
            {
                AreEqual = areEqual,
                Message = $"{q1.Value} {q1.Unit} {(areEqual ? "==" : "!=")} {q2.Value} {q2.Unit}"
            };
        }

        public QuantityResultDto ConvertUnit<T>(ConversionRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("Convert operation started");

            if (request is null)
            {
                _logger.LogError("Convert request is null");
                throw new ArgumentNullException(nameof(request));
            }

            Quantity<T> source = QuantityDtoMapper.ToEntity<T>(request.SourceQuantity);
            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);

            Quantity<T> result = _conversionService.ConvertTo(source, targetUnit);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Conversion",
                FirstValue = source.Value,
                FirstUnit = source.Unit.ToString(),
                SecondValue = null,
                SecondUnit = null,
                TargetUnit = targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString(),
            });

            _logger.LogInformation("Conversion completed: {Value} {Unit} -> {ResultValue} {ResultUnit}",
                source.Value, source.Unit, result.Value, result.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{source.Value} {source.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto AddUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("Add operation started");

            (Quantity<T> q1, Quantity<T> q2) = ValidateBinaryRequest<T>(request);
            Quantity<T> result = _arithmeticService.AddUnit(q1, q2);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Addition",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString(),
            });

            _logger.LogInformation("Addition completed: {ResultValue} {ResultUnit}",
                result.Value, result.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto AddUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("AddToTarget operation started");

            (Quantity<T> q1, Quantity<T> q2) = ValidateBinaryRequest<T>(request);

            if (string.IsNullOrWhiteSpace(request.TargetUnit))
            {
                _logger.LogError("Target unit missing in AddToTarget");
                throw new ArgumentException("Target unit is required.");
            }

            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);
            Quantity<T> result = _arithmeticService.AddToSpecificUnit(q1, q2, targetUnit);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Addition",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString(),
            });

            _logger.LogInformation("AddToTarget completed: {ResultValue} {ResultUnit}",
                result.Value, result.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto SubtractUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("Subtract operation started");

            (Quantity<T> q1, Quantity<T> q2) = ValidateBinaryRequest<T>(request);
            Quantity<T> result = _arithmeticService.SubtractUnit(q1, q2, q1.Unit);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Subtraction",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString(),
            });

            _logger.LogInformation("Subtraction completed: {ResultValue} {ResultUnit}",
                result.Value, result.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public QuantityResultDto SubtractUnitsToTarget<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("SubtractToTarget operation started");

            (Quantity<T> q1, Quantity<T> q2) = ValidateBinaryRequest<T>(request);

            if (string.IsNullOrWhiteSpace(request.TargetUnit))
            {
                _logger.LogError("Target unit missing in SubtractToTarget");
                throw new ArgumentException("Target unit is required.");
            }

            T targetUnit = QuantityDtoMapper.ParseTargetUnit<T>(request.TargetUnit);
            Quantity<T> result = _arithmeticService.SubtractUnit(q1, q2, targetUnit);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Subtraction",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = targetUnit.ToString(),
                ResultValue = result.Value,
                ResultUnit = result.Unit.ToString(),
            });

            _logger.LogInformation("SubtractToTarget completed: {ResultValue} {ResultUnit}",
                result.Value, result.Unit);

            return QuantityDtoMapper.ToResultDto(
                result,
                $"{q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
        }

        public DivisionResultDto DivideUnits<T>(BinaryQuantityRequestDto request) where T : struct, Enum
        {
            _logger.LogInformation("Divide operation started");

            (Quantity<T> q1, Quantity<T> q2) = ValidateBinaryRequest<T>(request);
            double result = _arithmeticService.DivideUnit(q1, q2);

            SaveHistoryAndInvalidateCache(new QuantityHistoryRecord
            {
                Category = typeof(T).Name.Replace("Unit", string.Empty),
                OperationType = "Divide",
                FirstValue = q1.Value,
                FirstUnit = q1.Unit.ToString(),
                SecondValue = q2.Value,
                SecondUnit = q2.Unit.ToString(),
                TargetUnit = null,
                ResultValue = result,
                ResultUnit = "Dimensionless",
            });

            _logger.LogInformation("Divide completed: {ResultValue}", result);

            return new DivisionResultDto
            {
                Value = result,
                Message = $"{q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {result:F4} (dimensionless)",
            };
        }

        public async Task<List<QuantityHistoryRecord>> GetAllRecordsAsync()
        {
            _logger.LogInformation("Checking cache for key {CacheKey}", HistoryAllCacheKey);

            List<QuantityHistoryRecord>? cachedRecords =
                await _cacheService.GetAsync<List<QuantityHistoryRecord>>(HistoryAllCacheKey);

            if (cachedRecords != null)
            {
                _logger.LogInformation("Cache hit for key {CacheKey}", HistoryAllCacheKey);
                return cachedRecords;
            }

            _logger.LogInformation("Cache miss for key {CacheKey}. Fetching from database", HistoryAllCacheKey);

            List<QuantityHistoryRecord> records = _historyRepository.GetAllRecords();
            await _cacheService.SetAsync(HistoryAllCacheKey, records, 15, 5);

            _logger.LogInformation("Cached {Count} records for key {CacheKey}", records.Count, HistoryAllCacheKey);
            return records;
        }

        public async Task<QuantityHistoryRecord?> GetRecordByIdAsync(int id)
        {
            string cacheKey = $"History:{id}";
            _logger.LogInformation("Checking cache for key {CacheKey}", cacheKey);

            QuantityHistoryRecord? cachedRecord = await _cacheService.GetAsync<QuantityHistoryRecord>(cacheKey);
            if (cachedRecord != null)
            {
                _logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
                return cachedRecord;
            }

            QuantityHistoryRecord? record = _historyRepository.GetRecordById(id);
            if (record != null)
            {
                await _cacheService.SetAsync(cacheKey, record, 15, 5);
                _logger.LogInformation("Record with Id {Id} cached", id);
            }
            else
            {
                _logger.LogWarning("Record with Id {Id} not found in database", id);
            }

            return record;
        }

        public async Task AddRecordAsync(QuantityHistoryRecord record)
        {
            _logger.LogInformation("Adding history record for Category {Category}", record.Category);

            _historyRepository.AddRecord(record);
            await InvalidateHistoryCacheAsync();

            _logger.LogInformation("History record added and cache invalidated");
        }

        public async Task<bool> DeleteRecordAsync(int id)
        {
            _logger.LogInformation("Deleting record with Id {Id}", id);
            bool deleted = _historyRepository.DeleteRecord(id);

            if (deleted)
            {
                await _cacheService.RemoveAsync($"History:{id}");
                await _cacheService.RemoveAsync(HistoryAllCacheKey);

                _logger.LogInformation("Deleted record with Id {Id} and invalidated related cache", id);
            }
            else
            {
                _logger.LogWarning("Delete failed for Id {Id}. Record not found", id);
            }

            return deleted;
        }

        private (Quantity<T>, Quantity<T>) ValidateBinaryRequest<T>(BinaryQuantityRequestDto request)
            where T : struct, Enum
        {
            if (request is null)
            {
                _logger.LogError("Request is null");
                throw new ArgumentNullException(nameof(request));
            }

            Quantity<T> q1 = QuantityDtoMapper.ToEntity<T>(request.FirstQuantity);
            Quantity<T> q2 = QuantityDtoMapper.ToEntity<T>(request.SecondQuantity);

            return (q1, q2);
        }

        private void SaveHistoryAndInvalidateCache(QuantityHistoryRecord record)
        {
            _historyRepository.AddRecord(record);

            _cacheService.RemoveAsync(HistoryAllCacheKey).GetAwaiter().GetResult();

            _logger.LogInformation("History saved and cache invalidated for key {CacheKey}", HistoryAllCacheKey);
        }

        private async Task InvalidateHistoryCacheAsync()
        {
            await _cacheService.RemoveAsync(HistoryAllCacheKey);
            _logger.LogInformation("Cache invalidated for key {CacheKey}", HistoryAllCacheKey);
        }
    }
}