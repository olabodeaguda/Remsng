﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Security;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dndownload")]
    public class DemandNoticeDownloadController : Controller
    {
        private readonly IPdfService _pdfService;
        private IHostingEnvironment hostingEnvironment;
        private INodeServices nodeServices;
        private IDnDownloadManager dnd;
        private readonly ILogger logger;
        private readonly IBatchDwnRequestManager batchRequestService;
        private IDNPaymentHistoryManager paymentHistoryService;
        public DemandNoticeDownloadController(IHostingEnvironment _hostingEnvironment,
            INodeServices _nodeServices, IDnDownloadManager _dnd,
            IBatchDwnRequestManager _batchRequestService,
            IDNPaymentHistoryManager _paymentHistoryService,
            ILoggerFactory loggerFactory, IPdfService pdfService)
        {
            hostingEnvironment = _hostingEnvironment;
            nodeServices = _nodeServices;
            dnd = _dnd;
            batchRequestService = _batchRequestService;
            paymentHistoryService = _paymentHistoryService;
            logger = loggerFactory.CreateLogger("Demand Notice download");
            _pdfService = pdfService;
        }


        [RemsRequirementAttribute("SINGLE_DOWNLOAD")]
        [Route("single/{billingno}")]
        [HttpGet]
        public async Task<object> Get(string billingno)
        {
            if (string.IsNullOrEmpty(billingno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Billing number is required"
                });
            }

            string template = await dnd.LcdaTemlate(billingno);

            string rootUrl = hostingEnvironment.WebRootPath;
            var htmlContent = await System.IO.File.ReadAllTextAsync($"{rootUrl}/templates/{template}");

            htmlContent = await dnd.PopulateReportHtml(htmlContent, billingno, rootUrl, User.Identity.Name);
            htmlContent = htmlContent.Replace("PATCH1", "<br /><br /><br /><br /><br /><br /><br /><br />");
            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            // var result = _pdfService.GetPdf(htmlContent); 


            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [RemsRequirementAttribute("BULK_DOWNLOAD")]
        // GET: api/values
        [Route("bulk/{batchno}")]
        [HttpGet]
        public async Task<object> BulkZip(string batchno)
        {
            //log zip download by xxx user

            // HttpClient hc = new HttpClient();
            string rootUrl = hostingEnvironment.WebRootPath; //$"http://{Request.Host}";

            var result = await System.IO.File.ReadAllBytesAsync($"{rootUrl}/zipReports/{batchno}/{batchno}.zip");
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

        // GET api/values/5
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
        [Route("receipt/{id}")]
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
            HttpClient hc = new HttpClient();
            string template = await dnd.ReceiptTemlate(dnph.BillingNumber);

            string rootUrl = hostingEnvironment.WebRootPath;
            var htmlContent = await System.IO.File.ReadAllTextAsync($"{rootUrl}/templates/{template}");

            htmlContent = await dnd.PopulateReceiptHtml(htmlContent, rootUrl, User.Identity.Name, dnph);
            htmlContent = htmlContent.Replace("BANK_NAME", dnph.BankName);
            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [HttpPost("downloadbytx")]
        public async Task<IActionResult> DownloadDNbyTaxpayer([FromBody] Guid[] dnId)
        {
            return Ok();
        }
    }
}
