using System.Net;
using System.Net.Mail;

namespace Biblioteka.Web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var appPassword = _config["EmailSettings:Password"];

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, appPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, "Biblioteka System"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}