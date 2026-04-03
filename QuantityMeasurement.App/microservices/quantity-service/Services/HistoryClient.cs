using Shared.Contracts;
using System.Net.Http.Json;

namespace QuantityService.Services;

/// <summary>
/// Typed HTTP client that forwards history records to the History Service.
/// The URL is resolved from configuration key "Services:HistoryService".
/// </summary>
public interface IHistoryClient
{
    Task AddRecordAsync(CreateHistoryRecordDto record);
}

public class HistoryClient : IHistoryClient
{
    private readonly HttpClient _http;
    private readonly ILogger<HistoryClient> _logger;

    public HistoryClient(HttpClient http, ILogger<HistoryClient> logger)
    {
        _http   = http;
        _logger = logger;
    }

    public async Task AddRecordAsync(CreateHistoryRecordDto record)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/history", record);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Non-critical: log but don't fail the main operation
            _logger.LogWarning(ex, "Failed to persist history record to History Service.");
        }
    }
}
