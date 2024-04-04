namespace MatchingJob.API.Email
{
    public interface IEmailSender
    {
        void sendEmail(Message message);
    }
}
