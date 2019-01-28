using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_userlcda")]
    public class UserLcda
    {
        public Guid LgdaId { get; set; }
        public Lcda Lcda { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
