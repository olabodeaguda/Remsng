using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IBatchDwnRequestService
    {
        Task<Response> AddBatchRequest(BatchDownloadRequestModel bdnModel);
        Task<Response> UpdateBatchRequest(BatchDownloadRequestModel bdnModel);
        Task<List<BatchDownloadRequestModel>> ListByBatchNo(string batchno);
        Task<BatchDownloadRequestModel> Dequeue();
    }
}
