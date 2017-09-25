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

namespace RemsNG.Controllers
{
    [Route("api/v1/lcda")]
    public class LcdaController : Controller
    {
        private ILcdaService lcdaService;
        public LcdaController(ILcdaService _lcdaService)
        {
            this.lcdaService = _lcdaService;
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

            List<Lcda> us = await this.lcdaService.byUsername(username);
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

        [Route("total")]
        [RemsRequirementAttribute("MOSADMIN")]
        [HttpGet]
        public async Task<object> All()
        {
            return await lcdaService.All();
        }
       
        [Route("all")]
        [RemsRequirementAttribute("GET_LCDA")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            var hasClaim = User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            if (hasClaim)
            {
                pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await lcdaService.All(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
            }
            else
            {
                var domainId = User.Claims.FirstOrDefault(x => x.Type == "Domain");
                if (domainId != null)
                {
                    Guid dId = Guid.Empty;
                    bool v = Guid.TryParse(domainId.Value, out dId);
                    if (v)
                    {
                        List<Lcda> cd = await lcdaService.ActiveLCDAByDomainId(dId);
                        return cd;
                    }
                }
            }
            return new object[] { };
        }

        [Route("create")]
        [RemsRequirementAttribute("ADD_LCDA")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Lcda lcda)
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
        [RemsRequirementAttribute("EDIT_LCDA")]
        [HttpPost]
        public async Task<IActionResult> EditLGA([FromBody] Lcda lcda)
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
        public async Task<IActionResult> ChangeStatu([FromBody] Lcda lcda)
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

    }
}
