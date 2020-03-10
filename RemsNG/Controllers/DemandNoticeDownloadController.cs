using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using RemsNG.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dndownload")]
    public class DemandNoticeDownloadController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly IPdfService _pdfService;
        private IHostingEnvironment hostingEnvironment;
        //private INodeServices nodeServices;
        private IDnDownloadManager dnd;
        private readonly ILogger logger;
        private readonly IBatchDwnRequestManager batchRequestService;
        private IDNPaymentHistoryManager paymentHistoryService;
        private readonly TemplateDetail _templateDetails;
        private IDemandNoticeTaxpayerManager _dnTxpayer;
        public DemandNoticeDownloadController(IHostingEnvironment _hostingEnvironment,
             IDnDownloadManager _dnd,
            IBatchDwnRequestManager _batchRequestService,
            IDNPaymentHistoryManager _paymentHistoryService,
            ILoggerFactory loggerFactory, IPdfService pdfService,
            TemplateDetail templateDetail, DbContext dbContext, IDemandNoticeTaxpayerManager demandNoticeTaxpayerManager)
        {
            _dnTxpayer = demandNoticeTaxpayerManager;
            _dbContext = dbContext;
            _templateDetails = templateDetail;
            hostingEnvironment = _hostingEnvironment;
            dnd = _dnd;
            batchRequestService = _batchRequestService;
            paymentHistoryService = _paymentHistoryService;
            logger = loggerFactory.CreateLogger("Demand Notice download");
            _pdfService = pdfService;
        }

        [RemsRequirementAttribute("SINGLE_DOWNLOAD")]
        [HttpGet("single/{billingno}")]
        public async Task<object> Get(long billingno)
        {
            if (billingno == default(long))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Billing number is required"
                });
            }

            var result = await dnd.GenerateDemandNotice(new long[] { billingno }, User.Identity.Name);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("BULK_DOWNLOAD")]
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkDownloadPdf([FromBody] long[] billingNo)
        {
            if (billingNo.Length <= 0)
            {
                return BadRequest(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Please select demand notice to download"
                });
            }

            var result = await dnd.GenerateDemandNotice(billingNo, User.Identity.Name);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("BULK_DOWNLOAD")]
        [HttpGet("bulk/{batchno}")]
        public async Task<object> BulkZip(string batchno)
        {
            //log zip download by xxx user

            // HttpClient hc = new HttpClient();
            string rootUrl = hostingEnvironment.WebRootPath; //$"http://{Request.Host}";

            var result = await System.IO.File.ReadAllBytesAsync($"{_templateDetails.ZipRepository}/{batchno}/{batchno}.zip");
            //var result = await hc.GetByteArrayAsync($"{rootUrl}/zipReports/{batchno}/{batchno}.zip");
            if (result.Length < 1)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = $"{batchno}.zip not found"
                });
            }

            HttpContext.Response.ContentType = "application/zip";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [HttpGet("{batchno}")]
        public async Task<object> ByBatchNo(string batchno)
        {
            if (string.IsNullOrEmpty(batchno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Batch number is required"
                });
            }

            return await batchRequestService.ListByBatchNo(batchno);
        }

        [RemsRequirementAttribute("BULK_DOWNLOAD")]
        [HttpPost("{batchno}")]
        public async Task<object> Post(string batchno)
        {
            if (string.IsNullOrEmpty(batchno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Batch number is required"
                });
            }
            BatchDemandNoticeModel batchDemanNoticeModel = new BatchDemandNoticeModel();
            batchDemanNoticeModel.batchNo = batchno;
            batchDemanNoticeModel.createdBy = User.Identity.Name;
            batchDemanNoticeModel.id = Guid.NewGuid();
            batchDemanNoticeModel.lcdaId = Guid.Empty;
            batchDemanNoticeModel.requestStatus = DemandNoticeStatus.PENDING.ToString();

            Response response = await batchRequestService.AddBatchRequest(batchDemanNoticeModel);
            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [RemsRequirementAttribute("DOWNLOAD_RECEIPT")]
        [HttpGet("receipt/{id}")]
        public async Task<object> DownloadReciept(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidCredentialsException("Invalid request");
            }
            DemandNoticePaymentHistoryModel dnph = await paymentHistoryService.ByIdExtended(id);
            if (dnph == null)
            {
                throw new NotFoundException("Request not found");
            }

            var result = await dnd.GenerateReceipt(User.Identity.Name, dnph);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [HttpPost("downloadbytx")]
        public async Task<IActionResult> DownloadDNbyTaxpayer([FromBody] Guid[] dnId)
        {
            return Ok();
        }

        [RemsRequirementAttribute("BULK_DOWNLOAD")]
        [HttpPost("reminder")]
        public async Task<IActionResult> Reminder([FromBody] long[] billingNo)
        {
            if (billingNo.Length <= 0)
            {
                return BadRequest(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Please select demand notice to download"
                });
            }

            var result = await dnd.GenerateReminder(billingNo, User.Identity.Name);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [AllowAnonymous]
        [HttpGet("sync")]
        public async Task<IActionResult> SyncPaymentStatus()
        {
            var list = await _dbContext.Set<DemandNoticeTaxpayer>().Where(s => s.BillingYr == 2020 && s.DemandNoticeStatus == "PAID").ToListAsync();
            var payment = await _dbContext.Set<DemandNoticePaymentHistory>().Where(x => list.Any(p => p.BillingNumber == x.BillingNumber)).ToListAsync();

            foreach (var tm in list)
            {
                var amountPaid = payment.Where(x => x.BillingNumber == tm.BillingNumber).Sum(x => x.Amount);
                var dnrp = await _dnTxpayer.ByBillingNo(tm.BillingNumber);
                if (dnrp != null)
                {
                    decimal finalTotal = dnrp.items.Sum(x => x.itemAmount) + dnrp.arrears + dnrp.penalty + dnrp.charges;
                    DemandNoticeStatus status = default(DemandNoticeStatus);

                    if (amountPaid > finalTotal)
                        status = DemandNoticeStatus.OVERPAYMENT;
                    else if (finalTotal == amountPaid)
                        status = DemandNoticeStatus.PAID;
                    else if (amountPaid < finalTotal)
                        status = DemandNoticeStatus.PART_PAYMENT;

                    if (tm.DemandNoticeStatus != status.ToString())
                        tm.DemandNoticeStatus = status.ToString();
                }
            }

            int count = await _dbContext.SaveChangesAsync();


            return Ok(count);
        }
    }
}
