using System.Net;

namespace MatchingJob.BLL
{
    public class APIResponse
    {
        public string Message { get; set; }
        public object Data { get; set; }

        public bool Success { get; set; }
    }
}
