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
    [Route("api/v1/contact")]
    public class ContactController : Controller
    {
        private IContactManagers contactService;
        private IUserManagers userService;
        public ContactController(IUserManagers _userService, IContactManagers _contactService)
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

            //User user = await userService.Get(id);
            //if (user == null)
            //{
            //    return NotFound(new Response()
            //    {
            //        code = MsgCode_Enum.NOTFOUND,
            //        description = "User not found"
            //    });
            //}

            return await contactService.ByOwnerId(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]ContactDetailModel contactDetail)
        {
            if (string.IsNullOrEmpty(contactDetail.ContactType))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Contact Type is required!!!"
                });
            }
            else if (string.IsNullOrEmpty(contactDetail.ContactValue))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Contact Value is required!!!"
                });
            }

            contactDetail.CreatedBy = User.Identity.Name;
            contactDetail.DateCreated = DateTime.Now;
            contactDetail.Id = Guid.NewGuid();

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
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to create contact"
                }, 409);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]ContactDetailModel contactDetail)
        {
            if (default(Guid) == id)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "bad request"
                });
            }

            ContactDetailModel cd = await contactService.ById(id);
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

            ContactDetailModel cd = await contactService.ById(id);
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
