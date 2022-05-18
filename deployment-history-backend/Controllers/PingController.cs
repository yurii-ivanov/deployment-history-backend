using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DeploymentHistoryBackend.Controllers
{
    [ApiController]
    public class PingController : ControllerBase
    {
        static readonly string Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        [HttpGet("")]
        [HttpGet("ping")]
        public IActionResult Get()
        {
            return Ok("pong");
        }

        [HttpGet("ping/env")]
        public string GetEnv()
        {
            return JsonConvert.SerializeObject(Environment);
        }
    }
}