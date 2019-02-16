using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Route("api/v1/domain")]
    public class DomainController : Controller
    {
        private readonly ILogger logger;
        private readonly IDomainManagers domainService;
        private readonly IUserManagers userService;
        private readonly ILcdaManagers lcdaService;
        public DomainController(IUserManagers _userService,
            IDomainManagers _domainService, ILoggerFactory loggerFactory,
            ILcdaManagers _lcdaService)
        {
            userService = _userService;
            domainService = _domainService;
            lcdaService = _lcdaService;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [Route("domainByusername/{username}")]
        public async Task<IActionResult> ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = "username is required!!!."
                });
            }
            UserModel us = await userService.GetUserByUsername(username);
            if (us == null)
            {
                return NotFound(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{username} does't exist"
                });
            }

            List<DomainModel> domains = await domainService.GetDomainByUsername(username);

            return Ok(new Response
            {
                code = MsgCode_Enum.SUCCESS,
                data = domains.Where(x => x.DomainStatus == UserStatus.ACTIVE.ToString())
            });
        }

        [Route("activeDomain")]
        public async Task<object> Get()
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                return await domainService.ActiveDomains();
            }
            else
            {
                Guid userid = ClaimExtension.UserId(User.Claims.ToArray());
                if (userid == Guid.Empty)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.INTERNAL_ERROR,
                        description = "Please logout and login again else contact your administrator"
                    });
                }
                return await domainService.GetUserDomainByUsernameId(userid);
            }
        }

        [Route("all")]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await domainService.Paginated(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
            }
            else
            {
                var domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId != default(Guid))
                {
                    DomainModel d = await domainService.DomainbyLCDAId(domainId);
                    return new
                    {
                        data = new List<object>() { d },
                        totalPageCount = 1
                    };
                }
            }
            return new
            {
                data = new List<object>() { },
                totalPageCount = 1
            };
        }
        [RemsRequirementAttribute("MOS-ADMIN")]
        [Route("create")]
        public async Task<IActionResult> Post([FromBody] DomainModel domain)
        {
            if (string.IsNullOrEmpty(domain.DomainCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(domain.DomainName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA name is required!!!"
                });
            }

            DomainModel ifExist = await domainService.ByDomainCode(domain.DomainCode);
            if (ifExist != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{domain.DomainCode} already exist"
                }, 409);
            }

            domain.Id = Guid.NewGuid();
            domain.DomainStatus = UserStatus.ACTIVE.ToString();
            domain.Datecreated = DateTime.Now;
            bool result = await domainService.Add(domain);
            if (result)
            {
                return Created("api/v1/domain", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = domain,
                    description = $"{domain.DomainName} have been added successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    data = domain,
                    description = $"{domain.DomainName} creation was not successfull. Please try again or contact administrator"
                }, 409);
            }
        }

        [RemsRequirementAttribute("UPDATE_DOMAIN")]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] DomainModel domain)
        {
            if (string.IsNullOrEmpty(domain.DomainCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(domain.DomainName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA name is required!!!"
                });
            }
            else if (domain.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid domain selected"
                });
            }

            bool result = await domainService.UpdateDomain(domain);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Update failed"
                });
            }
        }

        //approval
        [RemsRequirementAttribute("MOS-ADMIN")]
        [Route("changestatus")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] DomainModel domain)
        {
            if (string.IsNullOrEmpty(domain.DomainStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Status is required!!!"
                });
            }
            else if (domain.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid domain selected"
                });
            }
            else if (CommonList.StatusLst.FirstOrDefault(x => x == domain.DomainStatus) == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid domain status"
                });
            }

            bool result = await domainService.ChangeDomain(domain.Id, domain.DomainStatus);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Update failed"
                });
            }
        }

        [RemsRequirementAttribute("MOS-ADMIN")]
        [Route("currentdomain")]
        [HttpGet]
        public async Task<IActionResult> CurrentDomain()
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                var r = await lcdaService.All();
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = new
                    {
                        isMosAdmin = true,
                        domains = r
                    }
                });
            }
            else
            {
                Guid domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId == Guid.Empty)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.NO_DOMAIN_ACCESS,
                        description = "Aplication can not validate user domain"
                    });
                }

                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = new
                    {
                        isMosAdmin = false
                    }
                });
            }
        }
    }
}
