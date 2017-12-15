using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class ImageExtensionModel: Images
    {
        public string imgBase64 { get; set; }
    }
}
