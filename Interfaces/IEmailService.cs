namespace StartupWebAPIs.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string name, string email);
    

   Task SendEmailWithAttachmentAsync(
    string toEmail,
    string subject,
    string body,
    string attachmentPath);
    }
}