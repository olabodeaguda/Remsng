using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RemsNG.Dao
{
    public class DemandNoticeDao : AbstractDao
    {
        public DemandNoticeDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNotice @p0,@p1,@p2,@p3,@p4,@p5,@p6", new object[] {
                demandNotice.id,
                demandNotice.query,
                demandNotice.batchNo,
                demandNotice.demandNoticeStatus,
                demandNotice.billingYear,
                demandNotice.lcdaId,
                demandNotice.createdBy
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

        public async Task<Response> UpdateQuery(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateQueryDemandNotice @p0,@p1", new object[] {
                demandNotice.id,
                demandNotice.query
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

        public async Task<Response> UpdateBillingYr(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateBillingYrDemandNotice @p0,@p1", new object[] {
                demandNotice.id,
                demandNotice.billingYear
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

        public async Task<Response> UpdateStatus(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateStatusDemandNotice @p0,@p1", new object[] {
                demandNotice.id,
                demandNotice.demandNoticeStatus
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

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            List<DemandNotice> results = await db.DemandNotices.FromSql("sp_demandNoticeByLcda @p0,@p1, @p2", new object[] { lcdaId, pageModel.PageNum, pageModel.PageSize }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNotice demandNotice = results[0];
                totalCount = demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }

            return new
            {
                data = results.Select(x => new DemandNoticeExt()
                {
                    batchNo = x.batchNo,
                    billingYear = x.billingYear,
                    demandNoticeStatus = x.demandNoticeStatus,
                    id = x.id,
                    lcdaId = x.lcdaId,
                    demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequest>(EncryptDecryptUtils.FromHexString(x.query))
                }),
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<DemandNotice> GetById(Guid id)
        {
            return await db.DemandNotices.FromSql("sp_getDemandNotice @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<DemandNotice> DequeueDemandNotice()
        {
            try
            {
                return await db.DemandNotices.FromSql("sp_dequeueDemandNotice").FirstOrDefaultAsync();
            }
            catch (Exception x)
            {
                throw;
            }
        }
    }
}
