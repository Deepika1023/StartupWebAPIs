namespace StartupWebAPIs.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string name, string email);
    }
}