using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.Services;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/contact")]
    public class ContactController : Controller
    {
        private IContactService contactService;
        private IUserService userService;
        public ContactController(IUserService _userService, IContactService _contactService)
        {
            contactService = _contactService;
            userService = _userService;
        }
        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            if (default(Guid) == id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Wrong user detail"
                });
            }

            User user = await userService.Get(id);
            if (user == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "User not found"
                });
            }

            return await contactService.ByOwnerId(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]ContactDetail contactDetail)
        {
            if (string.IsNullOrEmpty(contactDetail.contactType))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Contact Type is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(contactDetail.contactValue))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Contact Value is required!!!"
                });
            }

            contactDetail.createdBy = User.Identity.Name;
            contactDetail.dateCreated = DateTime.Now;
            contactDetail.id = Guid.NewGuid();

            bool result = await contactService.Add(contactDetail);

            if (result)
            {
                return Created("/", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = ""
                });
            }
            else
            {
                return new  HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to create contact"
                }, 409);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]ContactDetail contactDetail)
        {
            if (default(Guid) == id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "bad request"
                });
            }

            ContactDetail cd = await contactService.ById(id);
            if (cd == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Not found"
                });
            }

            bool result = await contactService.Update(contactDetail);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to update contact. Try again or contact administrator"
                }, 409);
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<object> Delete(Guid id)
        {
            if (default(Guid) == id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "bad request"
                });
            }

            ContactDetail cd = await contactService.ById(id);
            if (cd == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Not found"
                });
            }

            bool result = await contactService.Remove(cd);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Record have beed removed"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to remove. Try again, or contact administrator for help"
                }, 409);
            }
        }
    }
}
