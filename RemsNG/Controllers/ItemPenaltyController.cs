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
    [Route("api/v1/itempenalty")]
    public class ItemPenaltyController : Controller
    {
        private IItemPenaltyManagers itemPenaltyService;
        private IItemManagers itemservice;
        public ItemPenaltyController(IItemPenaltyManagers _itemPenaltyService, IItemManagers _itemservice)
        {
            itemPenaltyService = _itemPenaltyService;
            itemservice = _itemservice;
        }

        [Route("byitem/{itemId}")]
        [HttpGet]
        public async Task<object> GetPenaltiesByItemId(Guid itemId)
        {
            if (itemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            var itm = itemservice.GetItemByIdAsync(itemId);
            if (itm == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Item not found"
                });
            }

            return await itemPenaltyService.ListByItemId(itemId);
        }

        [Route("byitempaginated/{itemId}")]
        [HttpGet]
        public async Task<object> ByLcdaIdPaginated([FromRoute] Guid itemId, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (itemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
            return await itemPenaltyService.ListByItemId(itemId, new PageModel()
            { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }

        // GET api/values/5
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

            return await itemPenaltyService.GetById(id);
        }

        [RemsRequirementAttribute("REGISTER_ITEM_PENALTY")]
        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]ItemPenaltyModel itemPenalty)
        {
            if (itemPenalty.ItemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.Duration))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Penalty duration required"
                });
            }
            else if (itemPenalty.Amount < 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Amount can't be less than zero"
                });
            }

            itemPenalty.CreatedBy = User.Identity.Name;
            itemPenalty.DateCreated = DateTime.Now;
            itemPenalty.PenaltyStatus = UserStatus.ACTIVE.ToString();
            Response response = await itemPenaltyService.Add(itemPenalty);

            return Ok(response);
        }

        [HttpPut]
        public async Task<object> Put([FromBody]ItemPenaltyModel itemPenalty)
        {
            if (itemPenalty.ItemId == default(Guid) || itemPenalty.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.Duration))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Penalty duration required"
                });
            }
            else if (itemPenalty.Amount < 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Amount can't be less than zero"
                });
            }

            Response response = await itemPenaltyService.Update(itemPenalty);

            return Ok(response);
        }

        [Route("changestatus")]
        [HttpPost]
        public async Task<object> ChangeStatus([FromBody]ItemPenaltyModel itemPenalty)
        {
            if (itemPenalty.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.PenaltyStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Penalty Status is required"
                });
            }

            Response response = await itemPenaltyService.UpdateStatus(itemPenalty);
            return Ok(response);
        }

        [HttpGet("addpenalty/{taxpayerId}")]
        public async Task<object> AddPenalty(Guid taxpayerId)
        {
            if (taxpayerId == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad Request"
                });
            }

            Guid[] tids = { taxpayerId };
            Response response = await itemPenaltyService.RunTaxpayerPenalty(tids);
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
