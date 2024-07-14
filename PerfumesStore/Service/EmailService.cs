using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PerfumesStore.Models;

namespace PerfumesStore.Service
{
    public class EmailService:IEmaiService
    {

        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("coffeemyshoeshop@gmail.com", _emailSettings.FromEmail));
            email.To.Add(new MailboxAddress("thovpce161117@fpt.edu.vn", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
