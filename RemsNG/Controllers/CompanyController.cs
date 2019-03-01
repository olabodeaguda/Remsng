using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Security;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/company")]
    public class CompanyController : Controller
    {
        private ICompanyManager companyService;
        private ILcdaManager lcdaService;
        private IDemandNoticeTaxpayerManager demandNoticeTaxpayerService;
        public CompanyController(ICompanyManager _companyservice, ILcdaManager _lcdaService,
            IDemandNoticeTaxpayerManager _demandNoticeTaxpayerService)
        {
            companyService = _companyservice;
            lcdaService = _lcdaService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
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

            return await companyService.ByStreetId(id);
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

            return await companyService.ByLcda(id, new Common.Models.PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
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

        [RemsRequirementAttribute("REGISTER_COMPANY")]
        [HttpPost]
        public async Task<object> Post([FromBody]CompanyModel value)
        {
            if (string.IsNullOrEmpty(value.CompanyName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company name is required"
                });
            }
            else if (value.SectorId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (value.CategoryId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category is required"
                });
            }
            else if (value.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }
            var lcda = lcdaService.Get(value.LcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }


            value.Id = Guid.NewGuid();
            value.CompanyStatus = UserStatus.ACTIVE.ToString();
            value.DateCreated = DateTime.Now;
            value.CreatedBy = User.Identity.Name;

            Response response = await companyService.Add(value);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Created("/company/" + value.Id, response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [RemsRequirementAttribute("REGISTER_COMPANY")]
        [HttpPut]
        public async Task<object> Put([FromBody]CompanyModel value)
        {
            if (value.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(value.CompanyName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company name is required"
                });
            }
            else if (value.SectorId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (value.CategoryId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category is required"
                });
            }
            else if (value.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }
            var lcda = lcdaService.Get(value.LcdaId);
            if (lcda == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Reload page and try again else log out and log in. Application could not tract your LCDA"
                });
            }


            value.CompanyStatus = UserStatus.ACTIVE.ToString();
            value.LastModifiedDate = DateTime.Now;
            value.Lastmodifiedby = User.Identity.Name;

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

        [RemsRequirementAttribute("REGISTER_COMPANY")]
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
            else if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Status is required"
                });
            }

            Response response = await companyService.UpdateStatus(new CompanyModel()
            {
                Id = id,
                CompanyStatus = status
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [Route("close/{id}")]
        [HttpGet]
        public async Task<object> CloseDemandNotice(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            CompanyModel company = await companyService.ById(id);
            if (company == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Company can not be found"
                });
            }

            bool result = await demandNoticeTaxpayerService.BlinkClosesDemandNoticeByCompany(id);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "All raised demand notice has been closed"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occured. Please try again later"
                });
            }
        }
    }
}
