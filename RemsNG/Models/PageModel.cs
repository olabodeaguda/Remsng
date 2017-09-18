using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class PageModel
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public  int totalPageCount { get; set; }
    }
}
