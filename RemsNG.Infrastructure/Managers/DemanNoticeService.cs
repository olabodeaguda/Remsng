using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemanNoticeManagers : IDemandNoticeManagers
    {
        private StreetRepository _streetRepository;
        private WardRepository _wardRepository;
        private DemandNoticeRepository demandNoticeDao;
        private DemandNoticeArrearRepository dnaDao;
        private DemandNoticeTaxpayersRepository _dnTaxpayerRepo;
        public DemanNoticeManagers(DbContext db)
        {
            demandNoticeDao = new DemandNoticeRepository(db);
            dnaDao = new DemandNoticeArrearRepository(db);
            _dnTaxpayerRepo = new DemandNoticeTaxpayersRepository(db);
            _wardRepository = new WardRepository(db);
            _streetRepository = new StreetRepository(db);
        }

        public async Task<Response> Add(DemandNoticeModel demandNotice)
        {
            return await demandNoticeDao.Add(demandNotice);
        }

        public async Task<object> SearchDemandNotice(DemandNoticeModel query, PageModel pageModel)
        {

            return await demandNoticeDao.SearchDemandNotice(query, pageModel);
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

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchDemandNotice(DemandNoticeRequestModel rhModel, PageModel pageModel)
        {
            var result = await _dnTaxpayerRepo.Search(rhModel, pageModel);
            return result;
        }

        public async Task<DemandNoticeRequestModel> SearchInfo(DemandNoticeRequestModel model)
        {
            if (model.wardId != default(Guid))
            {
                WardModel wardModel = await _wardRepository.GetWard(model.wardId);
                if (wardModel != null)
                {
                    model.wardName = wardModel.WardName;
                }
            }

            if (model.streetId != default(Guid))
            {
                StreetModel streetModel = await _streetRepository.ById(model.streetId);
                if (streetModel != null)
                {
                    model.streetName = streetModel.StreetName;
                }
            }

            return model;
        }
    }
}
