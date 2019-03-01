using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/companyitem")]
    public class CompanyItemController : Controller
    {
        private ICompanyItemManager companyItemService;
        public CompanyItemController(ICompanyItemManager _companyItemService)
        {
            companyItemService = _companyItemService;
        }

        [HttpGet("bytaxpayer/{id}")]
        public async Task<object> ByCompany([FromRoute]Guid id, [FromHeader] string pageNum, [FromHeader] string pageSize)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            return await companyItemService.ByTaxpayerpaginated(id, new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            return await companyItemService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody] CompanyItemModel companyItem)
        {
            if (companyItem.TaxpayerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required!!!"
                });
            }
            else if (companyItem.ItemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required!!!"
                });
            }
            else if (companyItem.BillingYear < 2015)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = companyItem.BillingYear == 0 ? "Billing year is required!!!" : "Bill year is in the wrong format"
                });
            }

            companyItem.CompanyStatus = UserStatus.ACTIVE.ToString();
            companyItem.Id = Guid.NewGuid();
            companyItem.CreatedBy = User.Identity.Name;

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
        public async Task<object> Put([FromBody]CompanyItemModel companyItem)
        {
            if (companyItem.TaxpayerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required!!!"
                });
            }
            else if (companyItem.ItemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required!!!"
                });
            }
            else if (companyItem.BillingYear < 2015)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = companyItem.BillingYear == 0 ? "Billing year is required!!!" : "Bill year is in the wrong format"
                });
            }
            else if (companyItem.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            companyItem.Lastmodifiedby = User.Identity.Name;
            companyItem.LastModifiedDate = DateTime.Now;

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
        public async Task<object> ChangeStatus([FromBody]CompanyItemModel companyItem)
        {
            if (companyItem.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(companyItem.CompanyStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company status is required"
                });
            }

            Response response = await companyItemService.UpdateStatus(companyItem.Id, companyItem.CompanyStatus);
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
