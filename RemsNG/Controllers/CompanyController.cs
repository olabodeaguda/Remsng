using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RemsNG.Utilities;
using RemsNG.Models;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/company")]
    public class CompanyController : Controller
    {
        private ICompany companyService;
        private ILcdaService lcdaService;
        public CompanyController(ICompany _companyservice, ILcdaService _lcdaService)
        {
            companyService = _companyservice;
            lcdaService = _lcdaService;
        }

        [Route("bylcda/{id}")]
        [HttpGet]
        public async Task<object> Bylcda([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                     code = MsgCode_Enum.FAIL,
                      description = "Bad Request"
                });
            }

            return await companyService.ByLcda(id);
        }

        [Route("bystreet/{id}")]
        [HttpGet]
        public async Task<object> ByStreet([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }

            return await companyService.ByStretId(id);
        }

        [Route("bylcdapaging/{id}")]
        [HttpGet]
        public async Task<object> BylcdaPaginated([FromRoute] Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await companyService.ByLcda(id, new Models.PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            var r = await companyService.ById(id);

            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Company not found"
                });
            }
            return r;
        }

        [HttpPost]
        public async Task<object> Post([FromBody]Company value)
        {
            if (string.IsNullOrEmpty(value.companyName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company name is required"
                });
            }
            else if (value.sectorId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (value.categoryId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category is required"
                });
            }
            else if (value.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }
            var lcda = lcdaService.Get(value.lcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }


            value.id = Guid.NewGuid();
            value.companyStatus = UserStatus.ACTIVE.ToString();
            value.dateCreated = DateTime.Now;
            value.createdBy = User.Identity.Name;

            Response response = await companyService.Add(value);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Created("/company/" + value.id, response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [HttpPut]
        public async Task<object> Put([FromBody]Company value)
        {
            if (value.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(value.companyName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company name is required"
                });
            }
            else if (value.sectorId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (value.categoryId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category is required"
                });
            }
            else if (value.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }
            var lcda = lcdaService.Get(value.lcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }


            value.companyStatus = UserStatus.ACTIVE.ToString();
            value.lastModifiedDate = DateTime.Now;
            value.lastmodifiedby = User.Identity.Name;

            Response response = await companyService.Update(value);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [Route("updatestatus/{id}/{status}")]
        [HttpPost]
        public async Task<object> UpdateStatus([FromRoute] Guid id, [FromRoute] string status)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if(string.IsNullOrEmpty(status))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Status is required"
                });
            }

            Response response = await companyService.UpdateStatus(new Company()
            {
                id = id,
                companyStatus = status
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok( response);
            }
            else
            {
                return BadRequest(response);
            }

        }
    }
}
