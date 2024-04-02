using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingJob.BLL.Authentication
{
    public sealed class TokenSettings
    {
        public string Secret { get; set; }
        public int AccessExpirationInMinutes { get; set; }
    }
}
