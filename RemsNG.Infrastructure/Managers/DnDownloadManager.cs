﻿using Microsoft.Extensions.Options;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DnDownloadManager : IDnDownloadManager
    {
        private readonly IDNPaymentHistoryManager dNPaymentHistoryService;
        private readonly IDemandNoticeTaxpayerManager dnts;
        private readonly IDemandNoticeChargesManager chargesService;
        private readonly IDemandNoticeDownloadHistoryManager demandNoticeDownloadHistory;
        private readonly ILcdaManager lcdaService;
        private readonly IListPropertyManager listPropertyService;
        private readonly ISectorManager sectorService;
        private readonly IDNAmountDueMgtManager amountDueMgtService;
        private readonly ITaxpayerCategoryManager _taxService;
        private readonly BankCategory _bankCategory;
        private readonly IPdfService _pdfService;
        private readonly TemplateDetail _templateDetails;
        public DnDownloadManager(IDemandNoticeTaxpayerManager _dnts,
            IDemandNoticeChargesManager _chargesService,
            IDemandNoticeDownloadHistoryManager _demandNoticeDownloadHistory,
            ILcdaManager _lcdaService,
            IListPropertyManager _listPropertyService,
            ISectorManager _sectorService, IDNAmountDueMgtManager _amountDueMgtService,
            IDNPaymentHistoryManager _dNPaymentHistoryService,
            BankCategory bankCategory, ITaxpayerCategoryManager taxService,
            IPdfService pdfService, TemplateDetail templateOptions)
        {
            _templateDetails = templateOptions;
            dnts = _dnts;
            chargesService = _chargesService;
            demandNoticeDownloadHistory = _demandNoticeDownloadHistory;
            lcdaService = _lcdaService;
            listPropertyService = _listPropertyService;
            sectorService = _sectorService;
            amountDueMgtService = _amountDueMgtService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            _taxService = taxService;
            _bankCategory = bankCategory;
            _pdfService = pdfService;
        }

        private async Task<string> LoadTemplateReceipt(string createdBy, DemandNoticePaymentHistoryModel dnph)
        {
            string htmlContent = await File.ReadAllTextAsync(_templateDetails.RecieptUrl);

            DemandNoticeReportModel dnrp = await dnts.ByBillingNo(dnph.BillingNumber);
            SectorModel sector = await sectorService.ByTaxpayerId(dnrp.TaxpayerId);

            htmlContent = htmlContent.Replace("LOCAL_GOVERNMENT_NAME", dnrp.DomainName);
            htmlContent = htmlContent.Replace("LCDA_NAME", dnrp.LcdaName);
            htmlContent = htmlContent.Replace("LCDA_ADDRESS", dnrp.LcdaAddress);
            htmlContent = htmlContent.Replace("LCDA_STATE", dnrp.LcdaState);

            if (!string.IsNullOrEmpty(_templateDetails.LagosLogo))
            {
                htmlContent = htmlContent.Replace("LAGOSLOGO", _templateDetails.LagosLogo);
            }

            if (!string.IsNullOrEmpty(_templateDetails.LcdaLogo))
            {
                htmlContent = htmlContent.Replace("LCDA_LOGO", _templateDetails.LcdaLogo);
            }

            if (!string.IsNullOrEmpty(_templateDetails.BackgroundLogo))
            {
                htmlContent = htmlContent.Replace("BKIMAGE", _templateDetails.BackgroundLogo);
            }

            htmlContent = htmlContent.Replace("BILL_NO", $"{(sector != null ? sector.Prefix : "")}{dnrp.BillingNumber}");
            htmlContent = htmlContent.Replace("PAYER_NAME", string.IsNullOrEmpty(dnph.OtherNames) ? dnrp.TaxpayersName : dnph.OtherNames);
            htmlContent = htmlContent.Replace("PAYER_ADDRESS", dnrp.AddressName);
            htmlContent = htmlContent.Replace("CURRENT_DATE", DateTime.Now.ToString("dd-MM-yyyy"));
            htmlContent = htmlContent.Replace("BILLING_YEAR", dnrp.BillingYr.ToString());//
            htmlContent = htmlContent.Replace("WARD_NAME", dnrp.WardName);
            htmlContent = htmlContent.Replace("BANK_NAME", dnph.BankName);
            htmlContent = htmlContent.Replace("REFERENCE_NUMBER", dnph.ReferenceNumber);
            htmlContent = htmlContent.Replace("PAYMENT_DATE", dnph.DateCreated.Value.ToString("dd-MM-yyyy"));

            // total amount paid from reciept page
            List<DemandNoticePaymentHistoryModel> dnpHistory =
                await dNPaymentHistoryService.ByBillingNumber(dnph.BillingNumber);
            dnpHistory = dnpHistory.Where(x => x.PaymentStatus == "APPROVED").ToList();

            dnrp.amountPaid = dnpHistory.Sum(x => x.Amount);
            decimal amtDue = dnrp.amountDue;
            // dnrp.amountPaid = amtPaid;

            if (dnpHistory.Count > 0)
            {
                var tt = dnpHistory.OrderByDescending(x => x.DateCreated).FirstOrDefault();
                htmlContent = htmlContent.Replace("PAYMENT_DATE", tt.DateCreated.Value.ToString("dd-MM-yyyy"));
            }
            htmlContent = htmlContent.Replace("TOTAL_AMOUNT", $"{String.Format("{0:n}", decimal.Round(dnph.Amount, 2))} naira");

            if (dnrp.amountPaid > amtDue)
            {
                // overpayment
                htmlContent = htmlContent.Replace("PAYMENT_STATUS", DemandNoticeStatus.OVERPAYMENT.ToString());//PAYMENT_STATUS
                htmlContent = htmlContent.Replace("AMOUNT_REMAINING", $"Amount Overpaid : {String.Format("{0:n}", decimal.Round(dnrp.amountPaid - amtDue, 2))} naira");//PAYMENT_STATUS

            }
            else if (dnrp.amountPaid < amtDue)
            {
                htmlContent = htmlContent.Replace("PAYMENT_STATUS", DemandNoticeStatus.PART_PAYMENT.ToString());//PAYMENT_STATUS
                htmlContent = htmlContent.Replace("AMOUNT_REMAINING",
                    $"Outstanding Balance : {String.Format("{0:n}", decimal.Round(amtDue - dnrp.amountPaid, 2))} naira");//PAYMENT_STATUS

            }
            else
            {
                htmlContent = htmlContent.Replace("PAYMENT_STATUS", DemandNoticeStatus.PAID.ToString());//PAYMENT_STATUS
                htmlContent = htmlContent.Replace("AMOUNT_REMAINING",
                    $"Outstanding Balance : {String.Format("{0:n}", decimal.Round(0, 2))} naira");//PAYMENT_STATUS
            }

            if (dnph.Amount == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD",
                   CurrencyWords.ConvertToWords(decimal.Round(dnph.Amount, 2).ToString()));
            }

            string discp = string.Join(',', dnrp.items.Select(x => x.itemTitle));

            htmlContent = htmlContent.Replace("DESCRIPTION", discp);
            htmlContent = htmlContent.Replace("BANK_NAME", dnph.BankName);

            return htmlContent + "<br/><br/><br/>" + htmlContent;
        }

        public async Task<string> LoadTemplateDemandNotice(string htmlContent, long billingno, string createdBy, TemplateType templateType)
        {
            DemandNoticeReportModel dnrp = await dnts.ByBillingNo(billingno);
            if (dnrp.items.Count == 0)
            {
                return string.Empty;
            }
            SectorModel sector = await sectorService.ByTaxpayerId(dnrp.TaxpayerId);

            htmlContent = htmlContent.Replace("LOCAL_GOVERNMENT_NAME", dnrp.DomainName);
            htmlContent = htmlContent.Replace("LCDA_NAME", dnrp.LcdaName);
            htmlContent = htmlContent.Replace("LCDA_ADDRESS", dnrp.LcdaAddress);
            htmlContent = htmlContent.Replace("LCDA_STATE", dnrp.LcdaState);

            htmlContent = htmlContent.Replace("dated", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            if (sector != null)
            {
                htmlContent = htmlContent.Replace("BILL_NO", $"{sector.Prefix}{dnrp.BillingNumber}");
            }
            else
            {
                htmlContent = htmlContent.Replace("BILL_NO", dnrp.BillingNumber.ToString());
            }

            htmlContent = htmlContent.Replace("PAYER_NAME", dnrp.TaxpayersName);
            htmlContent = htmlContent.Replace("PAYER_ADDRESS", dnrp.AddressName);
            htmlContent = htmlContent.Replace("CURRENT_DATE", DateTime.Now.ToString("dd-MM-yyyy"));
            htmlContent = htmlContent.Replace("BILLING_YEAR", dnrp.BillingYr.ToString());
            htmlContent = htmlContent.Replace("WARD_NAME", dnrp.WardName);

            htmlContent = htmlContent.Replace("ITEMLIST", DemandNoticeComponents.HtmlBuildItems(dnrp));

            var taxCategory = await _taxService.GetTaxpayerCategory(dnrp.TaxpayerId);
            htmlContent = htmlContent.Replace("TAXPAYERCATEGORY", taxCategory.TaxpayerCategoryName);

            //htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp));//, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("ARREARS_AMMOUNT", String.Format("{0:n}", decimal.Round(dnrp.arrears, 2)));
            htmlContent = htmlContent.Replace("PENALTY_AMOUNT", String.Format("{0:n}", decimal.Round(dnrp.penalty, 2)));

            #region  demand Notice images
            //if (!string.IsNullOrEmpty(_templateDetails.LcdaLogo) && templateType == TemplateType.DemandNotice)
            //{
            //    //htmlContent = htmlContent.Replace("LCDA_LOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.LcdaLogo)));
            //}
            //if (!string.IsNullOrEmpty(_templateDetails.LagosLogo) && templateType == TemplateType.DemandNotice)
            //{
            //    htmlContent = htmlContent.Replace("LAGOSLOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.LagosLogo)));
            //}
            //if (!string.IsNullOrEmpty(_templateDetails.BackgroundLogo) && templateType == TemplateType.DemandNotice)
            //{
            //    //htmlContent = htmlContent.Replace("LCDA_LOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.BackgroundLogo)));
            //}
            #endregion


            #region Reminder Images
            //if (!string.IsNullOrEmpty(_templateDetails.ReminderLcdaLogo) && templateType == TemplateType.Reminder)
            //{
            //    htmlContent = htmlContent.Replace("LCDA_LOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.ReminderLcdaLogo)));
            //}
            //if (!string.IsNullOrEmpty(_templateDetails.ReminderLagosLogo) && templateType == TemplateType.Reminder)
            //{
            //    htmlContent = htmlContent.Replace("LAGOSLOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.ReminderLagosLogo)));
            //}
            //if (!string.IsNullOrEmpty(_templateDetails.ReminderBackgroundLogo) && templateType == TemplateType.Reminder)
            //{
            //    htmlContent = htmlContent.Replace("LCDA_LOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(_templateDetails.ReminderBackgroundLogo)));
            //}
            #endregion


            if (!string.IsNullOrEmpty(_templateDetails.CouncilTrasurerSignature))
            {
                string sig = _templateDetails.CouncilTrasurerSignature;
                string tu = "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(sig));
                htmlContent = htmlContent.Replace("COUNCIL_TRESURER_SIG", tu);
            }
            if (!string.IsNullOrEmpty(_templateDetails.RevenueCoodinatorSignature))
            {
                htmlContent = htmlContent.Replace("REV_COORINATOR_SIG", _templateDetails.RevenueCoodinatorSignature);
            }

            htmlContent = htmlContent.Replace("TREASURER_MOBILE", string.IsNullOrEmpty(dnrp.CouncilTreasurerMobile) ? "nil" : dnrp.CouncilTreasurerMobile);
            decimal gtotal = dnrp.items.Sum(x => x.itemAmount) + dnrp.arrears + dnrp.penalty;
            htmlContent = htmlContent.Replace("GRAND_TOTAL", String.Format("{0:n}", decimal.Round(gtotal, 2)));
            htmlContent = htmlContent.Replace("CHARGES", String.Format("{0:n}", decimal.Round(dnrp.charges, 2)));
            decimal amountPaid = 0;
            List<DemandNoticePaymentHistoryModel> dnpHistory = await dNPaymentHistoryService.ByBillingNumber(billingno);
            dnpHistory = dnpHistory.Where(x => x.PaymentStatus == "APPROVED").ToList();
            if (dnpHistory.Count > 0)
            {
                amountPaid = decimal.Round(dnpHistory.Sum(x => x.Amount), 2);
            }

            decimal finalTotal = gtotal + dnrp.charges - amountPaid;
            var prepayment = await dNPaymentHistoryService.GetPrepaymentByTaxpayerId(dnrp.TaxpayerId);

            if (prepayment != null)
            {
                finalTotal = finalTotal - prepayment.amount;
            }
            if (finalTotal < 0)
            {
                finalTotal = 0;
            }

            htmlContent = htmlContent.Replace("AMOUNT_PAID", String.Format("{0:n}", decimal.Round(amountPaid, 2)));
            htmlContent = htmlContent.Replace("FINAL_TOTAL", String.Format("{0:n}", decimal.Round(finalTotal, 2)));

            if (finalTotal == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Payment fully paid");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", CurrencyWords.ConvertToWords(finalTotal.ToString()));
            }

            htmlContent = htmlContent.Replace("PREPAYMENT", $"{String.Format("{0:n}", decimal.Round((prepayment == null ? 0 : prepayment.amount), 2))}");

            DemandNoticeDownloadHistoryModel dndh = new DemandNoticeDownloadHistoryModel();
            dndh.Id = Guid.NewGuid();
            dndh.BillingNumber = billingno;
            dndh.Charges = dnrp.charges;
            dndh.CreatedBy = createdBy;
            dndh.DateCreated = DateTime.Now;
            dndh.GrandTotal = gtotal;

            await demandNoticeDownloadHistory.Add(dndh);
            return htmlContent;
        }

        public async Task<byte[]> GenerateDemandNotice(long[] billingno, string createdBy)
        {
            List<string> lst = new List<string>();
            string htmlContent = await File.ReadAllTextAsync(_templateDetails.DemandNoticeUrl);
            foreach (var tm in billingno)
            {
                string val = await LoadTemplateDemandNotice(htmlContent, tm, createdBy, TemplateType.DemandNotice);
                lst.Add(val);
            }

            byte[] result = _pdfService.GetBytes(lst.ToArray(), TemplateType.DemandNotice);

            return result;
        }

        public async Task<byte[]> GenerateReminder(long[] billingno, string createdBy)
        {
            List<string> lst = new List<string>();
            string htmlContent = await File.ReadAllTextAsync(_templateDetails.ReminderUrl);
            foreach (var tm in billingno)
            {
                string val = await LoadTemplateDemandNotice(htmlContent, tm, createdBy, TemplateType.Reminder);
                lst.Add(val);
            }

            byte[] result = _pdfService.GetBytes(lst.ToArray(), TemplateType.Reminder);

            return result;
        }

        public async Task<byte[]> GenerateReceipt(string createdBy, DemandNoticePaymentHistoryModel dnph)
        {
            string htmlTemplate = await LoadTemplateReceipt(createdBy, dnph);

            byte[] result = _pdfService.GetBytes(new string[] { htmlTemplate });

            return result;
        }

    }
}
