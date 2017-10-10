using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Exceptions
{
    public class UserValidationException : Exception
    {
        public UserValidationException(string msg) : base(msg)
        {
        }
    }
}
