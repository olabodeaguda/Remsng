using System;

namespace RemsNG.Common.Models
{
    public class RoleExtensionModel
    {
        public Guid id { get; set; }
        public string roleName { get; set; }
        public Guid domainId { get; set; }
        public string roleStatus { get; set; }
        public string domainName { get; set; }
    }
}
