using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException(string msg) : base(msg)
        {
        
        }
    }
}
