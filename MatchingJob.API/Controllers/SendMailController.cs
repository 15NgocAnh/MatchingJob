using MatchingJob.API.Email;
using MatchingJob.Data;
using Microsoft.AspNetCore.Mvc;

namespace MatchingJob.API.Controllers
{
    public class SendMailController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public SendMailController(AppDbContext context, IEmailSender emailSender) {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async void sendEmail(Message message)
        {
            await _emailSender.sendEmail(message);
        }
    }
}
