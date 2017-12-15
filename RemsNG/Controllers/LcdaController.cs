using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Utilities;
using RemsNG.Models;
using System.Security.Claims;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;
using RemsNG.Exceptions;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Security;

namespace RemsNG.Controllers
{
    [Route("api/v1/lcda")]
    public class LcdaController : Controller
    {
        private ILcdaService lcdaService;
        private IUserService userservice;
        private IRoleService roleService;
        public LcdaController(ILcdaService _lcdaService,
            IUserService _userservice, IRoleService _roleService)
        {
            this.lcdaService = _lcdaService;
            this.userservice = _userservice;
            this.roleService = _roleService;
        }

        [Route("byusername/{username}")]
        public async Task<IActionResult> LCDAByusername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = "username is required!!!."
                });
            }
            if (username.ToLower() == "mos-admin")
            {
                return Ok(new Response
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = new object[0]
                });
            }

            List<Lgda> us = await this.lcdaService.byUsername(username);
            if (us.Count < 1)
            {
                return NotFound(new Response
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{username} does't exist"
                });
            }

            return Ok(new Response
            {
                code = MsgCode_Enum.SUCCESS,
                data = us
            });
        }

        [Authorize]
        [Route("total")]
        [HttpGet]
        public async Task<object> All()
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                return await lcdaService.All();
            }
            else
            {
                Guid currentDomain = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (currentDomain == Guid.Empty)
                {
                    return new List<Lgda>();
                }

                return await lcdaService.UserDomainByUserId(currentDomain);
            }
        }

        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public async Task<object> Get([FromRoute] Guid id)
        {
            Lgda lgda = await lcdaService.Get(id);
            if (lgda == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Selected LCDA could not be found"
                });
            }

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = lgda
            });
        }

        [Route("all")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await lcdaService.All(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
            }
            else
            {
                var domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId != Guid.Empty)
                {
                    Lgda lgda = await lcdaService.Get(domainId);
                    List<Lgda> cd = new List<Lgda>();
                    cd.Add(lgda);
                    return new
                    {
                        data = cd,
                        totalPageCount = 1
                    };
                }
            }
            return new object[] { };
        }

        [Route("create")]
        [RemsRequirementAttribute("ADD_LCDA")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Lgda lcda)
        {
            if (string.IsNullOrEmpty(lcda.lcdaCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(lcda.lcdaName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA name is required!!!"
                });
            }
            else if (lcda.domainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Code is required!!!"
                });
            }
            var oldlcda = await this.lcdaService.byLCDACode(lcda.lcdaCode);
            if (oldlcda != null && oldlcda.domainId == lcda.domainId)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{lcda.lcdaCode} already exist for the selected domain"
                }, 409);
            }

            lcda.id = Guid.NewGuid();
            lcda.dateCreated = DateTime.Now;
            lcda.createdBy = User.Identity.Name;
            lcda.lcdaStatus = UserStatus.NOT_ACTIVE.ToString();
            //User.Identity.
            bool result = await lcdaService.Add(lcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{lcda.lcdaName} have been added successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{lcda.lcdaName} registration failed"
                });
            }
        }

        [Route("update")]
        [RemsRequirementAttribute("UPDATE_LCDA")]
        [HttpPost]
        public async Task<IActionResult> EditLGA([FromBody] Lgda lcda)
        {
            if (string.IsNullOrEmpty(lcda.lcdaCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(lcda.lcdaName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA name is required!!!"
                });
            }
            else if (lcda.domainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Code is required!!!"
                });
            }
            else if (lcda.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = " is required!!!"
                });
            }

            var oldlcda = await this.lcdaService.Get(lcda.id);
            if (oldlcda == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Data does not exist"
                });
            }

            bool result = await this.lcdaService.Update(lcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = lcda.lcdaName + " has been updated successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = lcda.lcdaName + " has been updated successfully"
                });
            }
        }

        [Route("changestatus")]
        [RemsRequirementAttribute("CHANGE_LCDA_STATUS")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] Lgda lcda)
        {
            if (string.IsNullOrEmpty(lcda.lcdaStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Status is required!!!"
                });
            }
            else if (lcda.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid lgda selected"
                });
            }
            else if (CommonList.StatusLst.FirstOrDefault(x => x == lcda.lcdaStatus) == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid status"
                });
            }

            bool result = await lcdaService.Changetatus(lcda.id, lcda.lcdaStatus);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"Lgda have been updated successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while processing {lcda.lcdaName}. Please try again or contact an administrator"
                }, 409);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("userdomain/{id}")]
        public async Task<object> UserDomains([FromRoute] Guid id)
        {
            User user = await userservice.Get(id);
            if (user == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "User not found"
                });
            }

            return await lcdaService.UserDomainByUserId(id);
        }

        [Authorize]
        [HttpGet]
        [Route("userroledomain/{id}")]
        public async Task<object> UserRoleDomains([FromRoute] Guid id)
        {
            User user = await userservice.Get(id);
            if (user == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "User not found"
                });
            }

            return await lcdaService.UserRoleDomainbyUserId(id);
        }

        [Authorize]
        [HttpGet]
        [Route("unassignlcda/{id}")]
        public async Task<object> LcdaNotInUser([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Wrong request"
                });
            }

            var result = await lcdaService.UnAssignUserDomainByUserId(id);
            return Ok(new Response()
            {
                data = result,
                code = MsgCode_Enum.SUCCESS,
            });
        }

        [RemsRequirementAttribute("REMOVE_USER_FROM LCDA")]
        [HttpPost]
        [Route("removeuserfromdomain")]
        public async Task<object> RemoveUserFromDomain([FromBody] UserLcda userLcda)
        {
            if (userLcda.userId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User is required"
                });
            }
            else if (userLcda.lgdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCDA is required"
                });
            }

            RoleExtension roleExtension = await roleService.UserDomainRolesByDomainId(userLcda.userId, userLcda.lgdaId);
            if (roleExtension != null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "User can't be remove from the selected role. User exist in a role"
                });
            }

            // remove

            bool result = await lcdaService.RemoveUserFromLCDA(userLcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Request was successful"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request failed"
                }, 409);
            }

        }

    }
}
