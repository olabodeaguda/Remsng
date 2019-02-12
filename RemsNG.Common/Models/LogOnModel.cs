using System;

namespace RemsNG.Common.Models
{
    public class LogOnModel
    {
        public string username { get; set; }
        public string userId { get; set; }
        public string shapass { get; set; }
        public DateTime logOnTime { get; private set; }
        public string domainCode { get; set; }
        public string[] permissions { get; set; }
    }
}
