
using StartupWebAPIs.Interfaces;

namespace StartupWebAPIs.Jobs
{
    public class BackgroundJobs
    {
        private readonly ILogger<BackgroundJobs> _logger;
        private readonly IEmailService _emailService;
        public BackgroundJobs(ILogger<BackgroundJobs> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task SendWelcomeMessageAsync(string name, string email)
        {
            _logger.LogInformation("Welcome email sent to {Name} at {Time}",
                name,
                DateTime.Now);

            await _emailService.SendWelcomeEmailAsync(name, email);
        }

        // NEW METHOD
        public void CleanExpiredApiKeys()
        {
            _logger.LogInformation(
                "Cleaning expired API Keys at {Time}",
                DateTime.Now);

            Console.WriteLine("Cleaning expired API Keys...");
        }
    }
}