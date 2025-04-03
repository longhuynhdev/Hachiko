using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Hachiko.Utility
{
    public class EmailSender : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string userName = "duahaudev@gmail.com";
            string passWord = "scta neam pdqy prbk";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential(userName, passWord),
                EnableSsl = true
            };

            return client.SendMailAsync(new MailMessage(from: userName, to: email, subject: subject, body: htmlMessage)
            {
                IsBodyHtml = true
            });
        }
    }
}
