using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Exceptions
{
    public class AlreadyExistException : BaseException
    {
        public AlreadyExistException(string msg) : base(msg)
        {
            httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            Code = MsgCode_Enum.FAIL;
        }
    }
}
