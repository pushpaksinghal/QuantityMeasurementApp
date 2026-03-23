using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            string? jsonDATA = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(jsonDATA))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(jsonDATA);
        }

        public async Task SetAsync<T>(string key, T value,int absoulteMinutes =10,int slidingMinutes = 5)
        {
            var option = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoulteMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(slidingMinutes)
            };
            string jsaonDATA = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsaonDATA, option);

        }

        public async Task RemoveAsync(String key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
