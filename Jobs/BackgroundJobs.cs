namespace StartupWebAPIs.Jobs
{
    public class BackgroundJobs
    {
        private readonly ILogger<BackgroundJobs> _logger;

        public BackgroundJobs(ILogger<BackgroundJobs> logger)
        {
            _logger = logger;
        }

        public void SendWelcomeMessage(string name)
        {
            _logger.LogInformation("Welcome email sent to {Name} at {Time}",
                name,
                DateTime.Now);

            Console.WriteLine($"Welcome email sent to {name}");
        }
    }
}