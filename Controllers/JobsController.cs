using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StartupWebAPIs.Jobs;

namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        [HttpPost("welcome")]
        public IActionResult Welcome(
            [FromServices] IBackgroundJobClient jobClient)
        {
            jobClient.Schedule<BackgroundJobs>(
    job => job.SendWelcomeMessage("Deepika"),
    TimeSpan.FromMinutes(1));

            return Ok("Background Job Created Successfully.");
        }
    }
}