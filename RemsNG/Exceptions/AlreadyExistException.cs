﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException(string msg) : base(msg)
        {
        }
    }
}
