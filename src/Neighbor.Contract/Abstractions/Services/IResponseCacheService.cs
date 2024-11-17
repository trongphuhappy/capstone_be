namespace Neighbor.Contract.Abstractions.Services;

public interface IResponseCacheService
{
    Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut);
    Task<string> GetCacheResponseAsync(string cacheKey);
    Task DeleteCacheResponseAsync(string cacheKey);
    Task SetListAsync<T>(string cacheKey, List<T> list, TimeSpan timeOut);
    Task<List<T>> GetListAsync<T>(string cacheKey);
    Task SetCacheResponseNoTimeoutAsync(string cacheKey, object response);
}