using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Security;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/taxpayer")]
    public class TaxpayerController : Controller
    {
        private ITaxpayerService taxpayerService;
        private ILcdaService lcdaService;
        private ILogger logger;
        private IAddress addressservice;
        private ICompany companyService;
        private IStreetService streetservice;
        public TaxpayerController(ITaxpayerService _taxpayerService, ILcdaService _lcdaService,
            ILoggerFactory _logger, IAddress address, ICompany _companyservice, IStreetService _streetservice)
        {
            taxpayerService = _taxpayerService;
            lcdaService = _lcdaService;
            addressservice = address;
            companyService = _companyservice;
            streetservice = _streetservice;
            logger = _logger.CreateLogger("Tax payer");
        }
        [Route("bylcda/{id}")]
        [HttpGet]
        public async Task<object> ByLcda(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByLcdaId(id);
        }

        [Route("search/{id}/{query}")]
        [HttpGet]
        public async Task<object> ByLcda(Guid id, string query)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.Search(id, query);
        }

        [Route("bylcdapaginated/{id}")]
        [HttpGet]
        public async Task<object> ByLcdaPaginated(Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByLcdaId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }

        [Route("bycompany/{id}")]
        [HttpGet]
        public async Task<object> ByCompany(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByCompanyId(id);
        }

        [Route("bystreet/{id}")]
        [HttpGet]
        public async Task<object> ByStreet(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByStreetId(id);
        }

        [Route("bystreetpaginated/{id}")]
        [HttpGet]
        public async Task<object> ByStreetPaginated(Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByStreetId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }


        [Route("bylcdapaged")]
        [HttpGet]
        public async Task<object> ByLcdaPaginated([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            Guid id = ClaimExtension.GetDomainId(User.Claims.ToArray());
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByLcdaId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }


        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]Taxpayer value, [FromHeader] string confirmcompany)
        {
            if (value.streetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street is required"
                });
            }
            else if (value.companyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required"
                });
            }
            else if (string.IsNullOrEmpty(value.streetNumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "House number is required"
                });
            }

            Company company = await companyService.ById(value.companyId);
            if (company == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Company not found"
                });
            }

            Street street = await streetservice.ById(value.streetId);
            if (street == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Street not found"
                });
            }

            //List<TaxpayerExtension> coys = await taxpayerService.ByStreetId(value.streetId);

            //if ((coys.Count + 1) > street.numberOfHouse)
            //{
            //    return BadRequest(new Response()
            //    {
            //        code = MsgCode_Enum.NOTFOUND,
            //        description = $"Registered company have gotten to expected number ({coys.Count})"
            //    });
            //}

            Lgda lgda = await lcdaService.ByStreet(value.streetId);

            if (lgda == null)
            {
                logger.LogError($"Application could not get lcda for the street {value.streetId}");
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to retrieve taxpayers lcda"
                });
            }
            value.id = Guid.NewGuid();

            Address address = new Address()
            {
                id = Guid.NewGuid(),
                addressnumber = value.streetNumber,
                lcdaid = lgda.id,
                streetId = value.streetId,
                createdBy = User.Identity.Name,
                ownerId = value.id
            };

            value.createdBy = User.Identity.Name;
            value.taxpayerStatus = "ACTIVE";
            value.addressId = address.id;

            Response response = await taxpayerService.Create(value, (confirmcompany.ToLower() == "true" ? true : false));

            if (response.code != MsgCode_Enum.SUCCESS)
            {
                return BadRequest(response);
            }

            Response addressResp = await addressservice.Add(address);

            if (addressResp.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }

        [HttpPut]
        public async Task<object> Put([FromBody]TaxpayerExtension taxpayerExtension)
        {
            if (taxpayerExtension.id == default(Guid) || taxpayerExtension.addressId == default(Guid)
                || taxpayerExtension.companyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(taxpayerExtension.streetNumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street number is required"
                });
            }

            Address address = await addressservice.ById(taxpayerExtension.addressId);
            if (address == null)
            {
                logger.LogError($"Not Found" + JsonConvert.SerializeObject(address));
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Address can be found"
                });
            }

            TaxpayerExtension te = await taxpayerService.ById(taxpayerExtension.id);
            if (te == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Not found"
                });
            }
            int sucessCount = 0;
            if (address.addressnumber != taxpayerExtension.streetNumber || taxpayerExtension.companyId != te.companyId
                || taxpayerExtension.firstname != te.firstname ||
                    taxpayerExtension.lastname != te.lastname || taxpayerExtension.surname != te.surname)
            {
                if (address.addressnumber != taxpayerExtension.streetNumber)
                {
                    address.addressnumber = taxpayerExtension.streetNumber;
                    address.lastModifiedDate = DateTime.Now;
                    address.lastmodifiedby = User.Identity.Name;
                    Response addressResp = await addressservice.Update(address);
                    if (addressResp.code != MsgCode_Enum.SUCCESS)
                    {
                        logger.LogError(addressResp.description + JsonConvert.SerializeObject(address));
                        return BadRequest(new Response()
                        {
                            code = MsgCode_Enum.FAIL,
                            description = "Update failed. Please try again or contact administrator"
                        });
                    }
                    else
                    {
                        sucessCount++;
                    }
                }

                if (taxpayerExtension.companyId != te.companyId || taxpayerExtension.firstname != te.firstname ||
                    taxpayerExtension.lastname != te.lastname || taxpayerExtension.surname != te.surname)
                {
                    Response tsResponse = await taxpayerService.Update(new Taxpayer()
                    {
                        addressId = te.addressId,
                        companyId = taxpayerExtension.companyId,
                        id = taxpayerExtension.id,
                        lastmodifiedby = User.Identity.Name,
                        lastModifiedDate = DateTime.Now,
                        streetId = taxpayerExtension.streetId,
                        firstname = taxpayerExtension.firstname,
                        lastname = taxpayerExtension.lastname,
                        surname = taxpayerExtension.surname
                    });

                    if (tsResponse.code != MsgCode_Enum.SUCCESS)
                    {
                        logger.LogError("Taxpayer update failed  :" + JsonConvert.SerializeObject(taxpayerExtension));
                        return BadRequest(new Response()
                        {
                            code = MsgCode_Enum.FAIL,
                            description = "Update failed. Please try again or contact administrator"
                        });
                    }
                    else
                    {
                        sucessCount++;
                    }
                }

            }

            if (sucessCount > 0)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update is successfull"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "No changes"
                });
            }
        }

        [HttpGet("paymenthistory/{id}")]
        public async Task<IActionResult> PaymentHistory(Guid id)
        {
            return Ok(await taxpayerService.PaymentHistory(id));
        }

    }
}
