﻿using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Exceptions
{
    public class NotFoundException: BaseException
    {
        public NotFoundException(string message):base(message)
        {
            httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            Code = MsgCode_Enum.NOTFOUND;
        }
    }
}
