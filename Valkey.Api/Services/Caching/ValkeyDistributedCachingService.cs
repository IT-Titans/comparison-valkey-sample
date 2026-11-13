using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

namespace Valkey.Api.Services.Caching;

/// <summary>
/// An implementation of <see cref="ICachingService" /> using the <see cref="IDistributedCache" />.
/// </summary>
public class ValkeyDistributedCachingService : ICachingService
{
    private readonly ILogger<ValkeyDistributedCachingService> _logger;

    private readonly IDistributedCache _cache;

    public ValkeyDistributedCachingService(ILogger<ValkeyDistributedCachingService> logger, IDistributedCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T? value, TimeSpan expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _logger.LogInformation("Storing value for key '{Key}' into cache", key);
        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expiry, }).ConfigureAwait(false);
        _logger.LogInformation("Stored value for key '{Key}' into cache", key);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _logger.LogInformation("Looking for a cached value for key '{Key}'", key);
        var value = await _cache.GetStringAsync(key).ConfigureAwait(false);

        if (value is null)
        {
            _logger.LogInformation("No cached value found for key '{Key}'", key);
            return default(T);
        }

        _logger.LogInformation("Found cached value for key '{Key}'", key);
        return JsonSerializer.Deserialize<T>(value);
    }

    /// <inheritdoc />
    public async Task InvalidateAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _logger.LogInformation("Invalidating the cached value for key '{Key}'", key);
        await _cache.RemoveAsync(key).ConfigureAwait(false);
        _logger.LogInformation("Invalidated the cached value for key '{Key}'", key);
    }
}