using DeploymentsHistoryBackend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DeploymentsHistoryBackend.Services
{
    public interface ICachedReleasesService : IReleasesService 
    {
        Task<IEnumerable<Release>> GetReleasesPaged(Application app, int skip, int pageSize);
        void DeleteCacheFor(string app);
    }
    public class CachedReleasesService : ICachedReleasesService
    {
        private readonly IReleasesService _releasesService;
        private readonly IMemoryCache _memoryCache;
        private const string RELEASES_CACHE_KEY_PREFIX = "Releases";
        private readonly TimeSpan _releasesCacheTimeout = TimeSpan.FromMinutes(5);
        private static Dictionary<string, bool>? _cachedKeys;

        public CachedReleasesService(IReleasesService releasesService, IMemoryCache memoryCache)
        {
            _releasesService = releasesService;
            _memoryCache = memoryCache;

            _cachedKeys ??= new Dictionary<string, bool>();
        }

        public async Task<IEnumerable<Release>> GetReleasesPaged(Application app, int skip, int pageSize)
        {
            var key = $"{RELEASES_CACHE_KEY_PREFIX}-{app.Name}-{skip}-{pageSize}";
            if (!_memoryCache.TryGetValue<IEnumerable<Release>>(key, out var releases))
            {
                releases = await _releasesService.GetReleases(app);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_releasesCacheTimeout)
                    .SetSize(releases.Count());

                _memoryCache.Set(key, releases, cacheOptions);
                _cachedKeys.TryAdd(key, true);
            }

            return releases;
        }

        public async Task<IEnumerable<Release>> GetReleases(Application app)
        {
            var key = $"{RELEASES_CACHE_KEY_PREFIX}-{app.Name}";
            if (!_memoryCache.TryGetValue<IEnumerable<Release>>(key, out var releases))
            {
                releases = await _releasesService.GetReleases(app);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_releasesCacheTimeout.Add(TimeSpan.FromDays(7)))
                    .SetSize(releases.Count());

                _memoryCache.Set(key, releases, cacheOptions);
                _cachedKeys.TryAdd(key, true);
            }

            return releases;
        }

        public void DeleteCacheFor(string appName)
        {
            var cacheKey = $"{RELEASES_CACHE_KEY_PREFIX}-{appName}";
            var keys = _cachedKeys.Keys.Where(k => k.StartsWith(cacheKey));

            foreach (var key in keys)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}
