using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/state")]
    public class StateController : Controller
    {
        private IStateService stateService;
        public StateController(IStateService _stateService)
        {
            stateService = _stateService;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<object> Get()
        {
            return await stateService.All();
        }
        
    }
}
