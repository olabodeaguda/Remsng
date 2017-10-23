using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.ORM
{
    public class Address : AbstractModel
    {
        public Guid id { get; set; }
        public string addressnumber { get; set; }
        public Guid streetId { get; set; } // add index
        public Guid ownerId { get; set; }
        public Guid lcdaid { get; set; }

        public string streetName { get; set; }
    }
}
