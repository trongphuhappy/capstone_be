using Microsoft.Extensions.Caching.Distributed;
using Neighbor.Contract.Abstractions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Neighbor.Infrastructure.Services;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public ResponseCacheService
        (IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task DeleteCacheResponseAsync(string cacheKey)
    {
        await _distributedCache.RemoveAsync(cacheKey);
    }

    public async Task<string> GetCacheResponseAsync(string cacheKey)
    {
        var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
        return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
    }

    public async Task<List<T>> GetListAsync<T>(string cacheKey)
    {
        var database = _connectionMultiplexer.GetDatabase();
        var serializedList = await database.StringGetAsync(cacheKey);

        if (serializedList.IsNullOrEmpty)
            return null;

        return JsonConvert.DeserializeObject<List<T>>(serializedList);
    }

    public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
    {
        if (response == null) return;
        var serializerResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        await _distributedCache.SetStringAsync(cacheKey, serializerResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeOut,
        });
    }

    public async Task SetCacheResponseNoTimeoutAsync(string cacheKey, object response)
    {
        if (response == null) return;
        var serializerResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        await _distributedCache.SetStringAsync(cacheKey, serializerResponse);
    }

    public async Task SetListAsync<T>(string cacheKey, List<T> list, TimeSpan timeOut)
    {
        if (list == null || !list.Any()) return;

        var database = _connectionMultiplexer.GetDatabase();
        var serializedList = JsonConvert.SerializeObject(list, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        // Store the serialized list as a string in Redis
        await database.StringSetAsync(cacheKey, serializedList, timeOut);
    }
}
