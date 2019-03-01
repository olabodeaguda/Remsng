using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class BatchDwnRequestManager : IBatchDwnRequestManager
    {
        private readonly BatchDownloadRequestRepository bnr;
        public BatchDwnRequestManager(DbContext _db)
        {
            bnr = new BatchDownloadRequestRepository(_db);
        }

        public async Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            return await bnr.AddBatchRequest(bdnModel);
        }

        public async Task<BatchDemandNoticeModel> Dequeue()
        {
            return await bnr.Dequeue();
        }

        public async Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno)
        {
            return await bnr.ListByBatchNo(batchno);
        }

        public async Task<Response> UpdateBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            return await bnr.UpdateBatchRequest(bdnModel);
        }
    }
}
