using RemsNG.Dao;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class DemandNoticeTaxpayerService : IDemandNoticeTaxpayerService
    {
        private readonly DemandNoticePaymentHistoryDao _dphDao;
        private DemandNoticeTaxpayersDao dntDao;
        private IDemandNoticeItemService dnItemService;
        private DemandNoticeArrearDao dna;
        private DemandNoticePenaltyDao dnp;
        private ITaxpayerService taxpayerService;
        private readonly IImageService imageService;
        private readonly ILcdaBankService lcdaBankService;
        private readonly IListPropertyService lpService;
        private readonly IDemandNoticeCharges chargesService;
        private readonly ILcdaService lcdaService;
        private IDNAmountDueMgtService _admService;
        public DemandNoticeTaxpayerService(RemsDbContext _db,
            IDemandNoticeItemService _dnItemService, ITaxpayerService _taxpayerService,
             IImageService _imageService, ILcdaBankService _lcdaBankService,
             IListPropertyService _lpService,
            IDemandNoticeCharges _chargesService, ILcdaService _lcdaService, IDNAmountDueMgtService admService)
        {
            dntDao = new DemandNoticeTaxpayersDao(_db);
            _dphDao = new DemandNoticePaymentHistoryDao(_db);
            dnItemService = _dnItemService;
            dna = new DemandNoticeArrearDao(_db);
            dnp = new DemandNoticePenaltyDao(_db);
            taxpayerService = _taxpayerService;
            imageService = _imageService;
            lcdaBankService = _lcdaBankService;
            lpService = _lpService;
            chargesService = _chargesService;
            lcdaService = _lcdaService;
            _admService = admService;
        }

        public async Task<DemandNoticeReportModel> ByBillingNo(string billingNo)
        {
            try
            {
                var t = await dntDao.ByBillingNo(billingNo);

                if (t == null)
                {
                    throw new NotFoundException($"{billingNo} does not exist");
                }

                DemandNoticeReportModel dnrm = new DemandNoticeReportModel()
                {
                    addressName = t.addressName,
                    billingNumber = t.billingNumber,
                    billingYr = t.billingYr,
                    councilTreasurerMobile = t.councilTreasurerMobile,
                    councilTreasurerSigFilen = t.councilTreasurerSigFilen,
                    createdBy = t.createdBy,
                    domainName = t.domainName.ToUpper(),
                    lcdaAddress = t.lcdaAddress,
                    lcdaLogoFileName = t.lcdaLogoFileName,
                    lcdaName = t.lcdaName.ToUpper(),
                    lcdaState = t.lcdaState,
                    revCoodinatorSigFilen = t.revCoodinatorSigFilen,
                    taxpayersName = t.taxpayersName,
                    wardName = t.wardName,
                    taxpayerId = t.taxpayerId,
                    demandNoticeStatus = t.demandNoticeStatus
                };
                Lgda lgda = await lcdaService.ByBillingNumber(billingNo); //await taxpayerService.getLcda(t.taxpayerId);
                List<LcdaProperty> ls = new List<LcdaProperty>();
                if (lgda != null)
                {
                    dnrm.lcdaId = lgda.id;
                    ls = await lpService.ByLcda(lgda.id);
                }
                List<LcdaProperty> coucilNum = ls.Where(z => z.propertyKey == "COUNCIL_TREASURER_MOBILE").ToList();
                if (coucilNum.Count > 0)
                {
                    dnrm.councilTreasurerMobile = String.Join(",", coucilNum.Select(x => x.propertyValue));
                }

                dnrm.lcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lgda.id,
                                   ImgTypesEnum.LOGO.ToString());
                dnrm.revCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.id,
                    ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                dnrm.councilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.id,
                    ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());

                List<DemandNoticeItem> dnitem = await dnItemService.ByBillingNumber(billingNo);

                dnrm.items = dnitem.Select(x => new DnReportItem()
                {
                    itemTitle = x.itemName,
                    itemAmount = x.itemAmount
                }).ToList();

                dnrm.amountPaid = dnrm.amountPaid + dnitem.Sum(x => x.amountPaid);

                dnrm.banks = await lcdaBankService.Get(lgda.id);

                var penalties = await dnp.ByTaxpayerId(dnrm.taxpayerId);

                dnrm.penalty = penalties.Sum(x => (x.totalAmount - x.amountPaid));
                dnrm.amountPaid = dnrm.amountPaid + penalties.Sum(x => x.amountPaid);

                var arrears = await dna.ByBillingNumber(billingNo);
                dnrm.arrears = arrears.Sum(x => (x.totalAmount - x.amountPaid));
                var amtDue = await _dphDao.ByBillingNumber(billingNo);

                dnrm.amountPaid = amtDue.Sum(x => x.amount);//dnrm.amountPaid + arrears.Sum(x => x.amountPaid);

                LcdaProperty isEnablePayment = ls.FirstOrDefault(x =>
                x.propertyKey == "ALLOW_PAYMENT_SERVICES" && x.propertyStatus == "ACTIVE");
                decimal gtotal = dnrm.items.Sum(x => x.itemAmount) + dnrm.arrears + dnrm.penalty;
                dnrm.amountDue = gtotal;
                if (isEnablePayment != null)
                {
                    if (isEnablePayment.propertyValue == "1")
                    {
                        dnrm.charges = await chargesService.getCharges(gtotal, dnrm.lcdaId);
                    }
                    else
                    {
                        dnrm.charges = 0;
                    }
                }
                else
                {
                    dnrm.charges = 0;
                }

                return dnrm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchNoAsync(string batchno)
        {
            return await dntDao.GetDNTaxpayerByBatchNoAsync(batchno);
        }

        public async Task<DemandNoticeTaxpayersDetail> TaxpayerMiniByBillingNo(string billingNo)
        {
            return await dntDao.ByBillingNo(billingNo);
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo, string createdBy)
        {
            return await dntDao.CancelTaxpayerDemandNoticeByBillingNo(billingNo, createdBy);
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> Search(string query)
        {
            return await dntDao.SearchAllAsync(query);
        }

        public async Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId)
        {
            return await dntDao.BlinkClosesDemandNoticeByCompany(companyId);
        }

        public async Task<DemandNoticeTaxpayersDetail[]> GetAllReceivables()
        {
            return await dntDao.GetAllReceivables();
        }

        public async Task<List<object>> GetTaxpayerPayables(Guid taxpayerId)
        {
            if (taxpayerId == default(Guid))
            {
                throw new UserValidationException("bad request");
            }

            var result = await dntDao.GetTaxpayerPayables(taxpayerId);
            List<object> lstPayables = new List<object>();
            foreach (var tm in result)
            {
                var currentDue = await _admService.ByBillingNo(tm.billingNumber);
                decimal amtDue = currentDue.Sum(x => (x.itemAmount - x.amountPaid));
                lstPayables.Add(new
                {
                    billingNumber = tm.billingNumber,
                    billingYr = tm.billingYr,
                    wardName = tm.wardName,
                    demandNoticeStatus = tm.demandNoticeStatus,
                    amountDue = amtDue,
                    dateCreated = tm.dateCreated
                });
            }

            return lstPayables;
        }

        public async Task<bool> MoveToBill(string billno)
        {
            return await dntDao.MoveToBills(billno);
        }
        public async Task<bool> MoveToUnBills(string billno)
        {
            return await dntDao.MoveToUnBills(billno);
        }

        public async Task<List<DemandNoticeArrears>> GetArrearsByTaxpayerId(Guid taxpayerId)
        {
            return await dna.ByTaxpayer(taxpayerId);
        }
    }
}
