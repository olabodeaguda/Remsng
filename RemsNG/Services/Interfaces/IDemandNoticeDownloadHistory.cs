﻿using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDemandNoticeDownloadHistory
    {
        Task Add(DemandNoticeDownloadHistory dndh);
    }
}