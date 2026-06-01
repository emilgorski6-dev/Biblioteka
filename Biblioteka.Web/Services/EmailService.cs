using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Biblioteka System", senderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Używamy portu 587 i StartTls
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                
                // Logowanie za pomocą Hasła Aplikacji
                await client.AuthenticateAsync(senderEmail, appPassword);
                
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}