﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Exceptions
{
    public class DuplicateCompanyException : Exception
    {
        public DuplicateCompanyException(string msg) : base(msg)
        {

        }
    }
}
