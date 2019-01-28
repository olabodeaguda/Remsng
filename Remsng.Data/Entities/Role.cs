using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_role")]
    public partial class Role
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public Guid DomainId { get; set; }
        public string RoleStatus { get; set; }

        public virtual Lcda Domain { get; set; }
    }
}
