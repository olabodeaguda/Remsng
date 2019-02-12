using System;

namespace RemsNG.Common.Models
{
    public class DbResponse
    {
        public Guid id { get; set; }
        public string msg { get; set; }
        public bool success { get; set; }
    }
}
