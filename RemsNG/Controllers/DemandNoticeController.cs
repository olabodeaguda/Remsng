using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace RemsNG.Controllers
{
    //[Authorize]
    [Route("api/v1/demandnotice")]
    public class DemandNoticeController : Controller
    {
        private readonly IArrearsManager _arrearsManager;
        private readonly IDemandNoticeManager demandService;
        private readonly IStreetManager streetService;
        private readonly IWardManager wardService;
        private readonly IDemandNoticeTaxpayerManager dnTaxpayerService;
        public DemandNoticeController(IDemandNoticeManager _demandService, IStreetManager _streetService,
            IWardManager _wardService, IDemandNoticeTaxpayerManager _dnTaxpayerService,
            IArrearsManager arrearsManager)
        {
            demandService = _demandService;
            streetService = _streetService;
            wardService = _wardService;
            dnTaxpayerService = _dnTaxpayerService;
            _arrearsManager = arrearsManager;
        }

        [HttpGet("bylcda")]
        public async Task<object> ByLcda([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            bool ismosdmin = ClaimExtension.IsMosAdmin(User.Claims.ToArray());
            if (lcdaId == default(Guid) && !ismosdmin)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (!ismosdmin)
            {
                return await demandService.ByLcdaId(lcdaId, new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
            else
            {
                return await demandService.All(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
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

            var result = await demandService.GetById(id);
            result.Query = Utilities.EncryptDecryptUtils.FromHexString(result.Query);
            Response response = new Response();
            response.data = result;
            if (result != null)
            {
                response.code = MsgCode_Enum.SUCCESS;
            }
            else
            {
                response.code = MsgCode_Enum.NOTFOUND;
                response.description = $"Record not found";
            }

            return response;
        }

        [HttpGet("batchno/{id}")]
        public async Task<object> GetByBatchNumber(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            var result = await demandService.GetByBatchId(id);
            result.Query = Utilities.EncryptDecryptUtils.FromHexString(result.Query);
            Response response = new Response();
            response.data = result;
            if (result != null)
            {
                response.code = MsgCode_Enum.SUCCESS;
            }
            else
            {
                response.code = MsgCode_Enum.NOTFOUND;
                response.description = $"{id} not found";
            }

            return response;
        }

        [RemsRequirementAttribute("DEMANDNOTICE_REQUEST")]
        [HttpPost]
        public async Task<object> Post([FromBody]DemandNoticeRequestModel demandNoticeRequest)
        {
            if (demandNoticeRequest.streetId != null && demandNoticeRequest.streetId != default(Guid))
            {
                StreetModel street = await streetService.ById(demandNoticeRequest.streetId);
                if (street == null)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.NOTFOUND,
                        description = "Street not found"
                    });
                }

            }
            else if (demandNoticeRequest.wardId != null && demandNoticeRequest.wardId != default(Guid))
            {
                Guid s = demandNoticeRequest.wardId;
                WardModel ward = await wardService.GetWard(demandNoticeRequest.wardId);
                if (ward == null)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.NOTFOUND,
                        description = "Ward not found"
                    });
                }
            }
            else if (demandNoticeRequest.dateYear == 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Billing year is required"
                });
            }


            DemandNoticeModel demandNotice = new DemandNoticeModel();
            demandNotice.LcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            string encr = JsonConvert.SerializeObject(demandNoticeRequest);
            string dx = Utilities.EncryptDecryptUtils.ToHexString(encr); ;
            string ddx = Utilities.EncryptDecryptUtils.FromHexString(dx);
            demandNotice.Query = Utilities.EncryptDecryptUtils.ToHexString(encr);

            demandNoticeRequest.createdBy = User.Identity.Name;
            demandNotice.BillingYear = demandNoticeRequest.dateYear;
            demandNotice.CreatedBy = User.Identity.Name;
            demandNotice.Id = Guid.NewGuid();
            demandNotice.BatchNo = CommonList.GetBatchNo();
            demandNotice.DemandNoticeStatus = DemandNoticeStatus.SUBMITTED.ToString();
            demandNotice.WardId = demandNoticeRequest.wardId;
            demandNotice.StreetId = demandNoticeRequest.streetId;
            demandNotice.IsUnbilled = demandNoticeRequest.isUnbilled;

            Response response = await demandService.Add(demandNotice);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Created("/demandnotice", response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [RemsRequirementAttribute("DEMANDNOTICE_REQUEST")]
        [HttpPost("run/bytaxpayer")]
        public async Task<IActionResult> PostDemandNoticeRequest([FromBody] DemandNoticeRequestModel demandNoticeRequest)
        {
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            bool ismosdmin = ClaimExtension.IsMosAdmin(User.Claims.ToArray());
            if (lcdaId == default(Guid) && !ismosdmin)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            demandNoticeRequest.createdBy = User.Identity.Name;
            demandNoticeRequest.lcdaId = lcdaId;
            bool result = await demandService.AddDemanNotice(demandNoticeRequest);
            if (!result)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "request failed"
                });
            }

            return Created("/demandNotice", new Response
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Demand notice has been added for onward processing"
            });
        }


        [HttpPost("search/{pageNum}/{pageSize}")]
        public async Task<IActionResult> SearchDemandNotice([FromBody] SearchDNModel model, int pageNum = 1, int pageSize = 20)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            return Ok(await demandService.SearchDemandNotice(modl, new PageModel()
            {
                PageNum = pageNum,
                PageSize = pageSize
            }));
        }

        [HttpPost("searchinfo")]
        public async Task<IActionResult> GetSearchInfo([FromBody] SearchDNModel model)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            var result = await demandService.SearchInfo(modl);

            return Ok(result);
        }

        [HttpPut("updatequery/{id}")]
        public async Task<object> UpdateQuery(Guid id, [FromBody]DemandNoticeRequestModel query)
        {
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            query.lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());

            Response response = await demandService.UpdateQuery(new DemandNoticeModel()
            {
                Id = id,
                Query = JsonConvert.SerializeObject(query),
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updateyear/{id}")]
        public async Task<object> UpdateBillingYear(Guid id, [FromHeader]int billingYr)
        {
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNoticeModel()
            {
                Id = id,
                BillingYear = billingYr,
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updatestatus/{id}")]
        public async Task<object> UpdateStatus(Guid id, [FromHeader]string dns)
        {
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNoticeModel()
            {
                Id = id,
                DemandNoticeStatus = dns,
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("validtaxpayers")]
        public async Task<IActionResult> ValidTaxpayer([FromBody] SearchDNModel model)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            TaxPayerModel[] taxpayers = await demandService.ValidTaxpayers(modl);
            return Ok(new Response
            {
                code = MsgCode_Enum.SUCCESS,
                data = taxpayers
            });
        }

        [HttpPost("arrears/add")]
        public async Task<IActionResult> AddArrears([FromBody] Guid[] dnId)
        {
            bool result = await _arrearsManager.RunTaxpayerArrears(dnId);

            return Ok(new Response
            {
                code = result ? MsgCode_Enum.SUCCESS : MsgCode_Enum.FAIL,
                status = result,
                description = result ? "Arrears has been run successfully" : "Please try again or contact your administrator"
            });
        }

        [HttpPost("arrears/remove")]
        public async Task<IActionResult> removeArrears([FromBody] Guid[] dnId)
        {
            bool result = await _arrearsManager.RemoveTaxpayerArrears(dnId);

            return Ok(new Response
            {
                code = result ? MsgCode_Enum.SUCCESS : MsgCode_Enum.FAIL,
                status = result,
                description = result ? "Arrears has been remove successfully" : "Please try again or contact your administrator"
            });
        }

        [HttpPost("runpenalty")]
        public async Task<IActionResult> AddPenalty([FromBody] Guid[] dnId)
        {
            return Ok();
        }
    }
}
