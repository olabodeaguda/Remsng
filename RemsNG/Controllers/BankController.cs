using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/banks")]
    public class BankController : Controller
    {
        private readonly IBankManager bankService;
        public BankController(IBankManager _bankService)
        {
            bankService = _bankService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<List<BankModel>> Get()
        {
            return await bankService.GetBankAsync();
        }
    }
}
