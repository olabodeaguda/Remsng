﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/domain")]
    public class DomainController : Controller
    {
        private readonly ILogger logger;
        private readonly IDomainService domainService;
        private readonly IUserService userService;
        private readonly ILcdaService lcdaService;
        public DomainController(IUserService _userService,
            IDomainService _domainService, ILoggerFactory loggerFactory,
            ILcdaService _lcdaService)
        {
            this.userService = _userService;
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
            User us = await this.userService.GetUserByUsername(username);
            if (us == null)
            {
                return NotFound(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{username} does't exist"
                });
            }

            List<Domain> domains = await domainService.GetDomainByUsername(username);

            return Ok(new Response
            {
                code = MsgCode_Enum.SUCCESS,
                data = domains.Where(x => x.domainStatus == UserStatus.ACTIVE.ToString())
            });
        }

        [Route("activeDomain")]        
        public async Task<object> Get()
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                return await this.domainService.ActiveDomains();
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
                pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await domainService.Paginated(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
            }
            else
            {
                var domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId != default(Guid))
                {
                    Domain d = await domainService.DomainbyLCDAId(domainId);
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
        public async Task<IActionResult> Post([FromBody] Domain domain)
        {
            if (string.IsNullOrEmpty(domain.domainCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(domain.domainName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA name is required!!!"
                });
            }

            Domain ifExist = await domainService.ByDomainCode(domain.domainCode);
            if (ifExist != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{domain.domainCode} already exist"
                }, 409);
            }

            domain.id = Guid.NewGuid();
            domain.domainStatus = UserStatus.ACTIVE.ToString();
            domain.datecreated = DateTime.Now;
            bool result = await domainService.Add(domain);
            if (result)
            {
                return Created("api/v1/domain", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = domain,
                    description = $"{domain.domainName} have been added successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    data = domain,
                    description = $"{domain.domainName} creation was not successfull. Please try again or contact administrator"
                }, 409);
            }
        }

        [RemsRequirementAttribute("UPDATE_DOMAIN")]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Domain domain)
        {
            if (string.IsNullOrEmpty(domain.domainCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(domain.domainName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LGA name is required!!!"
                });
            }
            else if (domain.id == default(Guid))
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
        public async Task<IActionResult> ChangeStatus([FromBody] Domain domain)
        {
            if (string.IsNullOrEmpty(domain.domainStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Status is required!!!"
                });
            }
            else if (domain.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid domain selected"
                });
            }
            else if (CommonList.StatusLst.FirstOrDefault(x => x == domain.domainStatus) == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid domain status"
                });
            }

            bool result = await domainService.ChangeDomain(domain.id, domain.domainStatus);
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
