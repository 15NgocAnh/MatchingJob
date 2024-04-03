using MatchingJob.BLL.Repositories;
using MatchingJob.DAL.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace MatchingJob.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IAuthRepository _authRepository;

        public AuthController(ILogger<UsersController> logger, IAuthRepository authRepository)
        {
            _logger = logger;
            _authRepository = authRepository;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken(LoginModel loginModel)
        {
            (string jwt, DateTime expiration) = await _authRepository.CreateJWT(loginModel);
            if (string.IsNullOrWhiteSpace(jwt))
                return BadRequest(new { message = "Invalid Username or Password" });
            return Ok(new { jwt, expiration });
        }
    }
}
