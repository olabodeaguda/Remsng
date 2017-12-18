using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Security;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/taxpayercategory")]
    public class TaxpayerCategoryController : Controller
    {
        private ITaxpayerCategoryService taxpayerCategoryService;
        public TaxpayerCategoryController(ITaxpayerCategoryService _taxpayerCategoryService)
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
        public async Task<object> Post([FromBody]TaxpayerCategory taxpayerCategory)
        {
            if (string.IsNullOrEmpty(taxpayerCategory.taxpayerCategoryName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category name is required!!"
                });
            }
            else if (taxpayerCategory.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required!!"
                });
            }

            taxpayerCategory.createdBy = User.Identity.Name;
            taxpayerCategory.dateCreated = DateTime.Now;
            taxpayerCategory.id = Guid.NewGuid();

            var r = await taxpayerCategoryService.GetByNameAndLcdaId(taxpayerCategory.lcdaId, taxpayerCategory.taxpayerCategoryName);
            if (r != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $" Category {taxpayerCategory.taxpayerCategoryName} already exist"
                }, 409);
            }

            Response response = await taxpayerCategoryService.Add(taxpayerCategory);
            return Ok(response);
        }

        [HttpPut]
        public async Task<object> Put([FromBody]TaxpayerCategory taxpayerCategory)
        {
            var r = await taxpayerCategoryService.GetById(taxpayerCategory.id);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{taxpayerCategory.taxpayerCategoryName} not found"
                });
            }

            if (string.IsNullOrEmpty(taxpayerCategory.taxpayerCategoryName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Category name is required!!"
                });
            }
            else if (taxpayerCategory.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required!!"
                });
            }
            else if (taxpayerCategory.id == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            taxpayerCategory.lastModifiedDate = DateTime.Now;
            taxpayerCategory.lastmodifiedby = User.Identity.Name;

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
