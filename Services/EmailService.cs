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
        private readonly IConfiguration _configuration;

        public EmailService(
            IOptions<EmailSettings> options,
            ILogger<EmailService> logger, IConfiguration configuration)
        {
            _settings = options.Value;
            _logger = logger;
            _configuration = configuration;
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
        public async Task SendEmailWithAttachmentAsync(string toEmail,string subject,string body,string attachmentPath)
        {
            using var message = new MailMessage();

            message.To.Add(toEmail);
            message.Subject = subject;
           message.Body = body;

            message.From = new MailAddress(_configuration["EmailSettings:From"]!);

           if (System.IO.File.Exists(attachmentPath))
            {
                message.Attachments.Add(new Attachment(attachmentPath));
            }

            using var smtp = new SmtpClient(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:Port"]!));

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]);

            await smtp.SendMailAsync(message);

            _logger.LogInformation(
                "Email with attachment sent to {Email}",
                toEmail);
        }
    }
}