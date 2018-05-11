using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dnt")]
    public class DemandNoticeTaxpayerController : Controller
    {
        private IDnTaxpayer dnTaxpayer;
        private IDemandNoticeTaxpayerService demandNoticeTaxpayerService;
        public DemandNoticeTaxpayerController(IDnTaxpayer _dnTaxpayer,
            IDemandNoticeTaxpayerService _demandNoticeTaxpayerService)
        {
            dnTaxpayer = _dnTaxpayer;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
        }

        [HttpGet("billingno/{billingno}")]
        public async Task<object> GetByBillingNumber([FromRoute]string billingno)
        {
            if (string.IsNullOrEmpty(billingno))
            {
                return BadRequest(new Response()
                {
                    code = Utilities.MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Billing number is required"
                });
            }
            var tu = await dnTaxpayer.ByBillingNo(billingno);

            return Ok(new Response()
            {
                code = Utilities.MsgCode_Enum.SUCCESS,
                data = new object[] { tu }
            });
        }

        [HttpGet("batchno/{batchno}")]
        public async Task<object> GetByBatchNo([FromRoute]string batchno, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (string.IsNullOrEmpty(batchno))
            {
                throw new InvalidCredentialsException("Batch number is required");
            }
            return await dnTaxpayer.GetDNTaxpayerByBatchIdAsync(batchno, new Models.PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
        }

        [HttpGet("cancel/{billingno}")]
        public async Task<object> CancelDemandNotice([FromRoute] string billingno)
        {
            if (string.IsNullOrEmpty(billingno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Billing number is required"
                });
            }

            Response response = await demandNoticeTaxpayerService.CancelTaxpayerDemandNoticeByBillingNo(billingno, User.Identity.Name);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("search/{query}")]
        public async Task<object> searchAsync([FromRoute] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            var results = await demandNoticeTaxpayerService.Search(query);
            return Ok(new Response() { code = MsgCode_Enum.SUCCESS, data = results });
        }
    }
}
