using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public ReportDownloadController(IReportService _reportService,
            IExcelService _excelService, ILcdaService _lcdaService)
        {
            reportService = _reportService;
            excelService = _excelService;
            lcdaService = _lcdaService;
        }

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

            Domain domain = await lcdaService.GetDomain(lgda.id);

            byte[] result = await excelService.WriteReportSummary(current, previous, (domain == null ? "Unknown" : domain.domainName), lgda.lcdaName, sd, ed);

            // System.IO.File.WriteAllBytes(@"C:\reports\report.xlsx", result);

            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }
    }
}
