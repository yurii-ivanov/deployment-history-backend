using DeploymentHistoryBackend.Data;
using DeploymentHistoryBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploymentHistoryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : Controller
    {
        private readonly ICachedApplicationsRepository _applicationsRepository;

        public ApplicationsController(ICachedApplicationsRepository applicationsRepository)
        {
            _applicationsRepository = applicationsRepository;
        }

        //GET: Applications
        [HttpGet(Name = "GetApplicationsList")]
        public async Task<IEnumerable<Application>> GetAll()
        {
            return await _applicationsRepository.GetAll();
        }

        //GET: Applications/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var application = await _applicationsRepository.GetById(id);
                return new OkObjectResult(application);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Application application)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _applicationsRepository.Save(application);
                return new OkObjectResult(application);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpDelete("{appId:int}")]
        public async Task<IActionResult> Delete(int appId)
        {
            try
            {
                var success = await _applicationsRepository.Delete(appId);
                return new OkObjectResult(success);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
