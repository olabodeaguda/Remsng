using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Utilities;
using RemsNG.Services.Interfaces;
using System.Security.Claims;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Security;
using Microsoft.AspNetCore.Authorization;

namespace RemsNG.Controllers
{
    [Route("api/v1/ward")]
    public class WardController : Controller
    {
        private readonly ILcdaService lcdaService;
        private readonly IWardService wardService;
        public WardController(IWardService _wardservice, ILcdaService _lcdaService)
        {
            wardService = _wardservice;
            lcdaService = _lcdaService;
        }

        [Route("all")]
        // [RemsRequirementAttribute("GET_WARD")]
        [Authorize]
        [HttpGet]
        public async Task<object> All()
        {
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                var wards = await wardService.all();
                return wards.OrderBy(x => x.wardName);
            }
            else
            {
                var domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());// User.Claims.FirstOrDefault(x => x.Type == "Domain");
                if (domainId != Guid.Empty)
                {
                    var wards = await wardService.GetWardByLGDAId(domainId);
                    return wards.OrderBy(x => x.wardName);
                }
            }

            return new object[] { };
        }

        [Route("paginated")]
        [Authorize]
        // [RemsRequirementAttribute("GET_WARD")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum, [FromHeader] string lcdaId)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
            Guid domainId = Guid.Empty;
            bool v2 = Guid.TryParse(lcdaId, out domainId);
            if (domainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCDA is required!!!"
                });
            }

            return await wardService.Paginated(new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            }, domainId);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            return await wardService.GetWard(id);
        }

        [RemsRequirementAttribute("ADD_WARD")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Ward ward)
        {
            if (ward.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Lgda is required!!"
                });
            }
            else if (string.IsNullOrEmpty(ward.wardName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward name is required!!"
                });
            }
            Lgda lcda = await lcdaService.Get(ward.lcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Wrong ward selected is required!!"
                });
            }

            Ward w = await wardService.GetWard(ward.wardName, ward.lcdaId);
            if (w != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{ward.wardName} already exist"
                }, 409);
            }
            ward.id = Guid.NewGuid();
            ward.createdBy = User.Identity.Name;
            ward.dateCreated = DateTime.Now;
            ward.wardStatus = UserStatus.ACTIVE.ToString();

            bool result = await wardService.Add(ward);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{ward.wardName} has been added successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Addtion failed. Please try again or contact administrator"
                }, 409);
            }
        }

        [RemsRequirementAttribute("UPDATE_WARD")]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Ward ward)
        {
            if (string.IsNullOrEmpty(ward.wardName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Ward name is required"
                });
            }
            else if (default(Guid) == ward.id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCDA is required"
                });
            }

            ward.lastmodifiedBy = User.Identity.Name;
            ward.lastModifiedDate = DateTime.Now;
            bool result = await this.wardService.Update(ward);
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
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to update record. Please try again or contact administrator"
                }, 409);
            }
        }
    }
}
