using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class ErrorModel
    {
        public Guid Id { get; set; }
        public string ErrorType { get; set; }
        public string Errorvalue { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid OwnerId { get; set; }
    }
}
