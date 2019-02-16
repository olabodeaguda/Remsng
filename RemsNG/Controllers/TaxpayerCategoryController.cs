using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Security;
using System;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/taxpayercategory")]
    public class TaxpayerCategoryController : Controller
    {
        private ITaxpayerCategoryManagers taxpayerCategoryService;
        public TaxpayerCategoryController(ITaxpayerCategoryManagers _taxpayerCategoryService)
        {
            taxpayerCategoryService = _taxpayerCategoryService;
        }
        // GET: api/values
        [HttpGet("bylcda/{lcdaId}")]
        public async Task<object> ByLcdaId(Guid lcdaId)
        {
            if (lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerCategoryService.GetListByLcdaIdAsync(lcdaId);
        }
        [Route("paginated/{lcdaid}")]
        [HttpGet]
        public async Task<object> ByLcdaId(Guid lcdaId, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            return await taxpayerCategoryService.GetListByLcdaIdAsync(lcdaId, new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            return await taxpayerCategoryService.GetById(id);
        }

        [RemsRequirementAttribute("REGISTER_TAXPAYER")]
        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]TaxpayerCategoryModel taxpayerCategory)
        {
            if (string.IsNullOrEmpty(taxpayerCategory.TaxpayerCategoryName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category name is required!!"
                });
            }
            else if (taxpayerCategory.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required!!"
                });
            }

            taxpayerCategory.CreatedBy = User.Identity.Name;
            taxpayerCategory.DateCreated = DateTime.Now;
            taxpayerCategory.Id = Guid.NewGuid();

            var r = await taxpayerCategoryService.GetByNameAndLcdaId(taxpayerCategory.LcdaId, taxpayerCategory.TaxpayerCategoryName);
            if (r != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $" Category {taxpayerCategory.TaxpayerCategoryName} already exist"
                }, 409);
            }

            Response response = await taxpayerCategoryService.Add(taxpayerCategory);
            return Ok(response);
        }

        [HttpPut]
        public async Task<object> Put([FromBody]TaxpayerCategoryModel taxpayerCategory)
        {
            var r = await taxpayerCategoryService.GetById(taxpayerCategory.Id);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{taxpayerCategory.TaxpayerCategoryName} not found"
                });
            }

            if (string.IsNullOrEmpty(taxpayerCategory.TaxpayerCategoryName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category name is required!!"
                });
            }
            else if (taxpayerCategory.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required!!"
                });
            }
            else if (taxpayerCategory.Id == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            taxpayerCategory.LastModifiedDate = DateTime.Now;
            taxpayerCategory.Lastmodifiedby = User.Identity.Name;

            Response response = await taxpayerCategoryService.Update(taxpayerCategory);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<object> Delete(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required!!"
                });
            }

            Response response = await taxpayerCategoryService.Delete(id);

            return Ok(response);
        }
    }
}
