using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class BatchDownloadRequestDao:AbstractDao
    {
        public BatchDownloadRequestDao(RemsDbContext _db):base(_db)
        {
        }

        public async Task<Response> AddBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addBatchDownloadRequest @p0,@p1,@p2,@p3", new object[] {
                bdnModel.batchNo,
                bdnModel.requestStatus,
                bdnModel.createdBy,
                bdnModel.lcdaId
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<Response> UpdateBatchRequest(BatchDemandNoticeModel bdnModel)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateBatchDownloadRequest @p0,@p1,@p2,@p3", new object[] {
                bdnModel.id,
                bdnModel.requestStatus,
                bdnModel.createdBy,
                bdnModel.batchFileName
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<List<BatchDemandNoticeModel>> ListByBatchNo(string batchno)
        {
            return await db.BatchDemanNoticeModels.Where(x=>x.batchNo == batchno).ToListAsync(); // db.BatchDemanNoticeModels.FromSql($"select * from tbl_batchDownloadRequest where batchNo = '{batchno}'").ToListAsync();
        }

        public async Task<BatchDemandNoticeModel> Dequeue()
        {
            try
            {
                return await db.BatchDemanNoticeModels.FromSql("sp_dequeueBatchDownload").FirstOrDefaultAsync();
            }
            catch (Exception x)
            {
                throw;
            }
        }
    }
}
