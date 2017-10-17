using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/item")]
    public class ItemController : Controller
    {
        IItemService itemService;
        public ItemController(IItemService _itemService)
        {
            itemService = _itemService;
        }

        [Route("bylcda/{lcdaId}")]
        [HttpGet]
        public async Task<object> ByLcdaId([FromRoute] Guid lcdaId)
        {
            if (lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }
            return Ok(await itemService.ListByLcdaId(lcdaId));
        }

        [Route("bylcdapaginated/{lcdaId}")]
        [HttpGet]
        public async Task<object> ByLcdaIdPaginated([FromRoute] Guid lcdaId, [FromHeader] string pageSize, [FromHeader] string pageNum)
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
            return await itemService.ListByLcdaId(lcdaId, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }
        
        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }

            var result = await itemService.GetItemByIdAsync(id);
            if (result == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Select item not found"
                });
            }
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<object> Post([FromBody]Item item)
        {
            if (string.IsNullOrEmpty(item.itemDescription))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "item Description is required"
                });
            }
            else if (item.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required"
                });
            }

            item.createdBy = User.Identity.Name;
            item.id = Guid.NewGuid();

            Response response = await itemService.Add(item);

            return Ok(response);
        }
        
        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]Item item)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Select item is corrupt. Please reload page and try again else contact administrator"
                });
            }
            else if (item.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "LCDA is required"
                });
            }
            else if (string.IsNullOrEmpty(item.itemDescription))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "item Description is required"
                });
            }

            item.lastmodifiedby = User.Identity.Name;
            item.lastModifiedDate = DateTime.Now;

            Response response = await itemService.Update(item);

            return (response);
        }
        
        [Route("changestatus")]
        [HttpPost]
        public async Task<object> ChangeStatus([FromBody] Item item)
        {
            if (item.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Select item is corrupt. Please reload page and try again else contact administrator"
                });
            }
            else if (string.IsNullOrEmpty(item.itemDescription))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "item Description is required"
                });
            }

            item.lastmodifiedby = User.Identity.Name;
            item.lastModifiedDate = DateTime.Now;

            Response response = await itemService.UpdateStatus(item);

            return Ok(response);
        }
    }
}
