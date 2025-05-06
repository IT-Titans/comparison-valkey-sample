using Microsoft.Extensions.Caching.Distributed;

namespace Valkey.Api.Services.Caching;

/// <summary>
/// An implementation of <see cref="IDistributedCache" /> using the <see cref="ICachingService" />.
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
    public Task SetAsync<T>(string key, T? value, TimeSpan expiry)
    {
        throw new NotSupportedException("IDistributedCache is not fully supported");
    }

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key)
    {
        throw new NotSupportedException("IDistributedCache is not fully supported");
    }

    /// <inheritdoc />
    public Task InvalidateAsync(string key)
    {
        throw new NotSupportedException("IDistributedCache is not fully supported");
    }
}