using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/domain")]
    public class DomainController : Controller
    {
        private readonly ILogger logger;
        private readonly IDomainService domainService;
        public DomainController(IDomainService _domainService, ILoggerFactory loggerFactory)
        {
            domainService = _domainService;
            logger = loggerFactory.CreateLogger<UserController>();
        }
        [Route("domainByusername/{username}")]
        public async Task<IActionResult> ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    data = $"{username} does't exist"
                });
            }

            List<Domain> domains = await domainService.GetDomainByUsername(username);

            return Ok(new Response
            {
                code = MsgCode_Enum.FAIL,
                data = domains
            });
        }
    }
}
