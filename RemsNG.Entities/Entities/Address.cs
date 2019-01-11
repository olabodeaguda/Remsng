using RemsNG.Common.Models;
using System;

namespace RemsNG.Data.Entities
{
    public class Address : BaseModel
    {
        public Guid id { get; set; }
        public string addressnumber { get; set; }
        public Guid streetId { get; set; } // add index
        public Guid ownerId { get; set; }
        public Guid lcdaid { get; set; }

    }
}
