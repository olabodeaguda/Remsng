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
    public class DemanNoticeService : IDemandNoticeService
    {
        private DemandNoticeDao demandNoticeDao;
        private DemandNoticeArrearDao dnaDao;
        public DemanNoticeService(RemsDbContext db)
        {
            demandNoticeDao = new DemandNoticeDao(db);
            dnaDao = new DemandNoticeArrearDao(db);
        }

        public async Task<Response> Add(DemandNotice demandNotice)
        {
            return await demandNoticeDao.Add(demandNotice);
        }

        public async Task<object> SearchDemandNotice(DemandNotice demandNotice,PageModel pageModel)
        {
            return await demandNoticeDao.SearchDemandNotice(demandNotice, pageModel);
        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await demandNoticeDao.ByLcdaId(lcdaId, pageModel);
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await demandNoticeDao.All(pageModel);
        }

        public async Task<DemandNotice> GetById(Guid id)
        {
            return await demandNoticeDao.GetById(id);
        }

        public async Task<Response> UpdateBillingYr(DemandNotice demandNotice)
        {
            return await demandNoticeDao.UpdateBillingYr(demandNotice);
        }

        public async Task<Response> UpdateQuery(DemandNotice demandNotice)
        {
            return await demandNoticeDao.UpdateQuery(demandNotice);
        }

        public async Task<Response> UpdateStatus(DemandNotice demandNotice)
        {
            return await demandNoticeDao.UpdateStatus(demandNotice);
        }

        public async Task<DemandNotice> GetByBatchId(string batchId)
        {
            return await demandNoticeDao.GetByBatchId(batchId);
        }

        public async Task<bool> AddArrears(DemandNoticeArrears dna)
        {
            return await dnaDao.AddArrears(dna);
        }
    }
}
