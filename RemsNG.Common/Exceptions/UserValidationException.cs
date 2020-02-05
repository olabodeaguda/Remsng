using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Exceptions
{
    public class UserValidationException : BaseException
    {
        public UserValidationException(string msg) : base(msg)
        {
            httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            Code = MsgCode_Enum.FAIL;
        }
    }
}
