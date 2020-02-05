using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RemsNG.Common.Exceptions
{
    public class BaseException : Exception
    {
        public string Code { get; set; } = MsgCode_Enum.FAIL;
        public HttpStatusCode httpStatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public BaseException(string msg) : base(msg)
        {
        }
    }
}
