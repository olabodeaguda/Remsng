using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemanNoticeManager : IDemandNoticeManagers
    {
        private LcdaPropertyRepository _lcdaPropertyRepo;
        private StateRepository _stateRepository;
        private AddressRepository _addressRepository;
        private ImageRepository _imagesRepository;
        private TaxpayerRepository _taxpayerRepository;
        private StreetRepository _streetRepository;
        private WardRepository _wardRepository;
        private DemandNoticeRepository demandNoticeDao;
        private DemandNoticeArrearRepository dnaDao;
        private DemandNoticeTaxpayersRepository _dnTaxpayerRepo;
        public DemanNoticeManager(DbContext db)
        {
            demandNoticeDao = new DemandNoticeRepository(db);
            dnaDao = new DemandNoticeArrearRepository(db);
            _dnTaxpayerRepo = new DemandNoticeTaxpayersRepository(db);
            _wardRepository = new WardRepository(db);
            _streetRepository = new StreetRepository(db);
            _taxpayerRepository = new TaxpayerRepository(db);
            _imagesRepository = new ImageRepository(db);
            _addressRepository = new AddressRepository(db);
            _stateRepository = new StateRepository(db);
            _lcdaPropertyRepo = new LcdaPropertyRepository(db);
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

        public async Task<TaxPayerModel[]> ValidTaxpayers(DemandNoticeRequestModel model)
        {
            DemandNoticeTaxpayersModel[] dntModel = await _dnTaxpayerRepo.Search(model);
            TaxPayerModel[] taxPayers = await _taxpayerRepository.SearchByDNRequest(model, dntModel.Select(x => x.TaxpayerId).ToArray());
            return taxPayers;
        }

        public async Task<bool> AddDemanNotice(DemandNoticeRequestModel model)
        {
            if (model.lcdaId == default(Guid))
            {
                throw new InvalidCredentialsException("Request is invalid");
            }

            if (model.TaxpayerIds.Length <= 0)
            {
                throw new InvalidCredentialsException("Please select Taxpayer");
            }

            Dictionary<string, ImagesModel> images = (await _imagesRepository.ByOwnerId(model.lcdaId)).ToDictionary(x => x.ImgFilename);
            var lcdaAdd = (await _addressRepository.ByOwnersId(model.lcdaId)).FirstOrDefault();
            model.LcdaAddress = lcdaAdd == null ? string.Empty : $"{lcdaAdd.Addressnumber}, {lcdaAdd.StreetName}";
            var lcdastate = await _stateRepository.ByLcda(model.lcdaId);
            model.LcdaState = lcdastate == null ? string.Empty : lcdastate.StateName;
            var treasurerMobile = (await _lcdaPropertyRepo.ByLcda(model.lcdaId)).Select(x => x.PropertyValue).ToArray();
            model.TreasurerMobile = treasurerMobile.Length > 0 ? string.Join(';', treasurerMobile) : string.Empty;
            int batchNo = 0;
            var lastDN = await demandNoticeDao.GetLastEntry();
            if (lastDN != null)
            {
                string serial = lastDN.BatchNo.Substring(0, lastDN.BatchNo.Length);
                batchNo = int.Parse(serial);
            }

            DemandNoticeModel demandNotice = new DemandNoticeModel()
            {
                BillingYear = model.dateYear,
                CreatedBy = model.createdBy,
                DateCreated = DateTime.Now,
                Id = Guid.NewGuid(),
                DemandNoticeStatus = "SUBMITTED",
                PlainTextQuery = JsonConvert.SerializeObject(model),
                IsUnbilled = model.isUnbilled,
                LcdaId = model.lcdaId,
                StreetId = model.streetId,
                WardId = model.wardId,
                Query = Common.Utilities.EncryptDecryptUtils.ToHexString(JsonConvert.SerializeObject(model)),
                BatchNo = lastDN == null ? $"1{CommonList.GetBatchNo()}" : $"{batchNo + 1}{CommonList.GetBatchNo()}"
            };

            DemandNoticeTaxpayersModel[] dnTaxpayer = await _dnTaxpayerRepo.ConstructByTaxpayerIds(model, images);
            demandNotice.TaxpayerModel = dnTaxpayer.ToList();

            //add demandNotice

            return true;
        }
    }
}
