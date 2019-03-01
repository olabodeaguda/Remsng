using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Route("api/v1/ward")]
    public class WardController : Controller
    {
        private readonly ILcdaManager lcdaService;
        private readonly IWardManager wardService;
        public WardController(IWardManager _wardservice, ILcdaManager _lcdaService)
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
                return wards.OrderBy(x => x.WardName);
            }
            else
            {
                var domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());// User.Claims.FirstOrDefault(x => x.Type == "Domain");
                if (domainId != Guid.Empty)
                {
                    var wards = await wardService.GetWardByLGDAId(domainId);
                    return wards.OrderBy(x => x.WardName);
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
        public async Task<IActionResult> Post([FromBody]WardModel ward)
        {
            if (ward.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Lgda is required!!"
                });
            }
            else if (string.IsNullOrEmpty(ward.WardName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward name is required!!"
                });
            }
            LcdaModel lcda = await lcdaService.Get(ward.LcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Wrong ward selected is required!!"
                });
            }

            WardModel w = await wardService.GetWard(ward.WardName, ward.LcdaId);
            if (w != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{ward.WardName} already exist"
                }, 409);
            }
            ward.Id = Guid.NewGuid();
            ward.CreatedBy = User.Identity.Name;
            ward.DateCreated = DateTime.Now;
            ward.WardStatus = UserStatus.ACTIVE.ToString();

            bool result = await wardService.Add(ward);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{ward.WardName} has been added successfully"
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
        public async Task<IActionResult> Update([FromBody] WardModel ward)
        {
            if (string.IsNullOrEmpty(ward.WardName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Ward name is required"
                });
            }
            else if (default(Guid) == ward.Id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "LCDA is required"
                });
            }

            ward.Lastmodifiedby = User.Identity.Name;
            ward.LastModifiedDate = DateTime.Now;
            bool result = await wardService.Update(ward);
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
