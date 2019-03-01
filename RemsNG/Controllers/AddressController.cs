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
    [Route("api/v1/address")]
    public class AddressController : Controller
    {
        private IAddressManager addressservice;

        public AddressController(IAddressManager _addressservice)
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
        public async Task<object> Post([FromBody]AddressModel address)
        {
            if (string.IsNullOrEmpty(address.Addressnumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Address number is required"
                });
            }
            else if (address.Lcdaid == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.OwnerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.StreetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            address.CreatedBy = User.Identity.Name;
            address.DateCreated = DateTime.Now;
            address.Id = Guid.NewGuid();

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
        public async Task<object> Put([FromBody]AddressModel address)
        {
            if (string.IsNullOrEmpty(address.Addressnumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Address number is required"
                });
            }
            else if (address.Lcdaid == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.OwnerId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.StreetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (address.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            address.Lastmodifiedby = User.Identity.Name;
            address.LastModifiedDate = DateTime.Now;

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
            AddressModel address = await addressservice.ById(id);
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
