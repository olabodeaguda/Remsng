using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public partial class PermissionModel
    {
        public Guid Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
    }
}
