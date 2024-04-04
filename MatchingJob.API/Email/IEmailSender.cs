namespace MatchingJob.API.Email
{
    public interface IEmailSender
    {
         Task sendEmail(Message message);
         Task sendEmailAsync(string email, string subject, string htmlMessage);
    }
}
