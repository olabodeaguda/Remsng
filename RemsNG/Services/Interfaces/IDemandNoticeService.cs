﻿using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDemandNoticeService
    {
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Response> UpdateStatus(DemandNotice demandNotice);
        Task<Response> UpdateBillingYr(DemandNotice demandNotice);
        Task<Response> UpdateQuery(DemandNotice demandNotice);
        Task<Response> Add(DemandNotice demandNotice);
        Task<DemandNotice> GetById(Guid id);
    }
}
