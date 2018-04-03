using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.Exceptions;
using RemsNG.Utilities;
using System.Text;

namespace RemsNG.Services
{
    public class DemandNoticeTaxpayerService : IDemandNoticeTaxpayerService
    {
        private DemandNoticeTaxpayersDao dntDao;
        private IDemandNoticeItemService dnItemService;
        private DemandNoticeArrearDao dna;
        private DemandNoticePenaltyDao dnp;
        private ITaxpayerService taxpayerService;
        private readonly IImageService imageService;
        private readonly ILcdaBankService lcdaBankService;
        private readonly IListPropertyService lpService;
        private readonly IDemandNoticeCharges chargesService;
        public DemandNoticeTaxpayerService(RemsDbContext _db,
            IDemandNoticeItemService _dnItemService, ITaxpayerService _taxpayerService,
             IImageService _imageService, ILcdaBankService _lcdaBankService,
             IListPropertyService _lpService,
            IDemandNoticeCharges _chargesService)
        {
            dntDao = new DemandNoticeTaxpayersDao(_db);
            dnItemService = _dnItemService;
            dna = new DemandNoticeArrearDao(_db);
            dnp = new DemandNoticePenaltyDao(_db);
            taxpayerService = _taxpayerService;
            imageService = _imageService;
            lcdaBankService = _lcdaBankService;
            lpService = _lpService;
            chargesService = _chargesService;
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
                Lgda lgda = await taxpayerService.getLcda(t.taxpayerId);
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
                    itemAmount = x.itemAmount - x.amountPaid
                }).ToList();

                dnrm.banks = await lcdaBankService.Get(lgda.id);

                var penalties = await dnp.ByBillingNumber(billingNo);

                dnrm.penalty = penalties.Sum(x => (x.totalAmount - x.amountPaid));

                var arrears = await dna.ByBillingNumber(billingNo);
                dnrm.arrears = arrears.Sum(x => (x.totalAmount - x.amountPaid));

                LcdaProperty isEnablePayment = ls.FirstOrDefault(x =>
                x.propertyKey == "ALLOW_PAYMENT_SERVICES" && x.propertyStatus == "ACTIVE");
                decimal gtotal = dnrm.items.Sum(x => x.itemAmount) + dnrm.arrears + dnrm.penalty;
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
            catch (Exception x)
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

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo)
        {
            return await dntDao.CancelTaxpayerDemandNoticeByBillingNo(billingNo);
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> Search(string query)
        {
            return await dntDao.SearchAllAsync(query);
        }

        public async Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId)
        {
            return await dntDao.BlinkClosesDemandNoticeByCompany(companyId);
        }
    }
}
