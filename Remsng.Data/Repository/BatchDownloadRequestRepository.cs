using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class BatchDownloadRequestRepository : IBatchDownloadRequestRepository
    {
        private readonly DbContext db;
        public BatchDownloadRequestRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            try
            {
                BatchDownloadRequest dnModel = new BatchDownloadRequest
                {
                    BatchFileName = bdnModel.batchFileName,
                    BatchNo = bdnModel.batchNo,
                    Createdby = bdnModel.createdBy,
                    DateCreated = DateTime.Now,
                    Id = bdnModel.id,
                    LcdaId = bdnModel.lcdaId,
                    RequestStatus = bdnModel.requestStatus
                };

                db.Set<BatchDownloadRequest>().Add(dnModel);

                await db.SaveChangesAsync();
            }
            catch (Exception x)
            {

                throw;
            }

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
            return await db.Set<BatchDownloadRequest>()
                .Where(x => x.BatchNo == batchno).Select(x => new BatchDemandNoticeModel
                {
                    batchFileName = x.BatchFileName,
                    batchNo = x.BatchNo,
                    createdBy = x.Createdby,
                    dateCreated = x.DateCreated,
                    id = x.Id,
                    lastmodifiedby = x.Lastmodifiedby,
                    lastModifiedDate = x.LastModifiedDate,
                    lcdaId = x.LcdaId,
                    requestStatus = x.RequestStatus
                }).OrderByDescending(x => x.dateCreated).ToListAsync(); // db.BatchDemanNoticeModels.FromSql($"select * from tbl_batchDownloadRequest where batchNo = '{batchno}'").ToListAsync();
        }

        public async Task<BatchDemandNoticeModel> Dequeue()
        {
            try
            {
                return await db.Set<BatchDownloadRequest>().FromSql("sp_dequeueBatchDownload").Select(x => new BatchDemandNoticeModel
                {
                    batchFileName = x.BatchFileName,
                    batchNo = x.BatchNo,
                    createdBy = x.Createdby,
                    dateCreated = x.DateCreated,
                    id = x.Id,
                    lastmodifiedby = x.Lastmodifiedby,
                    lastModifiedDate = x.LastModifiedDate,
                    lcdaId = x.LcdaId,
                    requestStatus = x.RequestStatus
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
