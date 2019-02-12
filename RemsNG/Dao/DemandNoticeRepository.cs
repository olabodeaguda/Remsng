using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticeRepository : AbstractRepository
    {
        private StreetRepository streetDao;
        WardRepository wardDao;
        public DemandNoticeRepository(RemsDbContext _db) : base(_db)
        {
            wardDao = new WardRepository(_db);
            streetDao = new StreetRepository(_db);
        }

        public async Task<Response> Add(DemandNotice demandNotice)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNotice @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9", new object[] {
                demandNotice.Id,
                demandNotice.Query,
                demandNotice.BatchNo,
                demandNotice.DemandNoticeStatus,
                demandNotice.BillingYear,
                demandNotice.LcdaId,
                demandNotice.CreatedBy,
                demandNotice.WardId,
                demandNotice.StreetId,
                demandNotice.IsUnbilled
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
            List<DemandNotice> results = await db.DemandNotices
                .FromSql("sp_searchdemandNoticePaginated @p0,@p1,@p2",
                new object[] {
                    pageModel.PageNum,
                    pageModel.PageSize,
                    query.Query
                }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNotice demandNotice = results[0];
                totalCount = 0;// demandNotice.TotalSize.HasValue ? demandNotice.TotalSize.Value : 1;
            }

            List<DemandNoticeModelExt> lst = new List<DemandNoticeModelExt>();
            foreach (var x in results)
            {
                DemandNoticeModelExt dne = new DemandNoticeModelExt();
                dne.batchNo = x.BatchNo;
                dne.billingYear = x.BillingYear;
                dne.demandNoticeStatus = x.DemandNoticeStatus;
                dne.id = x.Id;
                dne.lcdaId = x.LcdaId;
                dne.demandNoticeRequestModel = await TranslateDemandNoticeRequest(x.Query);
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
                    query.WardId,
                    query.StreetId
                }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNotice demandNotice = results[0];
                totalCount = 0;// demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }

            List<DemandNoticeModelExt> lst = new List<DemandNoticeModelExt>();
            foreach (var x in results)
            {
                DemandNoticeModelExt dne = new DemandNoticeModelExt();
                dne.batchNo = x.BatchNo;
                dne.billingYear = x.BillingYear;
                dne.demandNoticeStatus = x.DemandNoticeStatus;
                dne.id = x.Id;
                dne.lcdaId = x.LcdaId;
                dne.demandNoticeRequestModel = await TranslateDemandNoticeRequest(x.Query);
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
                demandNotice.Id,
                demandNotice.Query
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
                demandNotice.Id,
                demandNotice.BillingYear
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
                demandNotice.Id,
                demandNotice.DemandNoticeStatus
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
                totalCount = 0;// demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }
            List<DemandNoticeModelExt> lst = new List<DemandNoticeModelExt>();
            foreach (var x in results)
            {
                DemandNoticeModelExt dne = new DemandNoticeModelExt();
                dne.batchNo = x.BatchNo;
                dne.billingYear = x.BillingYear;
                dne.demandNoticeStatus = x.DemandNoticeStatus;
                dne.id = x.Id;
                dne.lcdaId = x.LcdaId;
                dne.demandNoticeRequestModel = await TranslateDemandNoticeRequest(x.Query);
                dne.CreatedDate = x.DateCreated.Value;
                lst.Add(dne);
            }

            return new
            {
                data = lst,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        private async Task<DemandNoticeRequestModel> TranslateDemandNoticeRequest(string jsonObject)
        {
            DemandNoticeRequestModel s = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(jsonObject));
            if (s.wardId != null)
            {
                Ward ward = await wardDao.GetWard(s.wardId.Value);
                if (ward != null)
                {
                    s.wardName = ward.WardName;
                }
            }
            if (s.streetId != null)
            {
                Street street = await streetDao.ById(s.streetId.Value);
                if (street != null)
                {
                    s.streetName = street.StreetName;
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
                totalCount = 0;// demandNotice.totalSize.HasValue ? demandNotice.totalSize.Value : 1;
            }

            return new
            {
                data = results.Select(x => new DemandNoticeModelExt()
                {
                    batchNo = x.BatchNo,
                    billingYear = x.BillingYear,
                    demandNoticeStatus = x.DemandNoticeStatus,
                    id = x.Id,
                    lcdaId = x.LcdaId,
                    demandNoticeRequestModel = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(x.Query))
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
            catch (Exception)
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
            catch (Exception)
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
