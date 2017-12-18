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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/payment")]
    public class DemanNoticePaymentController : Controller
    {
        private readonly IDNPaymentHistoryService dNPaymentHistoryService;
        private readonly IDemandNoticeTaxpayerService demandNoticeTaxpayerService;
        public DemanNoticePaymentController(IDNPaymentHistoryService _dNPaymentHistoryService,
            IDemandNoticeTaxpayerService _demandNoticeTaxpayerService, ITaxpayerService _taxpayerService)
        {
            dNPaymentHistoryService = _dNPaymentHistoryService;
            dNPaymentHistoryService = _dNPaymentHistoryService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
        }
        // GET api/values/5
        [HttpGet("{billingNumber}")]
        public async Task<List<DemandNoticePaymentHistory>> Get(string billingNumber)
        {
            return await dNPaymentHistoryService.ByBillingNumber(billingNumber);
        }

        // POST api/values
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
    }
}
