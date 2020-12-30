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
using System.IO;
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
        public async Task<IActionResult> Get(string startDate, string endDate, [FromQuery]int billingyr)
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

            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
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

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed, billingyr);
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

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");
            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();
        }


        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("revenue/{startDate}/{endDate}/{billingyr}")]
        public async Task<IActionResult> Get2(string startDate, string endDate, int billingyr)
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

            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
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

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed, billingyr);
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

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");
            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();
        }



        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("outstandingbybillno/{startDate}/{endDate}/{billingyr}")]
        public async Task<IActionResult> GetByBIllNumber(string startDate, string endDate, int billingyr)
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

            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
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

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed, billingyr);

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

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");
            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("outstandingbybillnoseperate/{startDate}/{endDate}/{billingyr}")]
        public async Task<IActionResult> GetByBIllNumberReport(string startDate, string endDate, int billingyr)
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
            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
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
            List<ItemReportSummaryModel> all = await reportService.ByDate(sd, ed, billingyr);
            List<ItemReportSummaryModel> valid = await reportService.ByDate(sd, ed, billingyr);

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

            var result = await excelService.WriteReportSummaryConsolidatedSeperateV2(valid,
                (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnph);

            // var result1 = await System.IO.File.ReadAllBytesAsync(result);

            //HttpContext.Response.ContentType = "application/octet-stream";
            // HttpContext.Response.Body.Write(result1, 0, result1.Length);
            //return new ContentResult();

            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");

            //return File(stream, "application/octet-stream", "FileDownloadName.ext");
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

        [HttpGet("category/{startDate}/{endDate}/{billingyr}")]
        public async Task<IActionResult> GetByCategoryReport(string startDate, string endDate, int billingyr)
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

            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
                });
            }
            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            var ids = await reportService.AllIdsByDate(sd, ed, billingyr);
            if (ids.billNumbers.Length < 0)
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = $"No record(s) found"
                });


            List<DemandNoticeItemModel> dnitem = await reportService.ReportitemsByCategory(ids.billNumbers);
            List<DemandNoticePenaltyModel> dnPenalty = await reportService.ReportPenaltyByCategory(ids.taxpayerIds);
            List<DemandNoticeArrearsModel> dnArrears = await reportService.ReportArrearsByCategory(ids.taxpayerIds);

            string category = string.Empty;
            if (Request.Headers.ContainsKey("category"))
                category = Request.Headers["category"].ToString();

            if (!string.IsNullOrEmpty(category))
            {
                dnitem = dnitem.Where(x => x.category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                dnPenalty = dnPenalty.Where(x => x.category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                dnArrears = dnArrears.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
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

            DomainModel domain = await lcdaService.GetDomain(lgda.Id);

            var result = await excelService.WriteReportCategory(
               (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnitem, dnPenalty, dnArrears);

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");
            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();
        }


        [HttpGet("categorydetails/{startDate}/{endDate}/{billingyr}")]
        public async Task<IActionResult> GetByCategoryReportDetails(string startDate, string endDate, int billingyr)
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

            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
                });
            }
            DateTime sd = DateTime.ParseExact(startDate, "dd-MM-yyyy", null);
            DateTime ed = DateTime.ParseExact(endDate, "dd-MM-yyyy", null);
            ed = ed.AddHours(23);
            ed = ed.AddMinutes(59);

            string category = string.Empty;
            if (Request.Headers.ContainsKey("category"))
                category = Request.Headers["category"].ToString();

            List<ItemReportSummaryModel> lst = await reportService.GetReportByCategory(sd, ed, category, billingyr);

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

            //var result = await excelService.WriteReportCategory(
            //   (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnitem, dnPenalty, dnArrears);


            List<DemandNoticePaymentHistoryModel> dnph = await dNPaymentHistoryService.ByBillingNumbers(lst.Select(x => x.billingNo).ToArray());

            byte[] result = await excelService.TaxpayerReportByWard(lst,
              (domain == null ? "Unknown" : domain.DomainName), lgda.LcdaName, sd, ed, dnph, string.IsNullOrEmpty(category) ? "All" : category);

            //return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            return new FileStreamResult(new MemoryStream(result), "application/octet-stream");

            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(result, 0, result.Length);
            //return new ContentResult();
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



            return new FileStreamResult(new MemoryStream(byt), "application/octet-stream");
            //HttpContext.Response.ContentType = "application/octet-stream";
            //HttpContext.Response.Body.Write(byt, 0, byt.Length);
            //return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("byward/{startDate}/{endDate}/{billingyr}")]
        public async Task<object> ByWard(string startDate, string endDate, int billingyr)
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
            if (billingyr <= default(int))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing year is required"
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

            List<ItemReportSummaryModel> current = await reportService.ByDate(sd, ed, billingyr);

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

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
