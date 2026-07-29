using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StartupWebAPIs.DTOs;
using StartupWebAPIs.Jobs;

namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        [HttpPost("welcome")]
        public IActionResult Welcome([FromServices] IBackgroundJobClient jobClient, WelcomeEmailRequest request)
        {
            jobClient.Schedule<BackgroundJobs>(
    job => job.SendWelcomeMessageAsync(request.Name, request.Email),
    TimeSpan.FromMinutes(1));

            return Ok("Background Job Created Successfully.");
        }
    }
}