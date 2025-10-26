using API.DTOs.Auth;
using API.IService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Services
{
    public class GmailService : IGmailService
    {
        private readonly GmailSettings gmailSettings;
        private readonly SendGridSettings sendGridSettings;

        public GmailService(
            IOptions<GmailSettings> gmailOption,
            IOptions<SendGridSettings> sendGridOption)
        {
            gmailSettings = gmailOption.Value;
            sendGridSettings = sendGridOption.Value;
        }

        // Gửi email bằng Gmail SMTP
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("No Reply", gmailSettings.GmailAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(gmailSettings.GmailAddress, gmailSettings.AppPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // Gửi email bằng SendGrid API
        public async Task SendEmailSendGridAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(sendGridSettings.ApiKey);
            var from = new EmailAddress(sendGridSettings.SenderEmail, sendGridSettings.SenderName);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid send email failed: {response.StatusCode} - {error}");
            }
        }
    }
}
