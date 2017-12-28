using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Exceptions;
using RemsNG.Utilities;
using RemsNG.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/payment")]
    public class DemanNoticePaymentController : Controller
    {
        private readonly IDNPaymentHistoryService dNPaymentHistoryService;
        private readonly IDemandNoticeTaxpayerService demandNoticeTaxpayerService;
        private ILogger logger;
        private IDNAmountDueMgtService amountDueMgtService;
        public DemanNoticePaymentController(IDNPaymentHistoryService _dNPaymentHistoryService,
            IDemandNoticeTaxpayerService _demandNoticeTaxpayerService, 
            ITaxpayerService _taxpayerService,ILoggerFactory loggerFactory,
            IDNAmountDueMgtService _amountDueMgtService)
        {
            dNPaymentHistoryService = _dNPaymentHistoryService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
            logger = loggerFactory.CreateLogger("Receipt payment");
            amountDueMgtService = _amountDueMgtService;
        }

        [HttpGet("{billingNumber}")]
        public async Task<List<DemandNoticePaymentHistory>> Get(string billingNumber)
        {
            return await dNPaymentHistoryService.ByBillingNumber(billingNumber);
        }

        [RemsRequirementAttribute("REGISTER_PAYMENT")]
        [HttpPost]
        public async Task<Response> Post([FromBody]DemandNoticePaymentHistory value)
        {
            if (value.amount < 1)
            {
                throw new InvalidCredentialsException("Amount is required");
            }
            else if (value.bankId == default(Guid))
            {
                throw new InvalidCredentialsException("Bank is required");
            }
            else if (string.IsNullOrEmpty(value.billingNumber))
            {
                throw new InvalidCredentialsException("Bank is required");
            }
            else if (string.IsNullOrEmpty(value.referenceNumber))
            {
                throw new InvalidCredentialsException("Reference number is required");
            }
            // 
            var taxpayeD = await demandNoticeTaxpayerService.TaxpayerMiniByBillingNo(value.billingNumber);
            if (taxpayeD == null)
            {
                throw new NotFoundException("Billing number not found");
            }

            // Lgda lgda = await taxpayerService.getLcda(t.taxpayerId);
            value.ownerId = taxpayeD.taxpayerId;
            value.createdBy = User.Identity.Name;
            value.paymentMode = PaymentModeEnum.BANKS.ToString();
            return await dNPaymentHistoryService.AddAsync(value);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("bylcda")]
        [HttpGet]
        public async Task<object> BylcdaPaginated([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            Guid id = ClaimExtension.GetDomainId(User.Claims.ToArray());

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await dNPaymentHistoryService.ByLcdaId(id, new Models.PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }

        [RemsRequirementAttribute("APPROVE_PAYMENT")]
        [HttpPost("changestatus/{id}")]
        public async Task<object> ApprovePayment(Guid id)
        {
            if (id == default(Guid))
            {
                logger.LogWarning("Payment id is invalid", id);
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Please refresh your page an try again"
                });
            }

            DemandNoticePaymentHistory dnph = await dNPaymentHistoryService.ById(id);
            if (dnph == null)
            {
                logger.LogWarning("Payment identity is invalid", id);
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Payment not found"
                });
            }
            if ((dnph.amount+dnph.charges) < 1)
            {
                logger.LogWarning("Payment amount must be more than zero", dnph);
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Please refresh your page an try again"
                });
            }
            List<DNAmountDueModel> paymentDueList = await amountDueMgtService.ByBillingNo(dnph.billingNumber);
            if ((dnph.amount+dnph.charges) == paymentDueList.Sum(x=>x.itemAmount))
            {
                //full payment
                //update demand notice taxpayer table
                //update all payment demand due list
                //update payment as full payment
            }
            else
            {
                //part payment
            }

            return BadRequest(new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = "Feature under construction"
            });
        }
    }
}
