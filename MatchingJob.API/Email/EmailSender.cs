
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NuGet.Packaging;
using System.Net.Mail;

namespace MatchingJob.API.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSetting _mailSetting;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<MailSetting> mailSetting, ILogger<EmailSender> logger) {
            _mailSetting = mailSetting.Value;
            _logger = logger;
        }

        // Gửi email, theo nội dung trong message
        public async Task sendEmail(Message message)
        {
            var email = CreateEmailMessage(message);

            var builder = new BodyBuilder();
            builder.HtmlBody = message.Content;
            email.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(_mailSetting.SmtpServer, _mailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetting.Mail, _mailSetting.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            _logger.LogInformation("send mail to " + message.To);

        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail);
            email.From.Add(new MailboxAddress(_mailSetting.DisplayName, _mailSetting.Mail));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;
            return email;
        }

        public async Task sendEmailAsync(string email, string subject, string htmlMessage)
        {
            await sendEmail(new Message()
            {
                To = email,
                Subject = subject,
                Content = htmlMessage
            });
        }
    }
}
