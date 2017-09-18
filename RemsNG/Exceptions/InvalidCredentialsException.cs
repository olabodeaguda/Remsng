using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message): base(message)
        {

        }
        public InvalidCredentialsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
