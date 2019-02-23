using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/payment")]
    public class DemanNoticePaymentController : Controller
    {
        private readonly IItemPenaltyManagers _penaltyService;
        private readonly IDNPaymentHistoryManagers dNPaymentHistoryService;
        private readonly IDemandNoticeTaxpayerManagers demandNoticeTaxpayerService;
        private ILogger logger;
        private IDNAmountDueMgtManagers amountDueMgtService;
        private IAbstractManagers abstractService;
        public DemanNoticePaymentController(IDNPaymentHistoryManagers _dNPaymentHistoryService,
            IDemandNoticeTaxpayerManagers _demandNoticeTaxpayerService,
            ITaxpayerManagers _taxpayerService, ILoggerFactory loggerFactory,
            IDNAmountDueMgtManagers _amountDueMgtService, IAbstractManagers _abstractService,
            IItemPenaltyManagers penaltyService)
        {
            dNPaymentHistoryService = _dNPaymentHistoryService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
            logger = loggerFactory.CreateLogger("Receipt payment");
            amountDueMgtService = _amountDueMgtService;
            abstractService = _abstractService;
            _penaltyService = penaltyService;
        }

        [HttpGet("{billingNumber}")]
        public async Task<List<DemandNoticePaymentHistoryModel>> Get(string billingNumber)
        {
            return await dNPaymentHistoryService.ByBillingNumber(billingNumber);
        }

        [RemsRequirementAttribute("REGISTER_PAYMENT")]
        [HttpPost]
        public async Task<Response> Post([FromBody]DemandNoticePaymentHistoryModel value, [FromHeader] string dateCreated)
        {
            if (value.Amount < 1)
            {
                throw new InvalidCredentialsException("Amount is required");
            }
            else if (value.BankId == default(Guid))
            {
                throw new InvalidCredentialsException("Bank is required");
            }
            else if (string.IsNullOrEmpty(value.BillingNumber))
            {
                throw new InvalidCredentialsException("Bank is required");
            }
            else if (string.IsNullOrEmpty(value.ReferenceNumber))
            {
                throw new InvalidCredentialsException("Reference number is required");
            }
            if (!string.IsNullOrEmpty(dateCreated))
            {
                value.DateCreated = DateTime.ParseExact(dateCreated, "dd-MM-yyyy", null);
            }
            else
            {
                value.DateCreated = DateTime.Now;
            }

            var taxpayeD = await demandNoticeTaxpayerService.TaxpayerMiniByBillingNo(value.BillingNumber);
            if (taxpayeD == null)
            {
                throw new NotFoundException("Billing number not found");
            }

            // Lgda lgda = await taxpayerService.getLcda(t.taxpayerId);
            value.OwnerId = taxpayeD.TaxpayerId;
            value.CreatedBy = User.Identity.Name;
            value.PaymentMode = PaymentModeEnum.BANKS.ToString();
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

            return await dNPaymentHistoryService.ByLcdaId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
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

            DemandNoticePaymentHistoryModel dnph = await dNPaymentHistoryService.ById(id);
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
                if (dnph.IsWaiver)
                {
                    // get arrears
                    //var arrears = await demandNoticeTaxpayerService.GetArrearsByTaxpayerId(dnph.ownerId);
                    //if (arrears.Sum(x=>x.totalAmount) <= dnph.amount)
                    //{
                    //    string query1 = ""
                    //}

                    ////get penalty
                    //var penalty = _penaltyService.ActivePenalty(dnph.ownerId);


                }

                var txpayer = await demandNoticeTaxpayerService.TaxpayerMiniByBillingNo(dnph.BillingNumber);
                var prepay = await dNPaymentHistoryService.GetPrepaymentByTaxpayerId(txpayer.TaxpayerId);
                if (prepay != null)
                {
                    dnph.Amount = dnph.Amount + prepay.amount;
                    query = query + $"update tbl_prepayment set prepaymentStatus = 'CLOSED' where id= {prepay.id};";
                }
                if ((dnph.Amount + dnph.Charges) < 1)
                {
                    logger.LogWarning("Payment amount must be more than zero", dnph);
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.WRONG_CREDENTIALS,
                        description = "Please refresh your page an try again"
                    });
                }
                List<DNAmountDueModel> paymentDueList = await amountDueMgtService.ByBillingNo(dnph.BillingNumber);

                if ((dnph.Amount + dnph.Charges) == paymentDueList.Sum(x => (x.itemAmount - x.amountPaid)))
                {
                    amountDueMgtService.CurrentAmountDue(paymentDueList, dnph.Amount, true);

                    query = query + amountDueMgtService.PaymentQuery(paymentDueList, dnph,
                       DemandNoticeStatus.PAID.ToString(), User.Identity.Name);
                }
                else if ((dnph.Amount + dnph.Charges) > paymentDueList.Sum(x => (x.itemAmount - x.amountPaid)))
                {
                    decimal rmain = (dnph.Amount + dnph.Charges) - paymentDueList.Sum(x => (x.itemAmount - x.amountPaid));

                    amountDueMgtService.CurrentAmountDue(paymentDueList, (dnph.Amount + dnph.Charges) - rmain, true);
                    query = query + amountDueMgtService.PaymentQuery(paymentDueList, dnph,
                       DemandNoticeStatus.PAID.ToString(), User.Identity.Name);
                    //over payment by taxpayer 
                    query = query + $"insert into tbl_prepayment(taxpayerId,amount,datecreated) values('{txpayer.TaxpayerId}','{rmain}',getdate());";
                }
                else
                {
                    List<DNAmountDueModel> paymentDueList2 = paymentDueList.Where(p => p.itemAmount > p.amountPaid).ToList();
                    amountDueMgtService.CurrentAmountDue(paymentDueList2, dnph.Amount, false);

                    query = query + amountDueMgtService.PaymentQuery(paymentDueList, dnph,
                        DemandNoticeStatus.PART_PAYMENT.ToString(), User.Identity.Name);
                }
            }

            query = query + $"update tbl_demandNoticeTaxpayers set isUnbilled= 0 where billingNumber = '{dnph.BillingNumber}' ";


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
