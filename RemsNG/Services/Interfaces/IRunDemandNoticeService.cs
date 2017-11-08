using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IRunDemandNoticeService
    {
        Task RegisterTaxpayer(DemandNoticeRequest demandNoticeRequest);
    }
}
