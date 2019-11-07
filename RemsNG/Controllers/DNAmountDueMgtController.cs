using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/amountdue")]
    public class DNAmountDueMgtController : Controller
    {
        private IDNAmountDueMgtManager amountDueMgtService;
        public DNAmountDueMgtController(IDNAmountDueMgtManager _amountDueMgtService)
        {
            amountDueMgtService = _amountDueMgtService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{billingno}")]
        public async Task<List<DNAmountDueModel>> Get(long billingno)
        {
            if (billingno == default(long))
            {
                throw new InvalidCredentialsException("Billing number is required");
            }

            return await amountDueMgtService.ByBillingNo(billingno);
        }



        [RemsRequirementAttribute("AMOUNT_DUE")]
        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]DNAmountDueModel value)
        {
            if (value.id == Guid.NewGuid())
            {
                throw new InvalidCredentialsException("Invalid request, identity number is required");
            }

            if (value.itemAmount < 0)
            {
                throw new InvalidCredentialsException("Item amount can't be less than zero"); ;
            }

            Response response = await amountDueMgtService.UpdateAmount(value);

            return Ok(response);

        }



    }
}
