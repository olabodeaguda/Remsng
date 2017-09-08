using System;

namespace RemsNG.ORM
{
    public class Street : AbstractModel
    {
        public Guid id { get; set; }
        public Guid wardId { get; set; }
        public string streetName { get; set; }
        public int numberOfHouse { get; set; }
    }
}