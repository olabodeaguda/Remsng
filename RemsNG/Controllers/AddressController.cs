using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Utilities;

namespace RemsNG.Controllers
{
    [Route("api/v1/address")]
    public class AddressController : Controller
    {
        private IAddress addressservice;

        public AddressController(IAddress _addressservice)
        {
            addressservice = _addressservice;
        }

        [Route("byownerid/{id}/{lcdaId}")]
        [HttpGet]
        public async Task<object> Get([FromRoute] Guid id, [FromRoute] Guid lcdaId)
        {
            return await addressservice.ByOwnersId(id, lcdaId);
        }

        [Route("ownerid/{id}")]
        [HttpGet]
        public async Task<object> GetByownerId([FromRoute] Guid id)
        {
            return await addressservice.ByOwnersId(id);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]Address address)
        {
            if (string.IsNullOrEmpty(address.addressnumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Address number is required"
                });
            }
            else if (address.lcdaid == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.ownerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.streetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            address.createdBy = User.Identity.Name;
            address.dateCreated = DateTime.Now;
            address.id = Guid.NewGuid();

            Response resp = await addressservice.Add(address);
            if (resp.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(resp);
            }
            else
            {
                return BadRequest(resp);
            }

        }

        // PUT api/values/5
        [HttpPut]
        public async Task<object> Put([FromBody]Address address)
        {
            if (string.IsNullOrEmpty(address.addressnumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Address number is required"
                });
            }
            else if (address.lcdaid == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.ownerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.streetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            address.lastmodifiedby = User.Identity.Name;
            address.lastModifiedDate = DateTime.Now;

            Response response = await addressservice.Update(address);
            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<object> Delete(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            Address address = await addressservice.ById(id);
            if (address == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Address not found"
                });
            }

            Response response = await addressservice.Remove(address);
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
