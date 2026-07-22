using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StartupWebAPIs.Data;
using StartupWebAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApiKeyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiKeyController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/apikey/generate
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateApiKey()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var apiKey = new ApiKey
            {
                UserId = userId,
                Key = Guid.NewGuid().ToString("N"),
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };

            _context.ApiKeys.Add(apiKey);

            await _context.SaveChangesAsync();

            return Ok(apiKey);
        }

        // GET: api/apikey
        [HttpGet]
        public async Task<IActionResult> GetMyApiKeys()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var keys = await _context.ApiKeys
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return Ok(keys);
        }

        // DELETE: api/apikey/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (apiKey == null)
            {
                return NotFound("API Key not found.");
            }

            _context.ApiKeys.Remove(apiKey);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "API Key deleted successfully."
            });
        }
    }
}
