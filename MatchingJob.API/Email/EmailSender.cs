
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace MatchingJob.API.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSetting _mailSetting;

        public EmailSender(IOptions<MailSetting> mailSetting) {
            _mailSetting = mailSetting.Value;
        }
        public void sendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            send(emailMessage);
        }

        private void send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _mailSetting.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_mailSetting.UserName, _mailSetting.Password);
                    client.Send(emailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _mailSetting.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
    }
}
