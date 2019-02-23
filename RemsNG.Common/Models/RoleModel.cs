using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public Guid DomainId { get; set; }
        public string RoleStatus { get; set; }
        public string domainName { get; set; }

        public virtual LcdaModel Domain { get; set; }
    }
}
