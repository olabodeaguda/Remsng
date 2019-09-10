using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IBatchDownloadRequestRepository
    {
        Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel);
        Task<Response> UpdateBatchRequest(BatchDemandNoticeModel model);
        Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno);
        Task<BatchDemandNoticeModel> Dequeue();
    }
}
