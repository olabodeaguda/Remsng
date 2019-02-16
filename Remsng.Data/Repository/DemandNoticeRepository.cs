using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeRepository : AbstractRepository
    {
        private StreetRepository streetDao;
        WardRepository wardDao;
        public DemandNoticeRepository(DbContext _db) : base(_db)
        {
            wardDao = new WardRepository(_db);
            streetDao = new StreetRepository(_db);
        }

        public async Task<Response> Add(DemandNoticeModel demandNotice)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addDemandNotice @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9", new object[] {
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

        public async Task<object> SearchDemandNotice2(DemandNoticeModel query, PageModel pageModel)
        {
            List<DemandNotice> results = await db.Set<DemandNotice>()
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

        public async Task<object> SearchDemandNotice(DemandNoticeModel query, PageModel pageModel)
        {
            List<DemandNotice> results = await db.Set<DemandNotice>()
                .FromSql("sp_searchdemandNoticePaginated2 @p0,@p1,@p2,@p3",
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


        public async Task<Response> UpdateQuery(DemandNoticeModel demandNotice)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_updateQueryDemandNotice @p0,@p1", new object[] {
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

        public async Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_updateBillingYrDemandNotice @p0,@p1", new object[] {
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

        public async Task<Response> UpdateStatus(DemandNoticeModel demandNotice)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_updateStatusDemandNotice @p0,@p1", new object[] {
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
            List<DemandNotice> results = await db.Set<DemandNotice>()
                .FromSql("sp_demandNoticeByLcda @p0,@p1, @p2", new object[] { lcdaId, pageModel.PageNum, pageModel.PageSize }).ToListAsync();
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
                WardModel ward = await wardDao.GetWard(s.wardId.Value);
                if (ward != null)
                {
                    s.wardName = ward.WardName;
                }
            }
            if (s.streetId != null)
            {
                StreetModel street = await streetDao.ById(s.streetId.Value);
                if (street != null)
                {
                    s.streetName = street.StreetName;
                }
            }
            return s;
        }

        public async Task<object> All(PageModel pageModel)
        {
            List<DemandNotice> results = await db.Set<DemandNotice>()
                .FromSql("sp_demandNoticePaginated @p0,@p1", new object[] { pageModel.PageNum, pageModel.PageSize }).ToListAsync();
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

        public async Task<DemandNoticeModel> GetById(Guid id)
        {
            var result = await db.Set<DemandNotice>()
                .FromSql("sp_getDemandNotice @p0", new object[] { id }).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return new DemandNoticeModel()
            {
                BatchNo = result.BatchNo,
                BillingYear = result.BillingYear,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DemandNoticeStatus = result.DemandNoticeStatus,
                Id = result.Id,
                IsUnbilled = result.IsUnbilled,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaId = result.LcdaId,
                Query = result.Query,
                StreetId = result.StreetId,
                WardId = result.WardId
            };
        }

        public async Task<DemandNoticeModel> GetByBatchId(string batchId)
        {
            try
            {
                var result = await db.Set<DemandNotice>()
                    .FromSql("sp_getDemandNoticeByBatchId @p0", new object[] { batchId }).FirstOrDefaultAsync();
                return new DemandNoticeModel()
                {
                    BatchNo = result.BatchNo,
                    BillingYear = result.BillingYear,
                    CreatedBy = result.CreatedBy,
                    DateCreated = result.DateCreated,
                    DemandNoticeStatus = result.DemandNoticeStatus,
                    Id = result.Id,
                    IsUnbilled = result.IsUnbilled,
                    Lastmodifiedby = result.Lastmodifiedby,
                    LastModifiedDate = result.LastModifiedDate,
                    LcdaId = result.LcdaId,
                    Query = result.Query,
                    StreetId = result.StreetId,
                    WardId = result.WardId
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DemandNoticeModel> DequeueDemandNotice()
        {
            try
            {

                var result = await db.Set<DemandNotice>()
                    .FromSql("sp_dequeueDemandNotice").FirstOrDefaultAsync();
                return new DemandNoticeModel()
                {
                    BatchNo = result.BatchNo,
                    BillingYear = result.BillingYear,
                    CreatedBy = result.CreatedBy,
                    DateCreated = result.DateCreated,
                    DemandNoticeStatus = result.DemandNoticeStatus,
                    Id = result.Id,
                    IsUnbilled = result.IsUnbilled,
                    Lastmodifiedby = result.Lastmodifiedby,
                    LastModifiedDate = result.LastModifiedDate,
                    LcdaId = result.LcdaId,
                    Query = result.Query,
                    StreetId = result.StreetId,
                    WardId = result.WardId
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<DemandNoticeModel>> GetUnSyncData()
        {
            string query = $"select tbl_demandnotice.*,-1 as totalSize from tbl_demandnotice where wardId is null and demandNoticeStatus = 'ERROR'";
            var results = await db.Set<DemandNotice>()
                .FromSql(query).ToListAsync();
            return results.Select(result =>
                 new DemandNoticeModel()
                 {
                     BatchNo = result.BatchNo,
                     BillingYear = result.BillingYear,
                     CreatedBy = result.CreatedBy,
                     DateCreated = result.DateCreated,
                     DemandNoticeStatus = result.DemandNoticeStatus,
                     Id = result.Id,
                     IsUnbilled = result.IsUnbilled,
                     Lastmodifiedby = result.Lastmodifiedby,
                     LastModifiedDate = result.LastModifiedDate,
                     LcdaId = result.LcdaId,
                     Query = result.Query,
                     StreetId = result.StreetId,
                     WardId = result.WardId
                 }).ToList();
        }

        public async Task<bool> updateData(string query)
        {
            int count = await db.Database.ExecuteSqlCommandAsync(query);

            return count > 0;
        }

    }
}
