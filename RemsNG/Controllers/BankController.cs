using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/banks")]
    public class BankController : Controller
    {
        private readonly IBankService bankService;
        public BankController(IBankService _bankService)
        {
            bankService = _bankService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<List<Bank>> Get()
        {
            return await bankService.GetBankAsync();
        }
    }
}
