using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models;
using DeploymentHistoryBackend.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeploymentHistoryBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleasesController : ControllerBase
    {
        private readonly ICachedApplicationsRepository _applicationsRepository;
        private readonly IDeploymentsRepository _deploymentsRepository;
        private readonly ICachedReleasesService _releasesService;

        public ReleasesController(
            ICachedApplicationsRepository applicationsRepository,
            IDeploymentsRepository deploymentsRepository,
            ICachedReleasesService releasesService)
        {
            _applicationsRepository = applicationsRepository;
            _deploymentsRepository = deploymentsRepository;
            _releasesService = releasesService;
        }
        // GET: api/<ReleaseController>
        [HttpGet("{appId:int}")]
        public async Task<IEnumerable<Release>> GetByAppId(int appId)
        {
            var app = await _applicationsRepository.GetById(appId);
            var deployments = await _deploymentsRepository.GetByAppId(appId);
            var releases = new List<Release>();

            if (deployments.Count() < 2)
            {
                return releases;
            }

            releases = (await _releasesService.GetReleases(app)).ToList();

            return releases;
        }

        // GET: api/<releases/{appid}?page={}&pageSize={}>
        [HttpGet("{appId:int}/{page:int}/{pageSize:int:min(2)}")]
        public async Task<IEnumerable<Release>> GetByAppIdPaged(int appId, int page, int pageSize)
        {
            var take = pageSize + 1; // because getting releases between deployments and there are only 2 stretches, between 1st and 2nd and 2nd and 3rd deployments
            var skip = (page - 1) * (pageSize);
            var app = await _applicationsRepository.GetById(appId);
            app.Deployments = (await _deploymentsRepository.GetByAppIdPaged(appId, skip, take)).ToList();
            //app.Deployments = deployments
            var releases = new List<Release>();

            if (app.Deployments.Count() < 2)
            {
                return releases;
            }

            releases = (await _releasesService.GetReleasesPaged(app, page, take)).ToList();

            return releases;
        }

    }
}
