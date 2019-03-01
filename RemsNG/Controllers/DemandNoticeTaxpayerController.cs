using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dnt")]
    public class DemandNoticeTaxpayerController : Controller
    {
        private IDnTaxpayerManager dnTaxpayer;
        private IDemandNoticeTaxpayerManager demandNoticeTaxpayerService;
        public DemandNoticeTaxpayerController(IDnTaxpayerManager _dnTaxpayer,
            IDemandNoticeTaxpayerManager _demandNoticeTaxpayerService)
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
                    code = MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Billing number is required"
                });
            }
            var tu = await dnTaxpayer.ByBillingNo(billingno);

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = new object[] { tu }
            });
        }

        [HttpGet("batchno/{batchno}")]
        public async Task<object> GetByBatchNo([FromRoute]string batchno,
            [FromHeader] string pageSize,
            [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (string.IsNullOrEmpty(batchno))
            {
                throw new InvalidCredentialsException("Batch number is required");
            }
            return await dnTaxpayer.GetDNTaxpayerByBatchIdAsync(batchno, new PageModel()
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

        [HttpGet("payable/{taxpayerId}")]
        public async Task<IActionResult> TaxpayerPayable(Guid taxpayerId)
        {
            return Ok(await demandNoticeTaxpayerService.GetTaxpayerPayables(taxpayerId));
        }

        [HttpGet("movetobill/{billno}")]
        public async Task<IActionResult> MovetoBill(string billno)
        {
            if (string.IsNullOrEmpty(billno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bill number is required"
                });
            }
            var r = await demandNoticeTaxpayerService.TaxpayerMiniByBillingNo(billno);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = $"{billno} can not be found"
                });
            }

            if (!r.IsUnbilled)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{billno} is already been moved to billed"
                });
            }

            bool result = await demandNoticeTaxpayerService.MoveToBill(billno);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "request was succesful"
                });
            }
            return BadRequest(new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = "Request failed.Please try again"
            });
        }

        [HttpGet("movetoUnbill/{billno}")]
        public async Task<IActionResult> MovetoUnBill(string billno)
        {
            if (string.IsNullOrEmpty(billno))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bill number is required"
                });
            }
            var r = await demandNoticeTaxpayerService.TaxpayerMiniByBillingNo(billno);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = $"{billno} can not be found"
                });
            }

            if (r.IsUnbilled)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{billno} is already unbilled"
                });
            }

            bool result = await demandNoticeTaxpayerService.MoveToUnBills(billno);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "request was succesful"
                });
            }
            return BadRequest(new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = "Request failed.Please try again"
            });
        }

        [HttpPost("searchbylcda/{pageNum}/{pageSize}")]
        public async Task<IActionResult> SearchDemandNotice([FromBody] SearchDNModel model, int pageNum = 1, int pageSize = 20)
        {
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            return Ok(await demandNoticeTaxpayerService.SearchByLcdaId(modl, new PageModel()
            {
                PageNum = pageNum,
                PageSize = pageSize
            }, lcdaId));
        }

    }
}
