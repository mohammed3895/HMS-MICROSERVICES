using HMS.Authentication.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(
            IMemoryCache memoryCache,
            ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out T? value))
                {
                    _logger.LogDebug("Cache hit for key {Key}", key);
                    return Task.FromResult(value);
                }

                _logger.LogDebug("Cache miss for key {Key}", key);
                return Task.FromResult<T?>(default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken = default)
        {
            try
            {
                var expirationTime = expiration ?? TimeSpan.FromMinutes(30);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime,
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                _memoryCache.Set(key, value, cacheEntryOptions);
                _logger.LogDebug("Value cached for key {Key} with expiration {Expiration}", key, expirationTime);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching value for key {Key}", key);
                throw;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                _logger.LogDebug("Removed key {Key} from cache", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key {Key}", key);
                return Task.FromResult(false);
            }
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var cachedValue = await GetAsync<T>(key, cancellationToken);

            if (cachedValue != null)
            {
                _logger.LogDebug("Cache hit for key {Key}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key {Key}, generating value", key);
            var value = await factory();
            await SetAsync(key, value, expiration, cancellationToken);

            return value;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            // In-memory cache doesn't support prefix removal easily
            // This would require tracking all keys, which is not implemented here
            _logger.LogWarning("RemoveByPrefixAsync called but not fully implemented for in-memory cache");
            return Task.CompletedTask;
        }

        public string GenerateUserCacheKey(Guid userId, string suffix = "")
        {
            return string.IsNullOrEmpty(suffix)
                ? $"user:{userId}"
                : $"user:{userId}:{suffix}";
        }

        public string GenerateRoleCacheKey(Guid roleId, string suffix = "")
        {
            return string.IsNullOrEmpty(suffix)
                ? $"role:{roleId}"
                : $"role:{roleId}:{suffix}";
        }
    }
}