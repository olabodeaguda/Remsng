﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDemandNoticeCharges
    {
        Task<decimal> getCharges(decimal amount, Guid lcdaId);
    }
}