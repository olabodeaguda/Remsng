using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemanNoticeManager : IDemandNoticeManager
    {
        private ILcdaPropertyRepository _lcdaPropertyRepo;
        private IStateRepository _stateRepository;
        private IAddressRepository _addressRepository;
        private IImageRepository _imagesRepository;
        private ITaxpayerRepository _taxpayerRepository;
        private IStreetRepository _streetRepository;
        private IWardRepository _wardRepository;
        private IDemandNoticeRepository demandNoticeDao;
        private IDemandNoticeArrearRepository dnaDao;
        private IDemandNoticeTaxpayersRepository _dnTaxpayerRepo;
        private readonly IDemandNoticeItemManager _dnItemManger;
        private readonly IArrearsManager _arrearsManager;
        private readonly ILogger _log;
        public DemanNoticeManager(
            IDemandNoticeItemManager demandNoticeItemManager, ITaxpayerRepository taxpayerRepository,
            ILcdaPropertyRepository lcdaPropertyRepository, IStreetRepository streetRepository,
            IStateRepository stateRepository, IWardRepository wardRepository,
            IAddressRepository addressRepository, IImageRepository imageRepository,
            IDemandNoticeRepository demandNoticeRepository, IDemandNoticeArrearRepository demandNoticeArrearRepository,
            IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            IArrearsManager arrearsManager, ILoggerFactory loggerFactory)
        {
            _arrearsManager = arrearsManager;
            demandNoticeDao = demandNoticeRepository;
            dnaDao = demandNoticeArrearRepository;
            _dnTaxpayerRepo = demandNoticeTaxpayersRepository;
            _wardRepository = wardRepository;
            _streetRepository = streetRepository;
            _taxpayerRepository = taxpayerRepository;
            _imagesRepository = imageRepository;
            _addressRepository = addressRepository;
            _stateRepository = stateRepository;
            _lcdaPropertyRepo = lcdaPropertyRepository;
            _dnItemManger = demandNoticeItemManager;
            _log = loggerFactory.CreateLogger("DemanNoticeManager");
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

        public async Task<Response> Delete(Guid id)
        {
            if (id == default(Guid))
            {
                throw new InvalidCredentialsException("Invalid parameters");
            }
            return await demandNoticeDao.UpdateStatus(new DemandNoticeModel
            {
                Id = id,
                DemandNoticeStatus = "DELETED"
            });
        }

        public async Task<DemandNoticeModel> GetByBatchId(string batchId)
        {
            return await demandNoticeDao.GetByBatchId(batchId);
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

        public async Task<TaxPayerModel[]> ValidTaxpayers(DemandNoticeRequestModel model, bool persist)
        {
            //paid, partpayment, pending
            string[] status = { "PART_PAYMENT", "PENDING" };

            DemandNoticeTaxpayersModel[] dntModel = await _dnTaxpayerRepo.SearchTaxpayers(model);

            Guid[] excludedTaxpayerids = dntModel
                .Where(s => !s.IsRunArrears && status.Any(d => d == s.DemandNoticeStatus))
                .Select(x => x.TaxpayerId)
                .ToArray();

            Guid[] demandNoticeIds = dntModel
                .Where(s => !s.IsRunArrears && status.Any(d => d == s.DemandNoticeStatus))
                .Select(x => x.Id)
                .ToArray();

            // run arrears
            #region run arrears 
            int temp = model.dateYear;
            model.dateYear = model.dateYear - 1;
            DemandNoticeTaxpayersModel[] dntModelPreviousYr = await _dnTaxpayerRepo.SearchTaxpayers(model);
            Guid[] dnIdsPrevious = dntModelPreviousYr.Where(s => !s.IsRunArrears && status.Any(d => d == s.DemandNoticeStatus)).Select(x => x.Id).ToArray();
            if ((demandNoticeIds.Length > 0 || dnIdsPrevious.Length > 0) && model.RunArrears)
            {
                try
                {
                    Guid[] idss = demandNoticeIds.Concat(dnIdsPrevious).Distinct().ToArray();
                    if (persist)
                    {
                        await _arrearsManager.RunTaxpayerArrears(idss);
                    }
                    excludedTaxpayerids = new Guid[] { };
                }
                catch (Exception x)
                {
                    _log.LogError(x, "Arrears run error", model);
                }
            }
            model.dateYear = temp;
            #endregion

            TaxPayerModel[] taxPayers = (await _taxpayerRepository.SearchByDNRequest(model, excludedTaxpayerids))
                .Select(d =>
                {
                    var r = d;
                    var t = dntModel.FirstOrDefault(x => x.TaxpayerId == r.Id);
                    r.DemandNoticeStatus = t == null ? "New" : t.DemandNoticeStatus;
                    return r;
                }).ToArray();


            return taxPayers.Where(x => !x.IsOneTime).ToArray();
        }

        public async Task<bool> AddDemanNotice(DemandNoticeRequestModel model)
        {
            if (model.lcdaId == default(Guid))
            {
                throw new InvalidCredentialsException("Request is invalid");
            }
            if (model.Period < 0)
            {
                throw new InvalidCredentialsException("Period is required");
            }

            if (model.TaxpayerIds.Length <= 0)
            {
                throw new InvalidCredentialsException("Please select Taxpayer");
            }

            //run arrears
            //get valid demandnotice Id

            TaxPayerModel[] dntModel = await ValidTaxpayers(model, true);
            var unknownId = model.TaxpayerIds.Where(x => !dntModel.Any(p => p.Id == x)).ToArray();

            if (unknownId.Length > 0)
            {
                throw new InvalidCredentialsException("Please refresh your page and try again");
            }

            Dictionary<string, ImagesModel> images = (await _imagesRepository.ByOwnerId(model.lcdaId)).ToDictionary(x => x.ImgFilename);
            var lcdaAdd = (await _addressRepository.ByOwnersId(model.lcdaId)).FirstOrDefault();
            model.LcdaAddress = lcdaAdd == null ? string.Empty : $"{lcdaAdd.Addressnumber}, {lcdaAdd.StreetName}";
            var lcdastate = await _stateRepository.ByLcda(model.lcdaId);
            model.LcdaState = lcdastate == null ? string.Empty : lcdastate.StateName;
            var treasurerMobile = (await _lcdaPropertyRepo.ByLcda(model.lcdaId)).Select(x => x.PropertyValue).ToArray();
            model.TreasurerMobile = treasurerMobile.Length > 0 ? string.Join(';', treasurerMobile) : string.Empty;
            long batchNo = 0;
            model.InitialBillingNumber = await _dnTaxpayerRepo.NewBillingNumber();
            var lastDN = await demandNoticeDao.GetLastEntry();
            if (lastDN != null)
            {
                string serial = lastDN.BatchNo.Substring(0, 7);
                batchNo = long.Parse(serial);
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

            model.DemandNoticeId = demandNotice.Id;
            DemandNoticeTaxpayersModel[] dnTaxpayer = await _dnTaxpayerRepo.ConstructByTaxpayerIds(model, images);
            //demandNotice.TaxpayerModel = dnTaxpayer.ToList();

            Response response = await demandNoticeDao.Add(demandNotice);
            if (response.code == MsgCode_Enum.SUCCESS)
            {
                await _dnTaxpayerRepo.Add(dnTaxpayer);
                await _dnItemManger.AddDemandNoticeItem(dnTaxpayer);
            }

            demandNotice.DemandNoticeStatus = DemandNoticeStatus.COMPLETED.ToString();
            await demandNoticeDao.UpdateStatus(demandNotice);
            return true;
        }

        public async Task<DemandNoticeTaxpayersModel[]> SearchDemandNotice(DemandNoticeRequestModel rhModel)
        {
            DemandNoticeTaxpayersModel[] model = await _dnTaxpayerRepo.Search(rhModel);
            return model;
        }
    }
}
