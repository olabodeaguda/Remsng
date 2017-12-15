using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class BatchDwnRequestService : IBatchDwnRequestService
    {
        private readonly BatchDownloadRequestDao bnr;
        public BatchDwnRequestService(RemsDbContext _db)
        {
            bnr = new BatchDownloadRequestDao(_db);
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
