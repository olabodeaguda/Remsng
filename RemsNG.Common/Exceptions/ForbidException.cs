using System;

namespace RemsNG.Common.Exceptions
{
    public class ForbidException : Exception
    {
        public ForbidException(string msg) : base(msg)
        {
        }
    }
}
