using System.ComponentModel.DataAnnotations;

namespace MatchingJob.DAL.DTOs.User
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required] public string? Password { get; set; }

    }
}
