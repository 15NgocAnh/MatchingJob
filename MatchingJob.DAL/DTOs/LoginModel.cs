using System.ComponentModel.DataAnnotations;

namespace MatchingJob.DAL
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]public string? Password { get; set; }

    }
}
