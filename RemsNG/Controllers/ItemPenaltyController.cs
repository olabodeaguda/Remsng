﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/itempenalty")]
    public class ItemPenaltyController : Controller
    {
        private IItemPenaltyService itemPenaltyService;
        private IItemService itemservice;
        public ItemPenaltyController(IItemPenaltyService _itemPenaltyService, IItemService _itemservice)
        {
            this.itemPenaltyService = _itemPenaltyService;
            this.itemservice = _itemservice;
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
            return await itemPenaltyService.ListByItemId(itemId, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
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

        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]ItemPenalty itemPenalty)
        {
            if (itemPenalty.itemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.duration))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Penalty duration required"
                });
            }
            else if (itemPenalty.amount < 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Amount can't be less than zero"
                });
            }

            Response response = await itemPenaltyService.Add(itemPenalty);

            return Ok(response);
        }


        [HttpPut]
        public async Task<object> Put([FromBody]ItemPenalty itemPenalty)
        {
            if (itemPenalty.itemId == default(Guid) || itemPenalty.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.duration))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Penalty duration required"
                });
            }
            else if (itemPenalty.amount < 0)
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
        public async Task<object> ChangeStatus([FromBody]ItemPenalty itemPenalty)
        {
            if (itemPenalty.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(itemPenalty.penaltyStatus))
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
    }
}