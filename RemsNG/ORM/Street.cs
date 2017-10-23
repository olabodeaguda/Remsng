using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.ORM
{
    public class Street : AbstractModel
    {
        public Guid id { get; set; }
        [ForeignKey("ward")]
        public Guid wardId { get; set; }
        public string streetName { get; set; }
        public int numberOfHouse { get; set; }
        public string streetDescription { get; set; }
        public string streetStatus { get; set; }

        public virtual Ward ward { get; set; }
    }
}