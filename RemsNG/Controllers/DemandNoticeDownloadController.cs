using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Exceptions;
using RemsNG.Security;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dndownload")]
    public class DemandNoticeDownloadController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        private INodeServices nodeServices;
        private IDnDownloadService dnd;
        private readonly ILogger logger;
        private readonly IBatchDwnRequestService batchRequestService;
        private IDNPaymentHistoryService paymentHistoryService;
        public DemandNoticeDownloadController(IHostingEnvironment _hostingEnvironment,
            INodeServices _nodeServices, IDnDownloadService _dnd, 
            IBatchDwnRequestService _batchRequestService,
            IDNPaymentHistoryService _paymentHistoryService,
            ILoggerFactory loggerFactory)
        {
            this.hostingEnvironment = _hostingEnvironment;
            nodeServices = _nodeServices;
            dnd = _dnd;
            batchRequestService = _batchRequestService;
            paymentHistoryService = _paymentHistoryService;
            logger = loggerFactory.CreateLogger("Demand Notice download");
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

          //  HttpClient hc = new HttpClient();
           // string rootUrl = $"http://{Request.Host}";
            string template = await dnd.LcdaTemlate(billingno);
            // var htmlContent = await hc.GetStringAsync($"{rootUrl}/templates/{template}");

            logger.LogInformation(Request.Host.ToString());
            string rootUrl = hostingEnvironment.WebRootPath; //$"http://{Request.Host}";
            var htmlContent = await System.IO.File.ReadAllTextAsync($"{rootUrl}/templates/{template}");


            htmlContent = await dnd.PopulateReportHtml(htmlContent, billingno, rootUrl, User.Identity.Name);
            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

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
            if(id == Guid.Empty)
            {
                throw new InvalidCredentialsException("Invalid request");
            }
            DemandNoticePaymentHistory dnph = await paymentHistoryService.ById(id);
            if (dnph == null)
            {
                throw new NotFoundException("Request not found");
            }
            HttpClient hc = new HttpClient();
           // string rootUrl = $"http://{Request.Host}";
            string template = await dnd.ReceiptTemlate(dnph.billingNumber);
           // var htmlContent = await hc.GetStringAsync($"{rootUrl}/templates/{template}");

            string rootUrl = hostingEnvironment.WebRootPath; //$"http://{Request.Host}";
            var htmlContent = await System.IO.File.ReadAllTextAsync($"{rootUrl}/templates/{template}");

            htmlContent = await dnd.PopulateReceiptHtml(htmlContent, rootUrl, User.Identity.Name,dnph);
            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();


        }
    }
}
