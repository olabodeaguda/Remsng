using RemsNG.Common.Utilities;
using System;

namespace RemsNG.Common.Exceptions
{
    public class ForbidException : BaseException
    {
        public ForbidException(string msg) : base(msg)
        {
            httpStatusCode = System.Net.HttpStatusCode.Forbidden;
            Code = MsgCode_Enum.FORBIDDEN;
        }
    }
}
