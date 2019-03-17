using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
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
        private ITaxpayerManager taxpayerService;
        private ILcdaManager lcdaService;
        private ILogger logger;
        private IAddressManager addressservice;
        private ICompanyManager companyService;
        private IStreetManager streetservice;
        public TaxpayerController(ITaxpayerManager _taxpayerService, ILcdaManager _lcdaService,
            ILoggerFactory _logger, IAddressManager address,
            ICompanyManager _companyservice,
            IStreetManager _streetservice)
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
        public async Task<object> Post([FromBody]TaxPayerModel value, [FromHeader] string confirmcompany)
        {
            if (value.StreetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street is required"
                });
            }
            else if (value.CompanyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required"
                });
            }
            else if (string.IsNullOrEmpty(value.StreetNumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "House number is required"
                });
            }

            CompanyModel company = await companyService.ById(value.CompanyId);
            if (company == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Company not found"
                });
            }

            StreetModel street = await streetservice.ById(value.StreetId.Value);
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

            LcdaModel lgda = await lcdaService.ByStreet(value.StreetId.Value);

            if (lgda == null)
            {
                logger.LogError($"Application could not get lcda for the street {value.StreetId.Value}");
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur while trying to retrieve taxpayers lcda"
                });
            }
            value.Id = Guid.NewGuid();

            AddressModel address = new AddressModel()
            {
                Id = Guid.NewGuid(),
                Addressnumber = value.StreetNumber,
                Lcdaid = lgda.Id,
                StreetId = value.StreetId.Value,
                CreatedBy = User.Identity.Name,
                OwnerId = value.Id
            };

            value.CreatedBy = User.Identity.Name;
            value.TaxpayerStatus = "ACTIVE";
            value.AddressId = address.Id;

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
        public async Task<object> Put([FromBody]TaxPayerModel taxpayerExtension)
        {
            if (taxpayerExtension.Id == default(Guid) || taxpayerExtension.AddressId == default(Guid)
                || taxpayerExtension.CompanyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            else if (string.IsNullOrEmpty(taxpayerExtension.StreetNumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street number is required"
                });
            }

            AddressModel address = await addressservice.ById(taxpayerExtension.AddressId.Value);
            if (address == null)
            {
                LcdaModel lgda = await lcdaService.ByStreet(taxpayerExtension.StreetId.Value);

                address = new AddressModel()
                {
                    Id = taxpayerExtension.AddressId.Value,
                    Addressnumber = taxpayerExtension.StreetNumber,
                    Lcdaid = lgda.Id,
                    StreetId = taxpayerExtension.StreetId.Value,
                    CreatedBy = User.Identity.Name,
                    OwnerId = taxpayerExtension.Id
                };
                Response addressResp = await addressservice.Add(address);
            }

            TaxPayerModel te = await taxpayerService.ById(taxpayerExtension.Id);
            if (te == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Not found"
                });
            }
            int sucessCount = 0;
            if (address.Addressnumber != taxpayerExtension.StreetNumber || taxpayerExtension.CompanyId != te.CompanyId
                || taxpayerExtension.Firstname != te.Firstname ||
                    taxpayerExtension.Lastname != te.Lastname || taxpayerExtension.Surname != te.Surname || taxpayerExtension.IsOneTime != te.IsOneTime)
            {
                if (address.Addressnumber != taxpayerExtension.StreetNumber)
                {
                    address.Addressnumber = taxpayerExtension.StreetNumber;
                    address.LastModifiedDate = DateTime.Now;
                    address.Lastmodifiedby = User.Identity.Name;
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

                taxpayerExtension.Lastmodifiedby = User.Identity.Name;
                taxpayerExtension.LastModifiedDate = DateTime.Now;
                taxpayerExtension.AddressId = te.AddressId;
                Response tsResponse = await taxpayerService.Update(taxpayerExtension);

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

        [HttpDelete("{taxpayerId}")]
        public async Task<IActionResult> Delete(Guid taxpayerId)
        {
            bool result = await taxpayerService.Delete(taxpayerId);
            if (!result)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Taxpayer has been deleted successfully"
            });
        }

        [HttpGet("paymenthistory/{id}")]
        public async Task<IActionResult> PaymentHistory(Guid id)
        {
            return Ok(await taxpayerService.PaymentHistory(id));
        }
        [Route("searchinstreet/{streetId}")]
        [HttpGet]
        public async Task<IActionResult> SearchInStreet(Guid streetId, string query)
        {
            if (streetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            if (string.IsNullOrEmpty(query))
            {
                return Ok(await taxpayerService.ByStreetId(streetId));
            }

            return Ok(await taxpayerService.SearchInStreet(streetId, query));
        }

    }
}
