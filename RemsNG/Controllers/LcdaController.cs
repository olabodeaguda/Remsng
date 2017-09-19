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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
                return await lcdaService.AllPaginated(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
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
        [RemsRequirementAttribute("GET_AddLCDA")]
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


            lcda.id = Guid.NewGuid();
            lcda.dateCreated = DateTime.Now;
            lcda.createdBy = User.Identity.Name;
            lcda.lcdaStatus = UserStatus.ACTIVE.ToString();
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
