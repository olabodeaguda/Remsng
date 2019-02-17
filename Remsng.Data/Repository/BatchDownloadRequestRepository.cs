using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class BatchDownloadRequestRepository : AbstractRepository
    {
        public BatchDownloadRequestRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            db.Set<BatchDownloadRequest>().Add(new BatchDownloadRequest
            {
                BatchFileName = bdnModel.batchFileName,
                BatchNo = bdnModel.batchNo,
                Createdby = bdnModel.createdBy,
                DateCreated = bdnModel.dateCreated.Value,
                Id = bdnModel.id,
                Lastmodifiedby = bdnModel.lastmodifiedby,
                LastModifiedDate = bdnModel.lastModifiedDate.Value,
                LcdaId = bdnModel.lcdaId,
                RequestStatus = bdnModel.requestStatus
            });

            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Request as been intiated successfully"
            };
        }

        public async Task<Response> UpdateBatchRequest(BatchDemandNoticeModel model)
        {
            var result = await db.Set<BatchDownloadRequest>().FindAsync(model.id);
            result.RequestStatus = model.requestStatus;
            result.Createdby = model.createdBy;
            result.BatchFileName = model.batchFileName;

            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Request has been updated successfully"
            };
        }

        public async Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno)
        {
            return await db.Set<BatchDemandNoticeModel>()
                .Where(x => x.batchNo == batchno).OrderByDescending(x => x.dateCreated).ToListAsync(); // db.BatchDemanNoticeModels.FromSql($"select * from tbl_batchDownloadRequest where batchNo = '{batchno}'").ToListAsync();
        }

        public async Task<BatchDemandNoticeModel> Dequeue()
        {
            try
            {
                return await db.Set<BatchDemandNoticeModel>().FromSql("sp_dequeueBatchDownload").FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
