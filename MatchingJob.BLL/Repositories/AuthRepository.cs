using MatchingJob.BLL.Authentication;
using MatchingJob.DAL.DTOs.User;
using MatchingJob.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingJob.BLL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _pwHasher;
        private readonly ITokenHelper _tokenHelper;
        public AuthRepository(AppDbContext context, IPasswordHasher pwhasher, ITokenHelper tokenHelper)
        {
            _context = context;
            _pwHasher = pwhasher;
            _tokenHelper = tokenHelper;
        }

        public async Task<(string token, DateTime expiration)> CreateJWT(LoginModel loginModel)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.UserName == loginModel.UserName);
                if (user != null)
                {
                    var (verified, needsUpgrade) = _pwHasher.Check(user.Password, loginModel.Password);
                    if (verified)
                    {
                        if (needsUpgrade)
                        {
                            user.Password = _pwHasher.Hash(loginModel.Password);
                            await _context.SaveChangesAsync();
                        }

                        return _tokenHelper.GenerateJWT(user.Id, user.UserName);
                    }
                }

                return (null, DateTime.MinValue);
            }
            catch
            {
                throw;
            }
        }
    }
}
