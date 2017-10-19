using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Exceptions
{
    public class GlobalExceptionHandler : Exception
    {
        public GlobalExceptionHandler(string msg) : base(msg)
        {

        }
    }
}
