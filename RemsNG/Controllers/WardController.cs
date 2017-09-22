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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        [RemsRequirementAttribute("GET_LCDA")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            var hasClaim = User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            if (hasClaim)
            {
                pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await wardService.Paginated(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
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
                        List<Ward> cd = await wardService.GetWardByLGDAId(dId);
                        return cd;
                    }
                }
            }
            return new object[] { };
        }

        // POST api/values
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
            Lcda lcda = await lcdaService.Get(ward.lcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Wrong ward selected is required!!"
                });
            }

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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
