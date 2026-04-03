using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using HistoryService.Data;
using HistoryService.Models;
using Shared.Contracts;
using System.Text.Json;

namespace HistoryService.Repositories;

// ── Repository interface ──────────────────────────────────────────────────
public interface IHistoryRepository
{
    Task<List<QuantityHistoryEntity>> GetAllAsync();
    Task<QuantityHistoryEntity?> GetByIdAsync(int id);
    Task<QuantityHistoryEntity> AddAsync(QuantityHistoryEntity record);
    Task<bool> DeleteAsync(int id);
}

// ── EF-backed repository ──────────────────────────────────────────────────
public class HistoryRepository : IHistoryRepository
{
    private readonly HistoryDbContext _db;

    public HistoryRepository(HistoryDbContext db) => _db = db;

    public Task<List<QuantityHistoryEntity>> GetAllAsync() =>
        _db.QuantityHistory.OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task<QuantityHistoryEntity?> GetByIdAsync(int id) =>
        _db.QuantityHistory.FindAsync(id).AsTask()!;

    public async Task<QuantityHistoryEntity> AddAsync(QuantityHistoryEntity record)
    {
        _db.QuantityHistory.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.QuantityHistory.FindAsync(id);
        if (entity is null) return false;
        _db.QuantityHistory.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}

// ── Redis cache service ───────────────────────────────────────────────────
public class HistoryCacheService
{
    private const string AllKey    = "History:all";
    private const string ItemKeyFn = "History:{0}";

    private readonly IDistributedCache _cache;
    private readonly ILogger<HistoryCacheService> _logger;

    private static readonly DistributedCacheEntryOptions ShortTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
        SlidingExpiration               = TimeSpan.FromMinutes(5)
    };

    public HistoryCacheService(IDistributedCache cache, ILogger<HistoryCacheService> logger)
    {
        _cache  = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes is null) return null;
            _logger.LogDebug("Cache hit: {Key}", key);
            return JsonSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache get failed for {Key}", key); return null; }
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cache.SetAsync(key, bytes, ShortTtl);
            _logger.LogDebug("Cache set: {Key}", key);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache set failed for {Key}", key); }
    }

    public async Task RemoveAsync(string key)
    {
        try { await _cache.RemoveAsync(key); _logger.LogDebug("Cache removed: {Key}", key); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache remove failed for {Key}", key); }
    }

    public string AllRecordsKey   => AllKey;
    public string ItemKey(int id) => string.Format(ItemKeyFn, id);
}

// ── Orchestrating service (cache-aside) ───────────────────────────────────
public class HistoryApplicationService
{
    private readonly IHistoryRepository _repo;
    private readonly HistoryCacheService _cache;
    private readonly ILogger<HistoryApplicationService> _logger;

    public HistoryApplicationService(
        IHistoryRepository repo,
        HistoryCacheService cache,
        ILogger<HistoryApplicationService> logger)
    {
        _repo  = repo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<QuantityHistoryRecordDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<QuantityHistoryRecordDto>>(_cache.AllRecordsKey);
        if (cached is not null) return cached;

        var entities = await _repo.GetAllAsync();
        var dtos     = entities.Select(ToDto).ToList();

        await _cache.SetAsync(_cache.AllRecordsKey, dtos);
        _logger.LogInformation("Fetched {Count} history records from DB", dtos.Count);
        return dtos;
    }

    public async Task<QuantityHistoryRecordDto?> GetByIdAsync(int id)
    {
        var cached = await _cache.GetAsync<QuantityHistoryRecordDto>(_cache.ItemKey(id));
        if (cached is not null) return cached;

        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;

        var dto = ToDto(entity);
        await _cache.SetAsync(_cache.ItemKey(id), dto);
        return dto;
    }

    public async Task<QuantityHistoryRecordDto> AddAsync(CreateHistoryRecordDto request)
    {
        var entity = new QuantityHistoryEntity
        {
            Category      = request.Category,
            OperationType = request.OperationType,
            FirstValue    = request.FirstValue,
            FirstUnit     = request.FirstUnit,
            SecondValue   = request.SecondValue,
            SecondUnit    = request.SecondUnit,
            TargetUnit    = request.TargetUnit,
            ResultValue   = request.ResultValue,
            ResultUnit    = request.ResultUnit
        };

        var saved = await _repo.AddAsync(entity);

        // Invalidate list cache
        await _cache.RemoveAsync(_cache.AllRecordsKey);

        _logger.LogInformation("Added history record Id={Id}", saved.Id);
        return ToDto(saved);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        bool deleted = await _repo.DeleteAsync(id);
        if (deleted)
        {
            await _cache.RemoveAsync(_cache.ItemKey(id));
            await _cache.RemoveAsync(_cache.AllRecordsKey);
            _logger.LogInformation("Deleted history record Id={Id}", id);
        }
        return deleted;
    }

    private static QuantityHistoryRecordDto ToDto(QuantityHistoryEntity e) =>
        new(e.Id, e.Category, e.OperationType,
            e.FirstValue, e.FirstUnit,
            e.SecondValue, e.SecondUnit,
            e.TargetUnit,
            e.ResultValue, e.ResultUnit,
            e.CreatedAt);
}
