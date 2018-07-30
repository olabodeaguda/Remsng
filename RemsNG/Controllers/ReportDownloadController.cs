using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Security;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/report")]
    public class ReportDownloadController : Controller
    {
        private readonly IReportService reportService;
        private readonly IExcelService excelService;
        private readonly ILcdaService lcdaService;
        private readonly IDNPaymentHistoryService dNPaymentHistoryService;
        private IHostingEnvironment hostingEnvironment;
        public ReportDownloadController(IReportService _reportService,
            IExcelService _excelService, ILcdaService _lcdaService,
            IDNPaymentHistoryService _dNPaymentHistoryService,
            IHostingEnvironment _hostingEnvironment)
        {
            reportService = _reportService;
            excelService = _excelService;
            lcdaService = _lcdaService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            hostingEnvironment = _hostingEnvironment;
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
            Lgda lgda = await lcdaService.Get(lcdaId);
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

            List<ItemReportSummaryModel> current = await reportService.ByDate2(sd, ed);
            List<ItemReportSummaryModel> previous = await reportService.ByDate2(
                new DateTime(sd.Year, 1, 1, 0, 0, 0), sd);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            Domain domain = await lcdaService.GetDomain(lgda.id);

            byte[] result = await excelService.WriteReportSummary(current, previous, (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("revenuehtml/{startDate}/{endDate}")]
        public async Task<object> GetHtml(string startDate, string endDate)
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
            Lgda lgda = await lcdaService.Get(lcdaId);
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
            List<ItemReportSummaryModel> previous = await reportService.ByDate(
                new DateTime(sd.Year, 1, 1, 0, 0, 0), sd);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            string result = await reportService.HtmlByDate(current, previous);
            if (!string.IsNullOrEmpty(result))
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = result
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = string.Empty
                };
            }
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
            Lgda lgda = await lcdaService.Get(lcdaId);
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

            List<ItemReportSummaryModel> current = await reportService.ByDate2(sd, ed);

            // getPayment history 
            string billnums = current.Select(x => x.billingNo).ToArray().FormatString();

            List<DemandNoticePaymentHistory> dnph = await dNPaymentHistoryService.ByBillingNumbers(billnums);

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            Domain domain = await lcdaService.GetDomain(lgda.id);

            // byte[] result = await excelService.WriteReportSummary(current, (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed);
            byte[] result = await excelService.WriteReportSummaryConsolidated(current,
                (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed, dnph);

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
            Lgda lgda = await lcdaService.Get(lcdaId);
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
            List<ItemReportSummaryModel> valid = await reportService.ByDate2(sd, ed);

            // getPayment history 
            string billnums = all.Select(x => x.billingNo).ToArray().FormatString();
            List<DemandNoticePaymentHistory> dnph = await dNPaymentHistoryService.ByBillingNumbers(billnums);

            if (all.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            Domain domain = await lcdaService.GetDomain(lgda.id);

            var result = await excelService.WriteReportSummaryConsolidatedSeperate(valid,
                (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed, dnph);

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

        [RemsRequirementAttribute("DOWNLOAD_REPORT")]
        [HttpGet("outstandingbybillnohtml/{startDate}/{endDate}")]
        public async Task<object> GetByBIllNumberHtml(string startDate, string endDate)
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
            Lgda lgda = await lcdaService.Get(lcdaId);
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

            if (current.Count < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Zero record(s) found"
                });
            }

            string result = await reportService.HtmlByDate(current);
            if (!string.IsNullOrEmpty(result))
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = result
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = string.Empty
                };
            }
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
    }
}
