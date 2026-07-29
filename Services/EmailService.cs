using Microsoft.Extensions.Options;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using System.Net;
using System.Net.Mail;

namespace StartupWebAPIs.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> options,
            ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string name, string email)
        {
            var message = new MailMessage
            {
                From = new MailAddress(
                    _settings.SenderEmail,
                    _settings.SenderName),

                Subject = "Welcome to StartupWebAPIs",

                Body = $"Hello {name},<br/><br/>Welcome to StartupWebAPIs!<br/><br/>Thank you.",
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var smtp = new SmtpClient(_settings.Host, _settings.Port);

            smtp.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password);

            smtp.EnableSsl = _settings.EnableSsl;

            await smtp.SendMailAsync(message);

            _logger.LogInformation(
                "Welcome email sent successfully to {Email}",
                email);
        }
    }
}