using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeTaxpayerManagers : IDemandNoticeTaxpayerManagers
    {
        private readonly DemandNoticePaymentHistoryRepository _dphDao;
        private DemandNoticeTaxpayersRepository dntDao;
        private IDemandNoticeItemManagers dnItemService;
        private DemandNoticeArrearRepository dna;
        private DemandNoticePenaltyRepository dnp;
        private ITaxpayerManagers taxpayerService;
        private readonly IImageManagers imageService;
        private readonly ILcdaBankManagers lcdaBankService;
        private readonly IListPropertyManagers lpService;
        private readonly IDemandNoticeChargesManagers chargesService;
        private readonly ILcdaManagers lcdaService;
        private IDNAmountDueMgtManagers _admService;
        public DemandNoticeTaxpayerManagers(DbContext _db,
            IDemandNoticeItemManagers _dnItemService, ITaxpayerManagers _taxpayerService,
             IImageManagers _imageService, ILcdaBankManagers _lcdaBankService,
             IListPropertyManagers _lpService,
            IDemandNoticeChargesManagers _chargesService, ILcdaManagers _lcdaService,
            IDNAmountDueMgtManagers admService)
        {
            dntDao = new DemandNoticeTaxpayersRepository(_db);
            _dphDao = new DemandNoticePaymentHistoryRepository(_db);
            dnItemService = _dnItemService;
            dna = new DemandNoticeArrearRepository(_db);
            dnp = new DemandNoticePenaltyRepository(_db);
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
                    AddressName = t.AddressName,
                    BillingNumber = t.BillingNumber,
                    BillingYr = t.BillingYr,
                    CouncilTreasurerMobile = t.CouncilTreasurerMobile,
                    CouncilTreasurerSigFilen = t.CouncilTreasurerSigFilen,
                    CreatedBy = t.CreatedBy,
                    DomainName = t.DomainName.ToUpper(),
                    LcdaAddress = t.LcdaAddress,
                    LcdaLogoFileName = t.LcdaLogoFileName,
                    LcdaName = t.LcdaName.ToUpper(),
                    LcdaState = t.LcdaState,
                    RevCoodinatorSigFilen = t.RevCoodinatorSigFilen,
                    TaxpayersName = t.TaxpayersName,
                    WardName = t.WardName,
                    TaxpayerId = t.TaxpayerId,
                    DemandNoticeStatus = t.DemandNoticeStatus
                };
                LcdaModel lgda = await lcdaService.ByBillingNumber(billingNo); //await taxpayerService.getLcda(t.taxpayerId);
                List<LcdaPropertyModel> ls = new List<LcdaPropertyModel>();
                if (lgda != null)
                {
                    dnrm.lcdaId = lgda.Id;
                    ls = await lpService.ByLcda(lgda.Id);
                }
                List<LcdaPropertyModel> coucilNum = ls.Where(z => z.PropertyKey == "COUNCIL_TREASURER_MOBILE").ToList();
                if (coucilNum.Count > 0)
                {
                    dnrm.CouncilTreasurerMobile = String.Join(",", coucilNum.Select(x => x.PropertyValue));
                }

                dnrm.LcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lgda.Id,
                                   ImgTypesEnum.LOGO.ToString());
                dnrm.RevCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.Id,
                    ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                dnrm.CouncilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.Id,
                    ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());

                List<DemandNoticeItemModel> dnitem = await dnItemService.ByBillingNumber(billingNo);

                dnrm.items = dnitem.Select(x => new DnReportItemModel()
                {
                    itemTitle = x.ItemName,
                    itemAmount = x.ItemAmount
                }).ToList();

                dnrm.amountPaid = dnrm.amountPaid + dnitem.Sum(x => x.AmountPaid);

                dnrm.banks = await lcdaBankService.Get(lgda.Id);

                var penalties = await dnp.ByTaxpayerId(dnrm.TaxpayerId);

                dnrm.penalty = penalties.Sum(x => (x.totalAmount - x.amountPaid));
                dnrm.amountPaid = dnrm.amountPaid + penalties.Sum(x => x.amountPaid);

                var arrears = await dna.ByBillingNumber(billingNo);
                dnrm.arrears = arrears.Sum(x => (x.TotalAmount - x.AmountPaid));
                var amtDue = await _dphDao.ByBillingNumber(billingNo);

                dnrm.amountPaid = amtDue.Sum(x => x.amount);//dnrm.amountPaid + arrears.Sum(x => x.amountPaid);

                LcdaPropertyModel isEnablePayment = ls.FirstOrDefault(x =>
                x.PropertyKey == "ALLOW_PAYMENT_SERVICES" && x.PropertyStatus == "ACTIVE");
                decimal gtotal = dnrm.items.Sum(x => x.itemAmount) + dnrm.arrears + dnrm.penalty;
                dnrm.amountDue = gtotal;
                if (isEnablePayment != null)
                {
                    if (isEnablePayment.PropertyValue == "1")
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

        public async Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchNoAsync(string batchno)
        {
            return await dntDao.GetDNTaxpayerByBatchNoAsync(batchno);
        }

        public async Task<DemandNoticeTaxpayersModel> TaxpayerMiniByBillingNo(string billingNo)
        {
            return await dntDao.ByBillingNo(billingNo);
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo, string createdBy)
        {
            return await dntDao.CancelTaxpayerDemandNoticeByBillingNo(billingNo, createdBy);
        }

        public async Task<List<DemandNoticeTaxpayersModel>> Search(string query)
        {
            return await dntDao.SearchAllAsync(query);
        }

        public async Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId)
        {
            return await dntDao.BlinkClosesDemandNoticeByCompany(companyId);
        }

        public async Task<DemandNoticeTaxpayersModel[]> GetAllReceivables()
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
                var currentDue = await _admService.ByBillingNo(tm.BillingNumber);
                decimal amtDue = currentDue.Sum(x => (x.itemAmount - x.amountPaid));
                lstPayables.Add(new
                {
                    billingNumber = tm.BillingNumber,
                    billingYr = tm.BillingYr,
                    wardName = tm.WardName,
                    demandNoticeStatus = tm.DemandNoticeStatus,
                    amountDue = amtDue,
                    dateCreated = tm.DateCreated
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

        public async Task<List<DemandNoticeArrearsModel>> GetArrearsByTaxpayerId(Guid taxpayerId)
        {
            return await dna.ByTaxpayer(taxpayerId);
        }
    }
}
