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
        private IAbstractService abstractService;
        public DemanNoticePaymentController(IDNPaymentHistoryService _dNPaymentHistoryService,
            IDemandNoticeTaxpayerService _demandNoticeTaxpayerService,
            ITaxpayerService _taxpayerService, ILoggerFactory loggerFactory,
            IDNAmountDueMgtService _amountDueMgtService, IAbstractService _abstractService)
        {
            dNPaymentHistoryService = _dNPaymentHistoryService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
            logger = loggerFactory.CreateLogger("Receipt payment");
            amountDueMgtService = _amountDueMgtService;
            abstractService = _abstractService;
        }

        [HttpGet("{billingNumber}")]
        public async Task<List<DemandNoticePaymentHistory>> Get(string billingNumber)
        {
            return await dNPaymentHistoryService.ByBillingNumber(billingNumber);
        }

        [RemsRequirementAttribute("REGISTER_PAYMENT")]
        [HttpPost]
        public async Task<Response> Post([FromBody]DemandNoticePaymentHistory value, [FromHeader] string dateCreated)
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
            if (!string.IsNullOrEmpty(dateCreated))
            {
                value.dateCreated = DateTime.ParseExact(dateCreated, "dd-MM-yyyy", null);
            }
            else
            {
                value.dateCreated = DateTime.Now;
            }
            
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
        public async Task<object> ApprovePayment(Guid id, [FromHeader] string pmt)
        {
            if (id == default(Guid))
            {
                logger.LogWarning("Payment id is invalid", id);
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Please refresh your page and try again"
                });
            }
            else if (string.IsNullOrEmpty(pmt))
            {
                logger.LogWarning("Payment id is invalid", id);
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "New status is required"
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
            string query = string.Empty;
            if (pmt == DemandNoticeStatus.CANCELED.ToString())
            {
                query = $"update tbl_demandNoticePaymentHistory set paymentStatus= '{pmt}' where id = '{id}' ";
            }
            else
            {
                if ((dnph.amount + dnph.charges) < 1)
                {
                    logger.LogWarning("Payment amount must be more than zero", dnph);
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.WRONG_CREDENTIALS,
                        description = "Please refresh your page an try again"
                    });
                }
                List<DNAmountDueModel> paymentDueList = await amountDueMgtService.ByBillingNo(dnph.billingNumber);
                if ((dnph.amount + dnph.charges) >= paymentDueList.Sum(x => (x.itemAmount - x.amountPaid )))
                {
                    amountDueMgtService.CurrentAmountDue(paymentDueList, dnph.amount, true);
                    query = amountDueMgtService.PaymentQuery(paymentDueList, dnph,
                       DemandNoticeStatus.PAID.ToString(),User.Identity.Name);
                }
                else
                {
                    List<DNAmountDueModel> paymentDueList2 = paymentDueList.Where(p => p.itemAmount > p.amountPaid).ToList();
                    amountDueMgtService.CurrentAmountDue(paymentDueList2, dnph.amount, false);
                    query = amountDueMgtService.PaymentQuery(paymentDueList, dnph,
                        DemandNoticeStatus.PART_PAYMENT.ToString(), User.Identity.Name);
                }
            }

            if (!string.IsNullOrEmpty(query))
            {
                bool result = await abstractService.ExecuteQueryAsync(query);
                if (result)
                {
                    return Ok(new Response()
                    {
                        code = MsgCode_Enum.SUCCESS,
                        description = $"Payment has been {pmt.ToLower()}"
                    });
                }
                else
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.FAIL,
                        description = "An error occur while processing payment. Please try again or contact administrator"
                    });
                }
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while processing payment. Please try again or contact administrator"
                });
            }
        }
    }
}
