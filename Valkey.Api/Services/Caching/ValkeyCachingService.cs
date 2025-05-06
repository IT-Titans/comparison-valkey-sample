using System.Text.Json;

using StackExchange.Redis;

namespace Valkey.Api.Services.Caching;

/// <summary>
/// An implementation of <see cref="ICachingService" /> using Valkey directly.
/// </summary>
public class ValkeyCachingService : ICachingService
{
    private readonly ILogger<ValkeyCachingService> _logger;

    private readonly IConnectionMultiplexer _valkey;

    public ValkeyCachingService(
        ILogger<ValkeyCachingService> logger,
        IConnectionMultiplexer valkey)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _valkey = valkey ?? throw new ArgumentNullException(nameof(valkey));
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T? value, TimeSpan expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _valkey.GetDatabase();

        _logger.LogInformation("Storing value for key '{Key}' into cache", key);
        await db.StringSetAsync(
            key: key,
            value: JsonSerializer.Serialize(value),
            expiry: expiry).ConfigureAwait(false);
        _logger.LogInformation("Stored value for key '{Key}' into cache", key);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _valkey.GetDatabase();

        _logger.LogInformation("Looking for a cached value for key '{Key}'", key);

        if (await db.KeyExistsAsync(key).ConfigureAwait(false) == false)
        {
            _logger.LogInformation("No cached value found for key '{Key}'", key);
            return default(T);
        }

        var value = await db.StringGetAsync(key).ConfigureAwait(false);

        _logger.LogInformation("Found cached value for key '{Key}'", key);
        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <inheritdoc />
    public async Task InvalidateAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _valkey.GetDatabase();

        _logger.LogInformation("Invalidating the cached value for key '{Key}'", key);
        await db.KeyDeleteAsync(key).ConfigureAwait(false);
        _logger.LogInformation("Invalidated the cached value for key '{Key}'", key);
    }
}