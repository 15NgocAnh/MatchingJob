using MatchingJob.DAL;
using MatchingJob.DAL.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingJob.BLL.Repositories
{
    public interface IAuthRepository
    {
        Task<(string token, DateTime expiration)> CreateJWT(LoginModel loginModel);
    }
}
