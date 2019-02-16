using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/state")]
    public class StateController : Controller
    {
        private IStateManagers stateService;
        public StateController(IStateManagers _stateService)
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
