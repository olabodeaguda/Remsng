using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/report")]
    public class ReportDownloadController : Controller
    {
        private readonly ITaxpayerManager _taxpayerService;
        private readonly IReportManager reportService;
        private readonly IExcelService excelService;
        private readonly ILcdaManager lcdaService;
        private readonly IDNPaymentHistoryManager dNPaymentHistoryService;
        private readonly IHostingEnvironment hostingEnvironment;
        public ReportDownloadController(IReportManager _reportService,
            IExcelService _excelService, ILcdaManager _lcdaService,
            IDNPaymentHistoryManager _dNPaymentHistoryService,
            IHostingEnvironment _hostingEnvironment, ITaxpayerManager taxpayerService)
        {
            reportService = _reportService;
            excelService = _excelService;
            lcdaService = _lcdaService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            hostingEnvironment = _hostingEnvironment;
            _taxpayerService = taxpayerService;
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("revenue/{startDate}/{endDate}")]
        public async Task<object> Get(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Start date is required"
                });
            }
            else if (string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "End date is required"
                });
            }
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            LcdaModel lgda = await lcdaService.Get(lcdaId);
            if (lgda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.UNKNOWN,
                    description = $"Log on user unknown"
                });
            }

            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed);
            //List<ItemReportSummaryModel> previous = await reportService.ByDate(
            //    new DateTime(sd.Year, 1, 1, 0, 0, 0), sd);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            byte[] result = await excelService.WriteReportSummary(current,
                (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("outstandingbybillno/{startDate}/{endDate}")]
        public async Task<object> GetByBIllNumber(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Start date is required"
                });
            }
            else if (string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "End date is required"
                });
            }
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            LcdaModel lgda = await lcdaService.Get(lcdaId);
            if (lgda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.UNKNOWN,
                    description = $"Log on user unknown"
                });
            }

            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed);

            // getPayment history 
            long[] billnums = current.Select(x => x.billingNo).ToArray();//.FormatString();

            List<DemandNoticePaymentHistoryModel> dnph = await dNPaymentHistoryService.ByBillingNumbers(billnums);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            // byte[] result = await excelService.WriteReportSummary(current, (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed);
            byte[] result = await excelService.WriteReportSummaryConsolidated(current,
                (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnph);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("outstandingbybillnoseperate/{startDate}/{endDate}")]
        public async Task<object> GetByBIllNumberReport(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Start date is required"
                });
            }
            else if (string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "End date is required"
                });
            }
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            LcdaModel lgda = await lcdaService.Get(lcdaId);
            if (lgda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.UNKNOWN,
                    description = $"Log on user unknown"
                });
            }

            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);
            List<ItemReportSummaryModel> all = await reportService.ByDate(sd, ed);
            List<ItemReportSummaryModel> valid = await reportService.ByDate(sd, ed);

            // getPayment history 
            long[] billnums = all.Select(x => x.billingNo).ToArray();//.FormatString();
            List<DemandNoticePaymentHistoryModel> dnph = await dNPaymentHistoryService.ByBillingNumbers(billnums);

            if (all.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            var result = await excelService.WriteReportSummaryConsolidatedSeperate(valid,
                (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnph);

            // var result1 = await System.IO.File.ReadAllBytesAsync(result);

            //HttpContext.Response.ContentType = "application/octet-stream";
            // HttpContext.Response.Body.Write(result1, 0, result1.Length);
            //return new ContentResult();

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = result
            });
        }

        [HttpGet("reportreceivables")]
        public async Task<object> GetReport()
        {
            var result = await reportService.ReportByCurrentYear();
            return Ok(result.Select(x => new
            {
                label = x.wardName,
                receivables = (x.itemAmount - x.amountPaid),
                amountPaid = x.amountPaid
            }).OrderBy(x => x.label));
        }

        [HttpGet("reportrevenue")]
        public async Task<object> GetRevenue()
        {
            var result = await reportService.ReportByCurrentYear();
            return Ok(result.Select(x => new
            {
                label = x.wardName,
                value = x.amountPaid
            }));
        }

        [HttpGet("category/{startDate}/{endDate}")]
        public async Task<IActionResult> GetByCategoryReport(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Start date is required"
                });
            }
            else if (string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "End date is required"
                });
            }

            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            List<DemandNoticeItemModel> dnitem = await reportService.ReportitemsByCategory(sd, ed);
            List<DemandNoticePenaltyModel> dnPenalty = await reportService.ReportPenaltyByCategory(sd, ed);
            List<DemandNoticeArrearsModel> dnArrears = await reportService.ReportArrearsByCategory(sd, ed);

            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            LcdaModel lgda = await lcdaService.Get(lcdaId);
            if (lgda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.UNKNOWN,
                    description = $"Log on user unknown"
                });
            }

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            var result = await excelService.WriteReportCategory(
               (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnitem, dnPenalty, dnArrears);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [HttpGet("withoutdn/{billingYr}")]
        public async Task<IActionResult> GetTaxpayeWithOutDemandNotice(int billingYr)
        {
            if (billingYr == default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"Billing year is required"
                });
            }

            var taxpayer = await _taxpayerService.UnBilledTaxpayer(billingYr);
            if (taxpayer.Length <= 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"All taxpayer has been billed for this year"
                });
            }

            byte[] byt = await excelService.TaxpayerWithOutDemandNotice(taxpayer, billingYr);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(byt, 0, byt.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("byward/{startDate}/{endDate}")]
        public async Task<object> ByWard(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Start date is required"
                });
            }
            else if (string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "End date is required"
                });
            }
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            LcdaModel lgda = await lcdaService.Get(lcdaId);
            if (lgda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.UNKNOWN,
                    description = $"Log on user unknown"
                });
            }

            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed);
            //List<ItemReportSummaryModel> previous = await reportService.ByDate(
            //    new DateTime(sd.Year, 1, 1, 0, 0, 0), sd);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            byte[] result = await excelService.TaxpayerReportByWard(current,
                (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }
    }
}
