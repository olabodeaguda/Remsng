using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_userdomain")]
    public class UserDomain
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Guid DomainId { get; set; }
        public Domain Domain { get; set; }
    }
}
