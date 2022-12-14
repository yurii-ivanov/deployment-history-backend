using DeploymentHistoryBackend.Controllers.Models;
using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models;
using DeploymentHistoryBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentHistoryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeploymentsController : Controller
    {
        private readonly IDeploymentsService _deploymentsService;
        private readonly ICachedApplicationsRepository _applicationsRepository;
        private readonly IConfiguration _configuration;

        public DeploymentsController(
            IDeploymentsService deploymentsService,
            ICachedApplicationsRepository applicationsRepository,
            IConfiguration configuration
        )
        {
            _deploymentsService = deploymentsService;
            _applicationsRepository = applicationsRepository;
            _configuration = configuration;
        }

        // GET: api/Deployments/5
        [HttpGet("{limit?}")]
        public async Task<IActionResult> GetDeployments(int limit = 10)
        {
            var d = await _deploymentsService.Get(limit);
            return new OkObjectResult(d);
        }

        // GET: api/Deployments/app/5
        [HttpGet("app/{appId}")]
        public async Task<IActionResult> GetByAppId(int appId)
        {
            if (appId <= 0)
            {
                return BadRequest($"{nameof(appId)} not valid");
            }

            var deployments = await _deploymentsService.GetByAppId(appId);

            return new OkObjectResult(deployments);
        }


        // POST api/Deployments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeploymentEdit request)
        {

            if (ModelState.IsValid == false)
            {
                var result = new BadRequestObjectResult(new ErrorResponse() { Message = "Data is missing check again."});
                return result;
            }

            var deployment = await CreateDeployment(request);

            deployment = await _deploymentsService.Save(deployment);

            return new OkObjectResult(deployment);
        }

        // POST api/Deployments/many
        [HttpPost]
        [Route("many")]
        public async Task<IActionResult> CreateMany([FromBody] DeploymentEdit[] request)
        {

            if (ModelState.IsValid == false)
            {
                var result = new BadRequestObjectResult(new ErrorResponse() { Message = "Data is missing check again." });
                return result;
            }

            var deployments = new List<Deployment>();

            foreach (var deploymentEdit in request)
            {
                var deployment = await CreateDeployment(deploymentEdit);
                deployments.Add(deployment);
            }

            deployments = deployments.Distinct().ToList();
            deployments = (await _deploymentsService.SaveMany(deployments)).ToList();

            return new OkObjectResult(deployments);
        }

        private async Task<Deployment> CreateDeployment(DeploymentEdit deploymentEdit)
        {
            if (string.IsNullOrWhiteSpace(deploymentEdit.ApplicationName))
            {
                throw new ArgumentException(nameof(deploymentEdit.ApplicationName) + " is missing");
            }
            if (string.IsNullOrWhiteSpace(deploymentEdit.CommitId))
            {
                throw new ArgumentException(nameof(deploymentEdit.CommitId) + " is missing");
            }

            var app = await _applicationsRepository.GetByName(deploymentEdit.ApplicationName);

            var commitTimestamp = TimeZoneInfo.ConvertTime(DateTime.Now, Program.AppTimeZone);
            if (deploymentEdit.Timestamp.HasValue)
            {
                commitTimestamp = TimeZoneInfo.ConvertTime(deploymentEdit.Timestamp.Value, Program.AppTimeZone);
            }

            var deployment = new Deployment()
            {
                Application = app,
                CommitId = deploymentEdit.CommitId,
                Timestamp = commitTimestamp,
                BranchName = deploymentEdit.BranchName
            };

            return deployment;
        }
    }
}
