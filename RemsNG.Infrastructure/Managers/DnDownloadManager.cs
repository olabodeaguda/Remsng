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
        private IListPropertyManager listPropertyService;
        private ISectorManager sectorService;
        private IDNAmountDueMgtManager amountDueMgtService;
        private ITaxpayerCategoryManager _taxService;
        private readonly BankCategory _bankCategory;
        private readonly IPdfService _pdfService;
        public DnDownloadManager(IDemandNoticeTaxpayerManager _dnts,
            IDemandNoticeChargesManager _chargesService,
            IDemandNoticeDownloadHistoryManager _demandNoticeDownloadHistory,
            ILcdaManager _lcdaService,
            IListPropertyManager _listPropertyService,
            ISectorManager _sectorService, IDNAmountDueMgtManager _amountDueMgtService,
            IDNPaymentHistoryManager _dNPaymentHistoryService,
            BankCategory bankCategory, ITaxpayerCategoryManager taxService,
            IPdfService pdfService)
        {
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

        public async Task<string> PopulateReceiptHtml(string htmlContent, string rootUrl,
            string createdBy, DemandNoticePaymentHistoryModel dnph)
        {
            //var amountDue = await amountDueMgtService.ByBillingNo(dnph.billingNumber);
            //decimal amtDue = amountDue.Sum(x => x.amountPaid);
            // get history amount
            DemandNoticeReportModel dnrp = await dnts.ByBillingNo(dnph.BillingNumber);
            SectorModel sector = await sectorService.ByTaxpayerId(dnrp.TaxpayerId);

            htmlContent = htmlContent.Replace("LOCAL_GOVERNMENT_NAME", dnrp.DomainName);
            htmlContent = htmlContent.Replace("LCDA_NAME", dnrp.LcdaName);
            htmlContent = htmlContent.Replace("LCDA_ADDRESS", dnrp.LcdaAddress);
            htmlContent = htmlContent.Replace("LCDA_STATE", dnrp.LcdaState);
            htmlContent = htmlContent.Replace("LAGOSLOGO", $"{rootUrl}/images/lagoslogo.jpg");
            htmlContent = htmlContent.Replace("LCDA_LOGO", $"{rootUrl}/images/{dnrp.LcdaLogoFileName}");
            htmlContent = htmlContent.Replace("BKIMAGE", $"{rootUrl}/templates/lagoslogo.jpeg");

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
            // htmlContent = htmlContent.Replace("TOTAL_AMOUNT", $"{String.Format("{0:n}", decimal.Round(dnrp.amountPaid, 2))} naira");
            #region old
            //dnph.amount > dnrp.amountPaid
            //if (dnph.amount > dnrp.amountPaid)
            //{
            //    htmlContent = htmlContent.Replace("PAYMENT_STATUS", DemandNoticeStatus.OVERPAYMENT.ToString());//PAYMENT_STATUS
            //    htmlContent = htmlContent.Replace("AMOUNT_REMAINING", $"Amount Overpaid : {String.Format("{0:n}", decimal.Round(dnph.amount - dnrp.amountPaid, 2))} naira");//PAYMENT_STATUS
            //}
            //else if (dnrp.amountDue > dnrp.amountPaid)
            //{
            //    htmlContent = htmlContent.Replace("PAYMENT_STATUS", dnrp.demandNoticeStatus);//PAYMENT_STATUS
            //    decimal s = 0;
            //    htmlContent = htmlContent.Replace("AMOUNT_REMAINING", $"Amount balance Due : {String.Format("{0:n}", decimal.Round(dnrp.amountDue - dnrp.amountPaid, 2), 2)} naira");//PAYMENT_STATUS
            //}
            //else
            //{
            //    htmlContent = htmlContent.Replace("PAYMENT_STATUS", dnrp.demandNoticeStatus);//PAYMENT_STATUS
            //    decimal s = 0;
            //    htmlContent = htmlContent.Replace("AMOUNT_REMAINING", $"Amount Overpaid/balance Due : {String.Format("{0:n}", decimal.Round(s, 2), 2)} naira");//PAYMENT_STATUS
            //} 
            #endregion

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

            return htmlContent;
        }

        public async Task<string> PopulateReportHtml(string htmlContent, long billingno,
            string rootUrl, string createdBy)
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
            htmlContent = htmlContent.Replace("LAGOSLOGO", $"{rootUrl}/images/lagoslogo.jpg");
            htmlContent = htmlContent.Replace("LCDA_LOGO", $"{rootUrl}/images/{dnrp.LcdaLogoFileName}");
            htmlContent = htmlContent.Replace("dated", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));//./templates/lcdaLogo.jpeg
            htmlContent = htmlContent.Replace("BKIMAGE", $"{rootUrl}/templates/lcdaLogo.jpeg");

            //if (dnrp.BillingNumber.Length < 5)
            //{
            //    string v = "";
            //    for (int i = 0; i < 5 - dnrp.BillingNumber.Length; i++)
            //    {
            //        v = v + "0";
            //    }

            //    dnrp.BillingNumber = v + dnrp.BillingNumber;
            //}

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

            htmlContent = htmlContent.Replace("ITEMLIST", DemandNoticeComponents.HtmlBuildItems1(dnrp));

            htmlContent = htmlContent.Replace("PATCH2", "");

            var taxCategory = await _taxService.GetTaxpayerCategory(dnrp.TaxpayerId);
            htmlContent = htmlContent.Replace("TAXPAYERCATEGORY", taxCategory.TaxpayerCategoryName);

            htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("ARREARS_AMMOUNT", String.Format("{0:n}", decimal.Round(dnrp.arrears, 2)));
            htmlContent = htmlContent.Replace("PENALTY_AMOUNT", String.Format("{0:n}", decimal.Round(dnrp.penalty, 2)));

            if (!string.IsNullOrEmpty(dnrp.CouncilTreasurerSigFilen))
            {
                htmlContent = htmlContent.Replace("COUNCIL_TRESURER_SIG", $"{rootUrl}/images/{dnrp.CouncilTreasurerSigFilen}");
            }
            if (!string.IsNullOrEmpty(dnrp.RevCoodinatorSigFilen))
            {
                htmlContent = htmlContent.Replace("REV_COORINATOR_SIG", $"{rootUrl}/images/{dnrp.RevCoodinatorSigFilen}");
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

            if (prepayment != null && finalTotal > 0)
            {
                finalTotal = finalTotal - prepayment.amount;
            }
            else if (finalTotal < 0)
            {
                finalTotal = 0;
            }

            htmlContent = htmlContent.Replace("AMOUNT_PAID", String.Format("{0:n}", decimal.Round(amountPaid, 2)));
            htmlContent = htmlContent.Replace("FINAL_TOTAL", String.Format("{0:n}", decimal.Round(finalTotal, 2)));

            if (finalTotal == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", CurrencyWords.ConvertToWords(finalTotal.ToString()));
            }

            htmlContent = htmlContent.Replace("PREPAYMENT", $"{String.Format("{0:n}", decimal.Round((prepayment == null ? 0 : prepayment.amount), 2))} naira");

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

        public async Task<string> PopulateReportHtml1(string htmlContent, long billingno,
            string rootUrl, string createdBy)
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

            //htmlContent = htmlContent.Replace("LAGOSLOGO", $"{rootUrl}/images/lagoslogo.jpg");
            //htmlContent = htmlContent.Replace("LCDA_LOGO", $"{rootUrl}/images/{dnrp.LcdaLogoFileName}");

            //htmlContent = htmlContent.Replace("LCDA_LOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/images/lagoslogo.png")));
            //htmlContent = htmlContent.Replace("LAGOSLOGO", "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/images/lagoslogo.png")));
            //htmlContent = htmlContent.Replace("LCDA_LOGO", Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/images/{dnrp.LcdaLogoFileName}")));

            htmlContent = htmlContent.Replace("dated", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));//./templates/lcdaLogo.jpeg
                                                                                                  //  htmlContent = htmlContent.Replace("BKIMAGE", $"{rootUrl}/templates/lcdaLogo.jpeg");

            //if (dnrp.BillingNumber.Length < 5)
            //{
            //string v = "";
            //for (int i = 0; i < 5 - dnrp.BillingNumber.Length; i++)
            //{
            //    v = v + "0";
            //}

            //dnrp.BillingNumber = v + dnrp.BillingNumber;
            //}

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

            htmlContent = htmlContent.Replace("PATCH2", "");

            var taxCategory = await _taxService.GetTaxpayerCategory(dnrp.TaxpayerId);
            htmlContent = htmlContent.Replace("TAXPAYERCATEGORY", taxCategory.TaxpayerCategoryName);

            //htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp));//, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("ARREARS_AMMOUNT", String.Format("{0:n}", decimal.Round(dnrp.arrears, 2)));
            htmlContent = htmlContent.Replace("PENALTY_AMOUNT", String.Format("{0:n}", decimal.Round(dnrp.penalty, 2)));

            if (!string.IsNullOrEmpty(dnrp.CouncilTreasurerSigFilen))
            {
                string sig = $"{rootUrl}/images/{dnrp.CouncilTreasurerSigFilen}";
                string tu = "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(sig));
                htmlContent = htmlContent.Replace("COUNCIL_TRESURER_SIG", tu);
            }
            if (!string.IsNullOrEmpty(dnrp.RevCoodinatorSigFilen))
            {
                htmlContent = htmlContent.Replace("REV_COORINATOR_SIG", $"{rootUrl}/images/{dnrp.RevCoodinatorSigFilen}");
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

            if (prepayment != null && finalTotal > 0)
            {
                finalTotal = finalTotal - prepayment.amount;
            }
            else if (finalTotal < 0)
            {
                finalTotal = 0;
            }

            htmlContent = htmlContent.Replace("AMOUNT_PAID", String.Format("{0:n}", decimal.Round(amountPaid, 2)));
            htmlContent = htmlContent.Replace("FINAL_TOTAL", String.Format("{0:n}", decimal.Round(finalTotal, 2)));

            if (finalTotal == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", CurrencyWords.ConvertToWords(finalTotal.ToString()));
            }

            htmlContent = htmlContent.Replace("PREPAYMENT", $"{String.Format("{0:n}", decimal.Round((prepayment == null ? 0 : prepayment.amount), 2))} naira");

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

        public async Task<byte[]> PopulateReportHtml(string htmlContent, long[] billingno,
            string rootUrl, string createdBy)
        {
            List<string> lst = new List<string>();
            foreach (var tm in billingno)
            {
                string val = await PopulateReportHtml1(htmlContent, tm, rootUrl, createdBy);
                lst.Add(val);
            }

            byte[] result = _pdfService.GetPdf(lst.ToArray());

            return result;
        }

        //public async Task<byte[]> PopulateReportHtml(string htmlContent, DemandNoticeTaxpayersModel[] billingno,
        //    string rootUrl, string createdBy)
        //{
        //    List<string> lst = new List<string>();
        //    foreach (var tm in billingno)
        //    {
        //        string val = await PopulateReportHtml1(htmlContent, tm, rootUrl, createdBy);
        //        lst.Add(val);
        //    }

        //    byte[] result = _pdfService.GetPdf(lst.ToArray());

        //    return result;
        //}

        public async Task<string> PopulateReportHtmlBase64(string htmlContent, long billingno,
          string rootUrl, string createdBy)
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
            htmlContent = htmlContent.Replace("LAGOSLOGO", Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/images/lagoslogo.jpg")));
            htmlContent = htmlContent.Replace("LCDA_LOGO", Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/images/{dnrp.LcdaLogoFileName}")));
            htmlContent = htmlContent.Replace("dated", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));//./templates/lcdaLogo.jpeg
            htmlContent = htmlContent.Replace("BKIMAGE", Convert.ToBase64String(await File.ReadAllBytesAsync($"{rootUrl}/templates/lcdaLogo.jpeg")));

            //if (dnrp.BillingNumber.Length < 5)
            //{
            //    string v = "";
            //    for (int i = 0; i < 5 - dnrp.BillingNumber.Length; i++)
            //    {
            //        v = v + "0";
            //    }

            //    dnrp.BillingNumber = v + dnrp.BillingNumber;
            //}

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

            htmlContent = htmlContent.Replace("PATCH2", "");

            var taxCategory = await _taxService.GetTaxpayerCategory(dnrp.TaxpayerId);
            htmlContent = htmlContent.Replace("TAXPAYERCATEGORY", taxCategory.TaxpayerCategoryName);

            htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp, _bankCategory, taxCategory));
            htmlContent = htmlContent.Replace("ARREARS_AMMOUNT", String.Format("{0:n}", decimal.Round(dnrp.arrears, 2)));
            htmlContent = htmlContent.Replace("PENALTY_AMOUNT", String.Format("{0:n}", decimal.Round(dnrp.penalty, 2)));

            if (!string.IsNullOrEmpty(dnrp.CouncilTreasurerSigFilen))
            {
                htmlContent = htmlContent.Replace("COUNCIL_TRESURER_SIG", $"{rootUrl}/images/{dnrp.CouncilTreasurerSigFilen}");
            }
            if (!string.IsNullOrEmpty(dnrp.RevCoodinatorSigFilen))
            {
                htmlContent = htmlContent.Replace("REV_COORINATOR_SIG", $"{rootUrl}/images/{dnrp.RevCoodinatorSigFilen}");
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
            //if (finalTotal < 0)
            //{
            //}
            if (prepayment != null && finalTotal > 0)
            {
                finalTotal = finalTotal - prepayment.amount;
            }
            else if (finalTotal < 0)
            {
                finalTotal = 0;
            }

            htmlContent = htmlContent.Replace("AMOUNT_PAID", String.Format("{0:n}", decimal.Round(amountPaid, 2)));
            htmlContent = htmlContent.Replace("FINAL_TOTAL", String.Format("{0:n}", decimal.Round(finalTotal, 2)));

            if (finalTotal == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", CurrencyWords.ConvertToWords(finalTotal.ToString()));
            }

            htmlContent = htmlContent.Replace("PREPAYMENT", $"{String.Format("{0:n}", decimal.Round((prepayment == null ? 0 : prepayment.amount), 2))} naira");

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

        public async Task<string> LcdaTemlate(long billingno)
        {
            LcdaModel lgda = await lcdaService.ByBillingNumber(billingno);
            if (lgda == null)
            {
                throw new NotFoundException($" {billingno} parent not found");
            }
            List<LcdaPropertyModel> lstProperties = await listPropertyService.ByLcda(lgda.Id);
            var allowPayment = lstProperties
                .FirstOrDefault(x => x.PropertyKey == "ALLOW_PAYMENT_SERVICES" && x.PropertyStatus == "ACTIVE");
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.PropertyKey == "ALLOW_HEADER" && x.PropertyStatus == "ACTIVE");

            return CommonList.Template((allowPayment == null ? "0" : allowPayment.PropertyValue),
                (allowHeader == null ? "0" : allowHeader.PropertyValue));
        }

        public async Task<string> LcdaTemlateByLcda(Guid lcdaId)
        {
            List<LcdaPropertyModel> lstProperties = await listPropertyService.ByLcda(lcdaId);
            var allowPayment = lstProperties
                .FirstOrDefault(x => x.PropertyKey == "ALLOW_PAYMENT_SERVICES" && x.PropertyStatus == "ACTIVE");
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.PropertyKey == "ALLOW_HEADER" && x.PropertyStatus == "ACTIVE");

            return CommonList.Template((allowPayment == null ? "0" : allowPayment.PropertyValue),
                (allowHeader == null ? "0" : allowHeader.PropertyValue));
        }

        public async Task<string> ReceiptTemlate(long billingno)
        {
            LcdaModel lgda = await lcdaService.ByBillingNumber(billingno);
            if (lgda == null)
            {
                throw new NotFoundException($" {billingno} parent not found");
            }
            List<LcdaPropertyModel> lstProperties = await listPropertyService.ByLcda(lgda.Id);
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.PropertyKey == "ALLOW_HEADER" && x.PropertyStatus == "ACTIVE");

            return CommonList.ReceiptTemplate((allowHeader == null ? "0" : allowHeader.PropertyValue));
        }


    }
}
