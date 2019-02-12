using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class ImagesModel
    {
        public Guid id { get; set; }
        public string imgFilename { get; set; }
        public Guid ownerId { get; set; }
        public string imgType { get; set; }
    }
}
