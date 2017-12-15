using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IBatchDwnRequestService
    {
        Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel);
        Task<Response> UpdateBatchRequest(BatchDemandNoticeModel bdnModel);
        Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno);
        Task<BatchDemandNoticeModel> Dequeue();
    }
}
