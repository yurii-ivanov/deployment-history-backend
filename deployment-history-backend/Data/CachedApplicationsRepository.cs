using DeploymentHistoryBackend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DeploymentHistoryBackend.Data
{
    public interface ICachedApplicationsRepository : IApplicationsRepository {}
    public class CachedApplicationsRepository : ICachedApplicationsRepository
    {
        private readonly IApplicationsRepository _applicationsRepository;
        private readonly IMemoryCache _memoryCache;
        private const string APPLICATIONS_CACHE_KEY = "All-Applications";
        private readonly TimeSpan _applicationsCacheTimeout = TimeSpan.FromMinutes(5);

        public CachedApplicationsRepository(IApplicationsRepository applicationsRepository, IMemoryCache memoryCache)
        {
            _applicationsRepository = applicationsRepository;
            _memoryCache = memoryCache;
        }

        public async Task<Application> GetById(int appId)
        {
            var apps = await GetAll();
            return apps.FirstOrDefault(a => a.Id == appId);
        }

        public async Task<Application> GetByName(string name)
        {
            var apps = await GetAll();
            return apps.FirstOrDefault(a => a.Name == name);
        }

        public async Task<IEnumerable<Application>> GetAll()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<Application>>(APPLICATIONS_CACHE_KEY, out var apps))
            {
                apps = await _applicationsRepository.GetAll();

                var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_applicationsCacheTimeout)
                    .SetSize(apps.Count());
                
                _memoryCache.Set(APPLICATIONS_CACHE_KEY, apps, memoryCacheEntryOptions);
            }

            return apps;
        }

        public async Task<Application> Save(Application application)
        {
            await _applicationsRepository.Save(application);
            _memoryCache.Remove(APPLICATIONS_CACHE_KEY);
            return application;
        }

        public async Task<bool> Delete(int appId)
        {
            var result = await _applicationsRepository.Delete(appId);
            _memoryCache.Remove(APPLICATIONS_CACHE_KEY);
            return result;
        }
    }
}
