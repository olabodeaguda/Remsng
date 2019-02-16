using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Route("api/v1/lcda")]
    public class LcdaController : Controller
    {
        private ILcdaManagers lcdaService;
        private IUserManagers userservice;
        private IRoleManagers roleService;
        public LcdaController(ILcdaManagers _lcdaService,
            IUserManagers _userservice, IRoleManagers _roleService)
        {
            lcdaService = _lcdaService;
            userservice = _userservice;
            roleService = _roleService;
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

            List<LcdaModel> us = await lcdaService.byUsername(username);
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
                    return new List<LcdaModel>();
                }

                return await lcdaService.UserDomainByUserId(currentDomain);
            }
        }

        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public async Task<object> Get([FromRoute] Guid id)
        {
            LcdaModel lgda = await lcdaService.Get(id);
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
                    LcdaModel lgda = await lcdaService.Get(domainId);
                    List<LcdaModel> cd = new List<LcdaModel>();
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
        public async Task<IActionResult> Post([FromBody] LcdaModel lcda)
        {
            if (string.IsNullOrEmpty(lcda.LcdaCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(lcda.LcdaName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA name is required!!!"
                });
            }
            else if (lcda.DomainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Code is required!!!"
                });
            }
            var oldlcda = await lcdaService.ByLcdaCode(lcda.LcdaCode);
            if (oldlcda != null && oldlcda.DomainId == lcda.DomainId)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{lcda.LcdaCode} already exist for the selected domain"
                }, 409);
            }

            lcda.Id = Guid.NewGuid();
            lcda.DateCreated = DateTime.Now;
            lcda.CreatedBy = User.Identity.Name;
            lcda.LcdaStatus = UserStatus.NOT_ACTIVE.ToString();
            //User.Identity.
            bool result = await lcdaService.Add(lcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{lcda.LcdaName} have been added successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{lcda.LcdaName} registration failed"
                });
            }
        }

        [Route("update")]
        [RemsRequirementAttribute("UPDATE_LCDA")]
        [HttpPost]
        public async Task<IActionResult> EditLGA([FromBody] LcdaModel lcda)
        {
            if (string.IsNullOrEmpty(lcda.LcdaCode))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA Code is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(lcda.LcdaName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCGA name is required!!!"
                });
            }
            else if (lcda.DomainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain Code is required!!!"
                });
            }
            else if (lcda.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = " is required!!!"
                });
            }

            var oldlcda = await lcdaService.Get(lcda.Id);
            if (oldlcda == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Data does not exist"
                });
            }

            bool result = await lcdaService.Update(lcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = lcda.LcdaName + " has been updated successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = lcda.LcdaName + " has been updated successfully"
                });
            }
        }

        [Route("changestatus")]
        [RemsRequirementAttribute("CHANGE_LCDA_STATUS")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] LcdaModel lcda)
        {
            if (string.IsNullOrEmpty(lcda.LcdaStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Status is required!!!"
                });
            }
            else if (lcda.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid lgda selected"
                });
            }
            else if (CommonList.StatusLst.FirstOrDefault(x => x == lcda.LcdaStatus) == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Invalid status"
                });
            }

            bool result = await lcdaService.Changetatus(lcda.Id, lcda.LcdaStatus);
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
                    description = $"An error occur while processing {lcda.LcdaName}. Please try again or contact an administrator"
                }, 409);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("userdomain/{id}")]
        public async Task<object> UserDomains([FromRoute] Guid id)
        {
            UserModel user = await userservice.Get(id);
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
            UserModel user = await userservice.Get(id);
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
        public async Task<object> RemoveUserFromDomain([FromBody] UserLcdaModel userLcda)
        {
            if (userLcda.UserId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User is required"
                });
            }
            else if (userLcda.LgdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCDA is required"
                });
            }

            RoleExtensionModel roleExtension = await roleService.UserDomainRolesByDomainId(userLcda.UserId, userLcda.LgdaId);
            if (roleExtension != null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "User can't be remove from the selected role. User exist in a role"
                });
            }

            // remove

            bool result = await lcdaService.RemoveUserFromLcda(userLcda);
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
