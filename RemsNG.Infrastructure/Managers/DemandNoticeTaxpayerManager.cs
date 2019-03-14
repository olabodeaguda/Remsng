﻿using Microsoft.EntityFrameworkCore;
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
    public class DemandNoticeTaxpayerManager : IDemandNoticeTaxpayerManager
    {
        private readonly DemandNoticePaymentHistoryRepository _dphDao;
        private DemandNoticeTaxpayersRepository dntDao;
        private IDemandNoticeItemManager dnItemService;
        private DemandNoticeArrearRepository dna;
        private DemandNoticePenaltyRepository dnp;
        private ITaxpayerManager taxpayerService;
        private readonly IImageManager imageService;
        private readonly ILcdaBankManager lcdaBankService;
        private readonly IListPropertyManager lpService;
        private readonly IDemandNoticeChargesManager chargesService;
        private readonly ILcdaManager lcdaService;
        private IDNAmountDueMgtManager _admService;
        public DemandNoticeTaxpayerManager(DbContext _db,
            IDemandNoticeItemManager _dnItemService, ITaxpayerManager _taxpayerService,
             IImageManager _imageService, ILcdaBankManager _lcdaBankService,
             IListPropertyManager _lpService,
            IDemandNoticeChargesManager _chargesService, ILcdaManager _lcdaService,
            IDNAmountDueMgtManager admService)
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

                dnrm.items = dnitem.Where(r => r.TaxpayerId == dnrm.TaxpayerId).Select(x => new DnReportItemModel()
                {
                    itemTitle = x.ItemName,
                    itemAmount = x.ItemAmount
                }).ToList();

                // dnrm.amountPaid = dnrm.amountPaid + dnitem.Sum(x => x.AmountPaid);

                dnrm.banks = await lcdaBankService.Get(lgda.Id);

                var penalties = await dnp.ByTaxpayerId(dnrm.TaxpayerId);
                dnrm.penalty = penalties.Sum(x => x.TotalAmount);

                //dnrm.amountPaid = dnrm.amountPaid + penalties.Sum(x => x.AmountPaid);

                var arrears = await dna.ByBillingNumber(dnrm.TaxpayerId);
                dnrm.arrears = arrears.Sum(x => x.TotalAmount);

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

        public async Task<DemandNoticeReportModel> ReportbyBillNumber(long billingNo)
        {
            try
            {
                var t = await dntDao.ByBillingNo(billingNo);

                if (t == null)
                {
                    return null;
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
                LcdaModel lgda = await lcdaService.ByBillingNumber(billingNo);
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

                dnrm.LcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lgda.Id, ImgTypesEnum.LOGO.ToString());
                dnrm.RevCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.Id, ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                dnrm.CouncilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lgda.Id, ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());

                List<DemandNoticeItemModel> dnitem = await dnItemService.ByBillingNumber(billingNo);

                dnrm.items = dnitem.Select(x => new DnReportItemModel()
                {
                    itemTitle = x.ItemName,
                    itemAmount = x.ItemAmount
                }).ToList();

                dnrm.banks = await lcdaBankService.Get(lgda.Id);

                var penalties = await dnp.ByTaxpayerId(dnrm.TaxpayerId);
                dnrm.penalty = penalties.Sum(x => x.TotalAmount);

                var arrears = await dna.ByBillingNumber(dnrm.TaxpayerId);
                dnrm.arrears = arrears.Sum(x => x.TotalAmount);

                var amtDue = await _dphDao.ByBillingNumber(billingNo);
                dnrm.amountPaid = amtDue.Sum(x => x.Amount);

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
                return null;
            }
        }

        public async Task<DemandNoticeReportModel[]> ByBillingNo(long[] billingNo)
        {
            List<DemandNoticeReportModel> lst = new List<DemandNoticeReportModel>();

            foreach (var tm in billingNo)
            {
                var b = await ReportbyBillNumber(tm);
                if (b != null)
                {
                    lst.Add(b);
                }
            }

            return lst.ToArray();
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

        public async Task<bool> MoveToBill(long billno)
        {
            return await dntDao.MoveToBills(billno);
        }

        public async Task<bool> MoveToUnBills(long billno)
        {
            return await dntDao.MoveToUnBills(billno);
        }

        public async Task<List<DemandNoticeArrearsModel>> GetArrearsByTaxpayerId(Guid taxpayerId)
        {
            return await dna.ByTaxpayer(taxpayerId);
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