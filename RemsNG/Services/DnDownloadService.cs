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
    public class DnDownloadService : IDnDownloadService
    {
        private readonly IDemandNoticeTaxpayerService dnts;
        private readonly IDemandNoticeCharges chargesService;
        private readonly IDemandNoticeDownloadHistory demandNoticeDownloadHistory;
        private readonly ILcdaService lcdaService;
        private IListPropertyService listPropertyService;
        private ISectorService sectorService;
        public DnDownloadService(IDemandNoticeTaxpayerService _dnts,
            IDemandNoticeCharges _chargesService,
            IDemandNoticeDownloadHistory _demandNoticeDownloadHistory,
            ILcdaService _lcdaService,
            IListPropertyService _listPropertyService,
            ISectorService _sectorService)
        {
            dnts = _dnts;
            chargesService = _chargesService;
            demandNoticeDownloadHistory = _demandNoticeDownloadHistory;
            lcdaService = _lcdaService;
            listPropertyService = _listPropertyService;
            sectorService = _sectorService;
        }

        public async Task<string> PopulateReceiptHtml(string htmlContent, string rootUrl,
            string createdBy, DemandNoticePaymentHistory dnph)
        {
            DemandNoticeReportModel dnrp = await dnts.ByBillingNo(dnph.billingNumber);
            Sector sector = await sectorService.ByTaxpayerId(dnrp.taxpayerId);

            htmlContent = htmlContent.Replace("LOCAL_GOVERNMENT_NAME", dnrp.domainName);
            htmlContent = htmlContent.Replace("LCDA_NAME", dnrp.lcdaName);
            htmlContent = htmlContent.Replace("LCDA_ADDRESS", dnrp.lcdaAddress);
            htmlContent = htmlContent.Replace("LCDA_STATE", dnrp.lcdaState);
            htmlContent = htmlContent.Replace("LAGOSLOGO", $"{rootUrl}/images/lagoslogo.jpg");
            htmlContent = htmlContent.Replace("LCDA_LOGO", $"{rootUrl}/images/{dnrp.lcdaLogoFileName}");
            htmlContent = htmlContent.Replace("BILL_NO", $"{sector.prefix}{dnrp.billingNumber}");
            htmlContent = htmlContent.Replace("PAYER_NAME", dnrp.taxpayersName);
            htmlContent = htmlContent.Replace("PAYER_ADDRESS", dnrp.addressName);
            htmlContent = htmlContent.Replace("CURRENT_DATE", DateTime.Now.ToString("dd-MM-yyyy"));
            htmlContent = htmlContent.Replace("BILLING_YEAR", dnrp.billingYr.ToString());
            htmlContent = htmlContent.Replace("WARD_NAME", dnrp.wardName);
            htmlContent = htmlContent.Replace("TOTAL_AMOUNT", $"{String.Format("{0:n}", decimal.Round(dnph.amount, 2))} naira");
            htmlContent = htmlContent.Replace("REFERENCE_NUMBER", dnph.referenceNumber);
            htmlContent = htmlContent.Replace("REFERENCE_NUMBER", dnph.referenceNumber);//PAYMENT_STATUS
            htmlContent = htmlContent.Replace("PAYMENT_STATUS", dnrp.demandNoticeStatus);//PAYMENT_STATUS
            if (dnph.amount == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD",
                    CurrencyWords.ConvertToWords(decimal.Round(dnph.amount, 2).ToString()));
            }

            string discp = string.Join(',', dnrp.items.Select(x => x.itemTitle));

            htmlContent = htmlContent.Replace("DESCRIPTION", discp);

            return htmlContent;
        }

        public async Task<string> PopulateReportHtml(string htmlContent, string billingno,
            string rootUrl, string createdBy)
        {
            DemandNoticeReportModel dnrp = await dnts.ByBillingNo(billingno);
            if (dnrp.items.Count == 0)
            {
                return string.Empty;
            }
            Sector sector = await sectorService.ByTaxpayerId(dnrp.taxpayerId);

            htmlContent = htmlContent.Replace("LOCAL_GOVERNMENT_NAME", dnrp.domainName);
            htmlContent = htmlContent.Replace("LCDA_NAME", dnrp.lcdaName);
            htmlContent = htmlContent.Replace("LCDA_ADDRESS", dnrp.lcdaAddress);
            htmlContent = htmlContent.Replace("LCDA_STATE", dnrp.lcdaState);
            htmlContent = htmlContent.Replace("LAGOSLOGO", $"{rootUrl}/images/lagoslogo.jpg");
            htmlContent = htmlContent.Replace("LCDA_LOGO", $"{rootUrl}/images/{dnrp.lcdaLogoFileName}");

            if (sector != null)
            {
                htmlContent = htmlContent.Replace("BILL_NO", $"{sector.prefix}{dnrp.billingNumber}");
            }
            else
            {
                htmlContent = htmlContent.Replace("BILL_NO", dnrp.billingNumber);
            }
            htmlContent = htmlContent.Replace("PAYER_NAME", dnrp.taxpayersName);
            htmlContent = htmlContent.Replace("PAYER_ADDRESS", dnrp.addressName);
            htmlContent = htmlContent.Replace("CURRENT_DATE", DateTime.Now.ToString("dd-MM-yyyy"));
            htmlContent = htmlContent.Replace("BILLING_YEAR", dnrp.billingYr.ToString());
            htmlContent = htmlContent.Replace("WARD_NAME", dnrp.wardName);

            htmlContent = htmlContent.Replace("ITEMLIST", DemandNoticeComponents.HtmlBuildItems(dnrp));
            int partialItemCount = 0;
            
            htmlContent = htmlContent.Replace("PATCH2", "");

            htmlContent = htmlContent.Replace("BANKLIST", DemandNoticeComponents.HtmlBuildBanks(dnrp));
            htmlContent = htmlContent.Replace("ARREARS_AMMOUNT", String.Format("{0:n}", decimal.Round(dnrp.arrears, 2)));
            htmlContent = htmlContent.Replace("PENALTY_AMOUNT", String.Format("{0:n}", decimal.Round(dnrp.penalty, 2)));

            if (!string.IsNullOrEmpty(dnrp.councilTreasurerSigFilen))
            {
                htmlContent = htmlContent.Replace("COUNCIL_TRESURER_SIG", $"{rootUrl}/images/{dnrp.councilTreasurerSigFilen}");
            }
            if (!string.IsNullOrEmpty(dnrp.revCoodinatorSigFilen))
            {
                htmlContent = htmlContent.Replace("REV_COORINATOR_SIG", $"{rootUrl}/images/{dnrp.revCoodinatorSigFilen}");
            }
            htmlContent = htmlContent.Replace("TREASURER_MOBILE", string.IsNullOrEmpty(dnrp.councilTreasurerMobile) ? "nil" : dnrp.councilTreasurerMobile);
            decimal gtotal = dnrp.items.Sum(x => x.itemAmount) + dnrp.arrears + dnrp.penalty;
            htmlContent = htmlContent.Replace("GRAND_TOTAL", String.Format("{0:n}", decimal.Round(gtotal, 2)));

            htmlContent = htmlContent.Replace("CHARGES", String.Format("{0:n}", decimal.Round(dnrp.charges, 2)));
            decimal finalTotal = gtotal + dnrp.charges;
            htmlContent = htmlContent.Replace("FINAL_TOTAL", String.Format("{0:n}", decimal.Round(finalTotal, 2)));

            if (finalTotal == 0)
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", "Zero");
            }
            else
            {
                htmlContent = htmlContent.Replace("AMOUNT_IN_WORD", CurrencyWords.ConvertToWords(finalTotal.ToString()));
            }
            DemandNoticeDownloadHistory dndh = new DemandNoticeDownloadHistory();
            dndh.id = Guid.NewGuid();
            dndh.billingNumber = billingno;
            dndh.charges = dnrp.charges;
            dndh.createdBy = createdBy;
            dndh.dateCreated = DateTime.Now;
            dndh.grandTotal = gtotal;

            await demandNoticeDownloadHistory.Add(dndh);

            return htmlContent;
        }

        public async Task<string> LcdaTemlate(string billingno)
        {
            Lgda lgda = await lcdaService.ByBillingNumber(billingno);
            if (lgda == null)
            {
                throw new NotFoundException($" {billingno} parent not found");
            }
            List<LcdaProperty> lstProperties = await listPropertyService.ByLcda(lgda.id);
            var allowPayment = lstProperties
                .FirstOrDefault(x => x.propertyKey == "ALLOW_PAYMENT_SERVICES" && x.propertyStatus == "ACTIVE");
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.propertyKey == "ALLOW_HEADER" && x.propertyStatus == "ACTIVE");

            return CommonList.Template((allowPayment == null ? "0" : allowPayment.propertyValue),
                (allowHeader == null ? "0" : allowHeader.propertyValue));
        }

        public async Task<string> LcdaTemlateByLcda(Guid lcdaId)
        {
            List<LcdaProperty> lstProperties = await listPropertyService.ByLcda(lcdaId);
            var allowPayment = lstProperties
                .FirstOrDefault(x => x.propertyKey == "ALLOW_PAYMENT_SERVICES" && x.propertyStatus == "ACTIVE");
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.propertyKey == "ALLOW_HEADER" && x.propertyStatus == "ACTIVE");

            return CommonList.Template((allowPayment == null ? "0" : allowPayment.propertyValue),
                (allowHeader == null ? "0" : allowHeader.propertyValue));
        }

        public async Task<string> ReceiptTemlate(string billingno)
        {
            Lgda lgda = await lcdaService.ByBillingNumber(billingno);
            if (lgda == null)
            {
                throw new NotFoundException($" {billingno} parent not found");
            }
            List<LcdaProperty> lstProperties = await listPropertyService.ByLcda(lgda.id);
            var allowHeader = lstProperties
               .FirstOrDefault(x => x.propertyKey == "ALLOW_HEADER" && x.propertyStatus == "ACTIVE");

            return CommonList.ReceiptTemplate((allowHeader == null ? "0" : allowHeader.propertyValue));
        }

    }
}
