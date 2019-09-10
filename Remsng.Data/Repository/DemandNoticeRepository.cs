using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemsNG.Common.Exceptions;
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
    public class DemandNoticeRepository : IDemandNoticeRepository
    {
        private IStreetRepository streetDao;
        private IWardRepository wardDao;
        private readonly DbContext db;
        public DemandNoticeRepository(DbContext _db,
            IWardRepository wardRepository, IStreetRepository streetRepository)
        {
            db = _db;
            wardDao = wardRepository;
            streetDao = streetRepository;
        }

        public async Task<Response> Add(DemandNoticeModel demandNotice)
        {
            db.Set<DemandNotice>().Add(new DemandNotice()
            {
                BatchNo = demandNotice.BatchNo,
                BillingYear = demandNotice.BillingYear,
                CreatedBy = demandNotice.CreatedBy,
                DateCreated = DateTime.Now,
                Id = demandNotice.Id,
                DemandNoticeStatus = demandNotice.DemandNoticeStatus,
                LcdaId = demandNotice.LcdaId,
                StreetId = demandNotice.StreetId,
                WardId = demandNotice.WardId,
                IsUnbilled = demandNotice.IsUnbilled,
                Query = demandNotice.Query,
            });

            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Demand notice has been created"
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

            List<DemandNoticeModel> lst = new List<DemandNoticeModel>();
            foreach (var x in results)
            {
                DemandNoticeModel dne = new DemandNoticeModel();
                dne.BatchNo = x.BatchNo;
                dne.BillingYear = x.BillingYear;
                dne.DemandNoticeStatus = x.DemandNoticeStatus;
                dne.Id = x.Id;
                dne.LcdaId = x.LcdaId;
                dne.DemandNoticeRequest = await TranslateDemandNoticeRequest(x.Query);
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
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_updateQueryDemandNotice @p0,@p1", new object[] {
            //    demandNotice.Id,
            //    demandNotice.Query
            //}).FirstOrDefaultAsync();

            var entity = await db.Set<DemandNotice>().FindAsync(demandNotice.Id);
            if (entity == null)
            {
                throw new NotFoundException("Demand Notice can not be found");
            }

            entity.Query = demandNotice.Query;
            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Demand Notice has been updated successfully"
            };

        }

        public async Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_updateBillingYrDemandNotice @p0,@p1", new object[] {
            //    demandNotice.Id,
            //    demandNotice.BillingYear
            //}).FirstOrDefaultAsync();
            var entity = await db.Set<DemandNotice>().FindAsync(demandNotice.Id);
            if (entity == null)
            {
                throw new NotFoundException("Demand Notice can not be found");
            }

            entity.BillingYear = demandNotice.BillingYear;
            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Demand Notice billing year has been updated successfully"
            };

        }

        public async Task<Response> UpdateStatus(DemandNoticeModel demandNotice)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_updateStatusDemandNotice @p0,@p1", new object[] {
            //    demandNotice.Id,
            //    demandNotice.DemandNoticeStatus
            //}).FirstOrDefaultAsync();

            var entity = await db.Set<DemandNotice>().FindAsync(demandNotice.Id);
            if (entity == null)
            {
                throw new NotFoundException("Demand Notice can not be found");
            }

            entity.DemandNoticeStatus = demandNotice.DemandNoticeStatus;
            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Demand Notice has been added successfully"
            };
        }

        public async Task<PageModel<List<DemandNoticeModel>>> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var totalCount = 0;
            var query = db.Set<DemandNotice>()
                .Include(x => x.Ward)
                .ThenInclude(d => d.Street)
                .Where(x => x.LcdaId == lcdaId);
            totalCount = await query.CountAsync();

            var ty = await query.ToListAsync();
            var results = await query.Select(x => new DemandNoticeModel
            {
                BatchNo = x.BatchNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Query = x.Query,
                StreetId = x.Street != null ? x.StreetId.Value : default(Guid),
                WardId = x.WardId.Value
            }).OrderByDescending(d => d.DateCreated).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToArrayAsync();

            List<DemandNoticeModel> lst = new List<DemandNoticeModel>();
            foreach (var tm in results)
            {
                tm.DemandNoticeRequest = await TranslateDemandNoticeRequest(tm.Query);
                lst.Add(tm);
            }

            return new PageModel<List<DemandNoticeModel>>
            {
                data = lst,
                totalPageCount = int.Parse(Math.Round((totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)).ToString())
            };
        }

        public async Task<object> All(PageModel pageModel)
        {
            //List<DemandNotice> results = await db.Set<DemandNotice>()
            //    .FromSql("sp_demandNoticePaginated @p0,@p1", new object[] { pageModel.PageNum, pageModel.PageSize }).ToListAsync();
            //var totalCount = 0;
            //if (results.Count > 0)
            //{
            //    DemandNotice demandNotice = results[0];
            //    totalCount = 0;
            //}

            var totalCount = 0;
            var query = db.Set<DemandNotice>();
            totalCount = await query.CountAsync();

            var results = query.Select(x => new DemandNoticeModel
            {
                BatchNo = x.BatchNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Query = x.Query,
                StreetId = x.StreetId.Value,
                WardId = x.WardId.Value
            }).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToArray();


            return new PageModel<DemandNoticeModel[]>
            {
                data = results.Select(x =>
                {
                    x.DemandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(x.Query));
                    return x;
                }).ToArray(),
                totalPageCount = int.Parse(Math.Round((totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)).ToString())
            };

            //return new
            //{
            //    data = results.Select(x => new DemandNoticeModelExt()
            //    {
            //        batchNo = x.BatchNo,
            //        billingYear = x.BillingYear,
            //        demandNoticeStatus = x.DemandNoticeStatus,
            //        id = x.Id,
            //        lcdaId = x.LcdaId,
            //        demandNoticeRequestModel = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(x.Query))
            //    }),
            //    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            //};
        }

        public async Task<DemandNoticeModel> GetById(Guid id)
        {
            //var result = await db.Set<DemandNotice>()
            //    .FromSql("sp_getDemandNotice @p0", new object[] { id }).FirstOrDefaultAsync();
            //if (result == null)
            //{
            //    return null;
            //}
            //return new DemandNoticeModel()
            //{
            //    BatchNo = result.BatchNo,
            //    BillingYear = result.BillingYear,
            //    CreatedBy = result.CreatedBy,
            //    DateCreated = result.DateCreated,
            //    DemandNoticeStatus = result.DemandNoticeStatus,
            //    Id = result.Id,
            //    IsUnbilled = result.IsUnbilled,
            //    Lastmodifiedby = result.Lastmodifiedby,
            //    LastModifiedDate = result.LastModifiedDate,
            //    LcdaId = result.LcdaId,
            //    Query = result.Query,
            //    StreetId = result.StreetId,
            //    WardId = result.WardId
            //};
            var x = await db.Set<DemandNotice>().FindAsync(id);

            return new DemandNoticeModel
            {
                BatchNo = x.BatchNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Query = x.Query,
                StreetId = x.StreetId.Value,
                WardId = x.WardId.Value,
                DemandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(x.Query))
            };
        }

        public async Task<DemandNoticeModel> GetByBatchId(string batchId)
        {
            var x = await db.Set<DemandNotice>().FirstOrDefaultAsync(p => p.BatchNo == batchId);

            return new DemandNoticeModel
            {
                BatchNo = x.BatchNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Query = x.Query,
                StreetId = x.StreetId.Value,
                WardId = x.WardId.Value,
                DemandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(x.Query),
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore })
            };
        }

        public async Task<DemandNoticeModel> GetLastEntry()
        {
            var result = await db.Set<DemandNotice>().OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
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
                StreetId = result.StreetId.Value,
                WardId = result.WardId.Value
            };
        }



        private async Task<DemandNoticeRequestModel> TranslateDemandNoticeRequest(string jsonObject)
        {
            string r = EncryptDecryptUtils.FromHexString(jsonObject);
            DemandNoticeRequestModel s = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(EncryptDecryptUtils.FromHexString(jsonObject), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            if (s.wardId != null)
            {
                WardModel ward = await wardDao.GetWard(s.wardId);
                if (ward != null)
                {
                    s.wardName = ward.WardName;
                }
            }
            if (s.streetId != null)
            {
                StreetModel street = await streetDao.ById(s.streetId);
                if (street != null)
                {
                    s.streetName = street.StreetName;
                }
            }
            return s;
        }

    }
}
