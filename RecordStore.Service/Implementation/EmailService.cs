using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RecordStore.Domain.DTO.Email;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            _logger.LogInformation($"Attempting to send email to: {message.MailTo} with subject: {message.Subject}");

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.SenderEmail);
            email.To.Add(MailboxAddress.Parse(message.MailTo));
            email.Subject = message.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message.Content 
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                _logger.LogInformation($"Connecting to SMTP server: {_mailSettings.SmtpServer}:{_mailSettings.SmtpPort}");
                await smtp.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("Authenticating with SMTP server.");
                await smtp.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                _logger.LogInformation("Sending email.");
                await smtp.SendAsync(email);
                _logger.LogInformation("Email sent successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {message.MailTo}. Error: {ex.Message}");
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
                smtp.Dispose();
            }
        }
    }
}
