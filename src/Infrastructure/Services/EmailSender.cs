using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

using Codidact.Authentication.Infrastructure.Common.Interfaces;
using Codidact.Authentication.Domain.Entities;

namespace Codidact.Authentication.Infrastructure.Services
{
    public class EmailSender : IEmailSender<EmailSettings>
    {
        private EmailSettings _emailConfiguration;
        public EmailSender(EmailSettings emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public async Task SendEmailAsync(string email, string subject, string textMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.Sender));
            message.To.Add(new MailboxAddress("Username", email));

            message.Subject = subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = textMessage
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect(_emailConfiguration.Host, _emailConfiguration.Port, _emailConfiguration.EnableSsl);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.Sender, _emailConfiguration.Password);

                await emailClient.SendAsync(message);

                emailClient.Disconnect(true);
            }

        }
        public async Task SendResetPassword(string email, string token)
        {
            await SendEmailAsync(email, "Reset your Password", "Click here to reset your password <a href='google.com'>Test</a>");

        }
    }
}
