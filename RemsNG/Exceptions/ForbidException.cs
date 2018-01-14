using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Exceptions
{
    public class ForbidException : Exception
    {
        public ForbidException(string msg) : base(msg)
        {
        }
    }
}
