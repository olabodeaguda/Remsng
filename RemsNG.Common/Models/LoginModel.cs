using System;

namespace RemsNG.Common.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string pwd { get; set; }
        public Guid domainId { get; set; }
    }
}
