using Microsoft.EntityFrameworkCore;
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
    public class DemandNoticeTaxpayerManager : IDemandNoticeTaxpayerManager
    {
        private readonly IDemandNoticePaymentHistoryRepository _dphDao;
        private readonly IDemandNoticeTaxpayersRepository dntDao;
        private readonly IDemandNoticeItemManager dnItemService;
        private readonly IDemandNoticeArrearRepository dna;
        private readonly IDemandNoticePenaltyRepository dnp;
        private readonly ITaxpayerManager taxpayerService;
        private readonly IImageManager imageService;
        private readonly ILcdaBankManager lcdaBankService;
        private readonly IListPropertyManager lpService;
        private readonly IDemandNoticeChargesManager chargesService;
        private readonly ILcdaManager lcdaService;
        private readonly IDNAmountDueMgtManager _admService;
        public DemandNoticeTaxpayerManager(IDemandNoticeItemManager _dnItemService, ITaxpayerManager _taxpayerService,
             IImageManager _imageService, ILcdaBankManager _lcdaBankService,
             IListPropertyManager _lpService,
            IDemandNoticeChargesManager _chargesService, ILcdaManager _lcdaService,
            IDNAmountDueMgtManager admService,
            IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            IDemandNoticePaymentHistoryRepository demandNoticePaymentHistoryRepository,
            IDemandNoticeArrearRepository demandNoticeArrearRepository,
            IDemandNoticePenaltyRepository demandNoticePenaltyRepository)
        {
            dntDao = demandNoticeTaxpayersRepository;
            _dphDao = demandNoticePaymentHistoryRepository;
            dnItemService = _dnItemService;
            dna = demandNoticeArrearRepository;
            dnp = demandNoticePenaltyRepository;
            taxpayerService = _taxpayerService;
            imageService = _imageService;
            lcdaBankService = _lcdaBankService;
            lpService = _lpService;
            chargesService = _chargesService;
            lcdaService = _lcdaService;
            _admService = admService;
        }

        public async Task<(decimal amountDue, List<AmountDueModel> amountDueDetails)> AmountDue(long billingNo)
        {
            var t = await dntDao.ByBillingNo(billingNo);

            if (t == null)
            {
                throw new NotFoundException($"{billingNo} does not exist");
            }
            decimal amtDue = 0;
            List<AmountDueModel> amountDue = new List<AmountDueModel>();

            List<DemandNoticeItemModel> dnitem = await dnItemService.ByBillingNumber(billingNo);
            if (dnitem.Count > 0)
            {
                var res = dnitem.Where(r => r.TaxpayerId == t.TaxpayerId);
                amountDue.AddRange(res
                    .Select(x => new AmountDueModel()
                    {
                        Description = x.ItemName,
                        Amount = x.ItemAmount,
                        Category = Category.Item,
                        Id = x.Id.ToString()
                    }).ToList());
                amtDue = amtDue + res.Sum(d => d.ItemAmount);
            }
            var penalties = await dnp.ByTaxpayerId(t.TaxpayerId);
            if (penalties.Count > 0)
            {
                amountDue.AddRange(penalties
                    .Select(x => new AmountDueModel()
                    {
                        Description = Category.Penalty.ToString(),
                        Amount = x.CurrentAmount,
                        Category = Category.Penalty,
                        Id = x.Id.ToString()
                    }).ToList());

                amtDue = amtDue + penalties.Sum(s => s.CurrentAmount);
            }

            var arrears = await dna.ByBillingNumber(t.TaxpayerId);
            if (arrears.Count > 0)
            {
                amountDue.AddRange(arrears
                   .Select(x => new AmountDueModel()
                   {
                       Description = Category.Arrears.ToString(),
                       Amount = x.CurrentAmount,
                       Category = Category.Arrears,
                       Id = x.Id.ToString()
                   }).ToList());
                amtDue = amtDue + arrears.Sum(x => x.CurrentAmount);
            }

            var amtPaid = (await _dphDao.ByBillingNumber(billingNo))
                .Where(x => x.PaymentStatus == "APPROVED").ToArray();
            if (amtPaid.Length > 0)
            {
                amountDue.AddRange(amtPaid
                      .Select(x => new AmountDueModel()
                      {
                          Description = Category.Arrears.ToString(),
                          Amount = x.Amount,
                          Category = Category.Paid,
                          Id = x.Id.ToString()
                      }).ToList());
                amtDue = amtDue - amtPaid.Sum(x => x.Amount);
            }

            var prepayment = await _dphDao.GetPrepaymentList(t.TaxpayerId);
            if (prepayment.Length > 0)
            {
                amountDue.AddRange(prepayment
                     .Select(x => new AmountDueModel()
                     {
                         Description = Category.Prepayment.ToString(),
                         Amount = x.amount,
                         Category = Category.Prepayment,
                         Id = x.id.ToString()
                     }).ToList());
                amtDue = amtDue - prepayment.Sum(x => x.amount);
            }

            return (amtDue, amountDue);
        }

        public async Task<DemandNoticeReportModel> ByBillingNo(long billingNo)
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
                    DemandNoticeStatus = t.DemandNoticeStatus,
                    Period = t.Period
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

                dnrm.items = dnitem.Where(r => r.TaxpayerId == dnrm.TaxpayerId).Select(x => new DnReportItemModel()
                {
                    itemTitle = x.ItemName,
                    itemAmount = x.ItemAmount
                }).ToList();

                string[] status = { "PENDING", "PART_PAYMENT" };

                dnrm.banks = await lcdaBankService.Get(lgda.Id);

                var penalties = await dnp.ByTaxpayerId(dnrm.TaxpayerId);
                dnrm.penalty = penalties.Where(x => x.BillingYear == t.BillingYr && status.Any(p => p == x.ItemPenaltyStatus)).Sum(x => x.TotalAmount);

                var arrears = await dna.ByBillingNumber(dnrm.TaxpayerId);
                dnrm.arrears = arrears.Where(x => x.BillingYear == t.BillingYr && status.Any(p => p == x.ArrearsStatus)).Sum(x => x.TotalAmount);

                var amtDue = await _dphDao.ByBillingNumber(billingNo);
                dnrm.amountPaid = amtDue.Sum(x => x.Amount);//dnrm.amountPaid + arrears.Sum(x => x.amountPaid);

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

        public async Task<DemandNoticeTaxpayersModel> TaxpayerMiniByBillingNo(long billingNo)
        {
            return await dntDao.ByBillingNo(billingNo);
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(long billingNo, string createdBy)
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

        public async Task<bool> MoveToBill(long billno)
        {
            return await dntDao.MoveToBills(billno);
        }

        public async Task<bool> MoveToUnBills(long billno)
        {
            return await dntDao.MoveToUnBills(billno);
        }

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchByLcdaId(DemandNoticeRequestModel rhModel,
            PageModel pageModel, Guid lcdaId)
        {
            if (lcdaId == default(Guid))
            {
                throw new UserValidationException("User lcda is invalid...");
            }

            return await dntDao.SearchByLcdaId(rhModel, pageModel, lcdaId);
        }

        public async Task<bool> Delete(Guid[] id)
        {
            if (id.Length <= 0)
            {
                throw new InvalidCredentialsException("Invalid parameters");
            }

            return await dntDao.UpdateSatus(id, "DELETED");
        }
    }
}
