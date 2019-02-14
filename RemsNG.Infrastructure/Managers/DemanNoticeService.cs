using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class DemanNoticeManagers : IDemandNoticeManagers
    {
        private DemandNoticeRepository demandNoticeDao;
        private DemandNoticeArrearRepository dnaDao;
        public DemanNoticeManagers(RemsDbContext db)
        {
            demandNoticeDao = new DemandNoticeRepository(db);
            dnaDao = new DemandNoticeArrearRepository(db);
        }

        public async Task<Response> Add(DemandNoticeModel demandNotice)
        {
            return await demandNoticeDao.Add(demandNotice);
        }

        public async Task<object> SearchDemandNotice(DemandNoticeModel demandNotice, PageModel pageModel)
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

        public async Task<DemandNoticeModel> GetById(Guid id)
        {
            return await demandNoticeDao.GetById(id);
        }

        public async Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice)
        {
            return await demandNoticeDao.UpdateBillingYr(demandNotice);
        }

        public async Task<Response> UpdateQuery(DemandNoticeModel demandNotice)
        {
            return await demandNoticeDao.UpdateQuery(demandNotice);
        }

        public async Task<Response> UpdateStatus(DemandNoticeModel demandNotice)
        {
            return await demandNoticeDao.UpdateStatus(demandNotice);
        }

        public async Task<DemandNoticeModel> GetByBatchId(string batchId)
        {
            return await demandNoticeDao.GetByBatchId(batchId);
        }

        public async Task<bool> AddArrears(DemandNoticeArrearsModel dna)
        {
            return await dnaDao.AddArrears(dna);
        }
    }
}
