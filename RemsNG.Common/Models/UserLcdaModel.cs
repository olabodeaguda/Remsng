using System;

namespace RemsNG.Common.Models
{
    public class UserLcdaModel
    {
        public Guid LgdaId { get; set; }
        public LcdaModel Lcda { get; set; }
        public UserModel User { get; set; }
        public Guid UserId { get; set; }
    }
}
