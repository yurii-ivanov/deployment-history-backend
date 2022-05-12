using DeploymentsHistoryBackend.Data;
using DeploymentsHistoryBackend.Models;

namespace DeploymentsHistoryBackend.Services
{
    public interface IDeploymentsService
    {
        Task<IEnumerable<Deployment>> GetByAppId(int appId);
        Task<IEnumerable<Deployment>> GetByAppIdPaged(int appId, int skip, int take);
        Task<IEnumerable<Deployment>> Get(int limit);
        Task<Deployment> Save(Deployment deployment);
        Task<IEnumerable<Deployment>> SaveMany(IEnumerable<Deployment> deployments);
    }
    public class DeploymentsService : IDeploymentsService
    {
        private readonly IDeploymentsRepository _deploymentsRepository;
        private readonly ICachedReleasesService _releasesService;

        public DeploymentsService(IDeploymentsRepository deploymentsRepository, ICachedReleasesService releasesService)
        {
            _deploymentsRepository = deploymentsRepository;
            _releasesService = releasesService;
        }


        public async Task<IEnumerable<Deployment>> GetByAppId(int appId)
        {
            if (appId < 1) throw new ArgumentOutOfRangeException(nameof(appId));

            return await _deploymentsRepository.GetByAppId(appId);
        }

        public async Task<IEnumerable<Deployment>> GetByAppIdPaged(int appId, int skip, int take)
        {
            if (appId <= 0) throw new ArgumentOutOfRangeException(nameof(appId));
            if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));
            if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));

            return await _deploymentsRepository.GetByAppIdPaged(appId, skip, take);
        }

        public async Task<IEnumerable<Deployment>> Get(int limit)
        {
            if (limit < 1) throw new ArgumentOutOfRangeException(nameof(limit));
            return await _deploymentsRepository.Get(limit);
        }

        public async Task<Deployment> Save(Deployment deployment)
        {
            if (deployment.IsValid == false)
            {
                throw new ArgumentException(nameof(deployment) + $" is not valid + ({deployment})");
            }
            deployment = await _deploymentsRepository.Save(deployment);

            if (deployment.Id > 0)
            {
                _releasesService.DeleteCacheFor(deployment.Application.Name);
            }

            return deployment;
        }

        public async Task<IEnumerable<Deployment>> SaveMany(IEnumerable<Deployment> deployments)
        {
            var appsNames = new List<string>();
            foreach (var deployment in deployments)
            {
                if (deployment.IsValid == false)
                {
                    throw new ArgumentException(nameof(deployment) + $" is not valid + ({deployment})");
                }
                appsNames.Add(deployment.Application.Name);
            }

            deployments = await _deploymentsRepository.SaveMany(deployments);
            appsNames.ForEach(n => _releasesService.DeleteCacheFor(n));

            return deployments;
        }
    }
}
