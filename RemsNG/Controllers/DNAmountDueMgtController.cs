using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.Exceptions;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/amountdue")]
    public class DNAmountDueMgtController : Controller
    {
        private IDNAmountDueMgtService amountDueMgtService;
        public DNAmountDueMgtController(IDNAmountDueMgtService _amountDueMgtService)
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
        public async Task<List<DNAmountDueModel>> Get(string billingno)
        {
            if (string.IsNullOrEmpty(billingno))
            {
                throw new InvalidCredentialsException("Billing number is required");
            }

            return await amountDueMgtService.ByBillingNo(billingno);
        }


        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]DNAmountDueModel value)
        {
            if (value.id == Guid.NewGuid())
            {
                throw new InvalidCredentialsException("Invalid request identity number is required"); 
            }

            if (value.itemAmount <= 0)
            {
                throw new InvalidCredentialsException("Item amount can't be less than zero"); ;
            }

            Response response = await amountDueMgtService.UpdateAmount(value);

            return Ok(response);
          
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}
