using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace Hachiko.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string userName = emailSettings["Username"] ?? throw new InvalidOperationException("Email username not configured");
            string password = emailSettings["Password"] ?? throw new InvalidOperationException("Email password not configured");
            string smtpHost = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
            int smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(userName, password),
                EnableSsl = true
            };

            return client.SendMailAsync(new MailMessage(from: userName, to: email, subject: subject, body: htmlMessage)
            {
                IsBodyHtml = true
            });
        }
    }
}
