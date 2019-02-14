using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IBatchDwnRequestManagers
    {
        Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel);
        Task<Response> UpdateBatchRequest(BatchDemandNoticeModel bdnModel);
        Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno);
        Task<BatchDemandNoticeModel> Dequeue();
    }
}
