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
        private StreetDao streetDao;
        WardDao wardDao;
        public DemandNoticeDao(RemsDbContext _db) : base(_db)
        {
            wardDao = new WardDao(_db);
            streetDao = new StreetDao(_db);
        }

        public async Task<Response> Add(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNotice @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8", new object[] {
                demandNotice.id,
                demandNotice.query,
                demandNotice.batchNo,
                demandNotice.demandNoticeStatus,
                demandNotice.billingYear,
                demandNotice.lcdaId,
                demandNotice.createdBy,demandNotice.wardId,demandNotice.streetId
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

        public async Task<object> SearchDemandNotice2(DemandNotice query, PageModel pageModel)
        {
            List<DemandNotice> results = await db.DemandNotices.FromSql("sp_searchdemandNoticePaginated @p0,@p1,@p2",
                new object[] {
                    pageModel.PageNum,
                    pageModel.PageSize,
                    query.query
                }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNotice demandNotice = results[0];
                totalCount = demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }

            List<DemandNoticeExt> lst = new List<DemandNoticeExt>();
            foreach (var x in results)
            {
                DemandNoticeExt dne = new DemandNoticeExt();
                dne.batchNo = x.batchNo;
                dne.billingYear = x.billingYear;
                dne.demandNoticeStatus = x.demandNoticeStatus;
                dne.id = x.id;
                dne.lcdaId = x.lcdaId;
                dne.demandNoticeRequest = await TranslateDemandNoticeRequest(x.query);
                lst.Add(dne);
            }

            return new
            {
                data = lst,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<object> SearchDemandNotice(DemandNotice query, PageModel pageModel)
        {
            List<DemandNotice> results = await db.DemandNotices.FromSql("sp_searchdemandNoticePaginated2 @p0,@p1,@p2,@p3",
                new object[] {
                    pageModel.PageNum,
                    pageModel.PageSize,
                    query.wardId,
                    query.streetId
                }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNotice demandNotice = results[0];
                totalCount = demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }

            List<DemandNoticeExt> lst = new List<DemandNoticeExt>();
            foreach (var x in results)
            {
                DemandNoticeExt dne = new DemandNoticeExt();
                dne.batchNo = x.batchNo;
                dne.billingYear = x.billingYear;
                dne.demandNoticeStatus = x.demandNoticeStatus;
                dne.id = x.id;
                dne.lcdaId = x.lcdaId;
                dne.demandNoticeRequest = await TranslateDemandNoticeRequest(x.query);
                lst.Add(dne);
            }

            return new
            {
                data = lst,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
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
            List<DemandNoticeExt> lst = new List<DemandNoticeExt>();
            foreach (var x in results)
            {
                DemandNoticeExt dne = new DemandNoticeExt();
                dne.batchNo = x.batchNo;
                dne.billingYear = x.billingYear;
                dne.demandNoticeStatus = x.demandNoticeStatus;
                dne.id = x.id;
                dne.lcdaId = x.lcdaId;
                dne.demandNoticeRequest = await TranslateDemandNoticeRequest(x.query);
                dne.CreatedDate = x.dateCreated.Value;
                lst.Add(dne);
            }

            return new
            {
                data = lst,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        private async Task<DemandNoticeRequest> TranslateDemandNoticeRequest(string jsonObject)
        {
            DemandNoticeRequest s = JsonConvert.DeserializeObject<DemandNoticeRequest>(EncryptDecryptUtils.FromHexString(jsonObject));
            if (s.wardId != null)
            {
                Ward ward = await wardDao.GetWard(s.wardId.Value);
                if (ward != null)
                {
                    s.wardName = ward.wardName;
                }
            }
            if (s.streetId != null)
            {
                Street street = await streetDao.ById(s.streetId.Value);
                if (street != null)
                {
                    s.streetName = street.streetName;
                }
            }
            return s;
        }

        public async Task<object> All(PageModel pageModel)
        {
            List<DemandNotice> results = await db.DemandNotices.FromSql("sp_demandNoticePaginated @p0,@p1", new object[] { pageModel.PageNum, pageModel.PageSize }).ToListAsync();
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

        public async Task<DemandNotice> GetByBatchId(string batchId)
        {
            try
            {
                return await db.DemandNotices.FromSql("sp_getDemandNoticeByBatchId @p0", new object[] { batchId }).FirstOrDefaultAsync();
            }
            catch (Exception x)
            {

                throw;
            }
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


        public async Task<List<DemandNotice>> GetUnSyncData()
        {
            string query = $"select tbl_demandnotice.*,-1 as totalSize from tbl_demandnotice where wardId is null and demandNoticeStatus = 'ERROR'";
            return await db.DemandNotices.FromSql(query).ToListAsync();
        }

        public async Task<bool> updateData(string query)
        {
            int count = await db.Database.ExecuteSqlCommandAsync(query);

            return count > 0;
        }

    }
}
