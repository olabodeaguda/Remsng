using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/companyitem")]
    public class CompanyItemController : Controller
    {
        private ICompanyItemService companyItemService;
        public CompanyItemController(ICompanyItemService _companyItemService)
        {
            companyItemService = _companyItemService;
        }

        [HttpGet("bycompany/{id}")]
        public async Task<object> ByCompany(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await companyItemService.ByCompany(id);
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            return await companyItemService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody] CompanyItem companyItem)
        {
            if (companyItem.companyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required!!!"
                });
            }
            else if (companyItem.itemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required!!!"
                });
            }
            else if (companyItem.billingYear < 2015)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = companyItem.billingYear == 0 ? "Billing year is required!!!" : "Bill year is in the wrong format"
                });
            }

            companyItem.companyStatus = UserStatus.ACTIVE.ToString();
            companyItem.id = Guid.NewGuid();
            companyItem.createdBy = User.Identity.Name;

            Response response = await companyItemService.Add(companyItem);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<object> Put([FromBody]CompanyItem companyItem)
        {
            if (companyItem.companyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required!!!"
                });
            }
            else if (companyItem.itemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required!!!"
                });
            }
            else if (companyItem.billingYear < 2015)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = companyItem.billingYear == 0 ? "Billing year is required!!!" : "Bill year is in the wrong format"
                });
            }
            else if (companyItem.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            companyItem.lastmodifiedby = User.Identity.Name;
            companyItem.lastModifiedDate = DateTime.Now;

            Response response = await companyItemService.Update(companyItem);
            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [Route("changestatus")]
        [HttpPost]
        public async Task<object> ChangeStatus([FromBody]CompanyItem companyItem)
        {
            if (companyItem.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(companyItem.companyStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company status is required"
                });
            }

            Response response = await companyItemService.UpdateStatus(companyItem.id, companyItem.companyStatus);
            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
