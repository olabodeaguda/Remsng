using Newtonsoft.Json;
using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using RemsNG.Utilities;
using Microsoft.Extensions.Logging;

namespace RemsNG.Services
{
    public class RunDemandNoticeService : IRunDemandNoticeService
    {
        private readonly TaxpayerDao taxpayerDao;
        private readonly DemandNoticeDao demandNoticeDao;
        private readonly DemandNoticeTaxpayersDao demandNoticeTaxpayersDao;
        private readonly ErrorDao errorDao;
        private readonly ILogger logger;
        private readonly DemandNoticeItemDao demandNoticeItemDao;
        private readonly DemandNoticeArrearDao demandNoticeArrearDao;
        private readonly DemandNoticePenaltyDao demandNoticePenaltyDao;
        private readonly LcdaDao lcdaDao;
        private readonly WardDao wardDao;
        private readonly StreetDao streetDao;
        private readonly IAddress address;
        private readonly ILcdaService lcdaService;
        private readonly IStateService stateService;
        private readonly IImageService imageService;
        private readonly IDnDownloadService dnDownloadService;
        private readonly IBatchDwnRequestService batchDwnRequestService;
        public RunDemandNoticeService(RemsDbContext _db,
            ILoggerFactory loggerFactory, IAddress _address,
            ILcdaService _lcdaService, IStateService _stateService,
            IImageService _imageService,
            IDnDownloadService _dnDownloadService, IBatchDwnRequestService _batchDwnRequestService)
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            taxpayerDao = new TaxpayerDao(_db);
            demandNoticeDao = new DemandNoticeDao(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersDao(_db);
            errorDao = new ErrorDao(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemDao(_db);
            demandNoticeArrearDao = new DemandNoticeArrearDao(_db);
            demandNoticePenaltyDao = new DemandNoticePenaltyDao(_db);
            lcdaDao = new LcdaDao(_db, loggerFactory);
            wardDao = new WardDao(_db);
            streetDao = new StreetDao(_db, loggerFactory);
            address = _address;
            lcdaService = _lcdaService;
            imageService = _imageService;
            stateService = _stateService;
            dnDownloadService = _dnDownloadService;
            batchDwnRequestService = _batchDwnRequestService;
        }

        public async Task RegisterTaxpayer()
        {
            DemandNotice demandNotice = await demandNoticeDao.DequeueDemandNotice();

            try
            {
                if (demandNotice != null)
                {
                    String query = EncryptDecryptUtils.FromHexString(demandNotice.query);
                    DemandNoticeRequest demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequest>(query);
                    List<Taxpayer> taxpayers = await taxpayerDao.Get(demandNoticeRequest);
                    string domainName = string.Empty;

                    if (taxpayers.Count > 0)
                    {
                        Lgda lcda = await lcdaService.Get(demandNoticeRequest);
                        if (lcda != null)
                        {
                            Domain dd = await lcdaDao.GetDomain(lcda.id);
                            if (dd != null)
                            {
                                domainName = dd.domainName;
                            }

                        }

                        List<DemandNoticeTaxpayersDetail> dt =
                            await demandNoticeTaxpayersDao.getTaxpayerByIds(taxpayers.Select(x => string.Format("'{0}'", x.id)).ToArray(), demandNotice.billingYear);
                        //make sure demand notice have not been raised for the taxpayers by year and taxpayersId

                        foreach (var tm in taxpayers)
                        {
                            var itExist = dt.FirstOrDefault(x => x.taxpayerId == tm.id);
                            if (itExist != null)
                            {
                                Error error = new Error()
                                {
                                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                    errorvalue = $"Demand notice have already been raised for {tm.surname} {tm.firstname} {tm.lastname} " +
                                      $"for billing year {demandNotice.billingYear}",
                                    ownerId = demandNotice.id
                                };
                                bool result = await errorDao.Add(error);
                                continue;
                            }
                            else
                            {
                                //add taxpayers
                                DemandNoticeTaxpayersDetail dntd = new DemandNoticeTaxpayersDetail();
                                dntd.billingYr = demandNotice.billingYear;
                                dntd.dnId = demandNotice.id;
                                dntd.createdBy = demandNoticeRequest.createdBy;
                                dntd.taxpayerId = tm.id;
                                dntd.domainName = domainName;
                                dntd.lcdaAddress = await address.AddressByOwnerId(lcda.id);
                                dntd.lcdaState = await stateService.StateNameByLcda(lcda.id);
                                dntd.lcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lcda.id, 
                                    ImgTypesEnum.LOGO.ToString());
                                dntd.revCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.id,
                                    ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                                dntd.councilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.id,
                                    ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());

                                Response response1 = await demandNoticeTaxpayersDao.Add(dntd);

                                if (response1.code == MsgCode_Enum.SUCCESS)
                                {
                                    dntd.billingNumber = response1.data.ToString().Trim();
                                    //run arrears 
                                    await RunArrears(dntd);
                                    await RunDemandNoticeItem(dntd); //run items 
                                }
                                else
                                {
                                    // log error
                                    Error error = new Error()
                                    {
                                        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        errorvalue = response1.description,
                                        ownerId = dntd.dnId
                                    };
                                    bool result = await errorDao.Add(error);
                                }
                            }
                        }
                    }
                    else
                    {
                        Error error = new Error()
                        {
                            errorType = ErrorType.DEMAND_NOTICE.ToString(),
                            errorvalue = $"{taxpayers.Count} taxpayer was found from the query request ",
                            ownerId = demandNotice.id
                        };
                        bool result = await errorDao.Add(error);
                    }


                    //update demand notice
                    demandNotice.demandNoticeStatus = DemandNoticeStatus.COMPLETED.ToString();
                    Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation($"{demandNotice.id} has been completed");
                    }
                }
            }
            catch (Exception x)
            {
                // cancel no retry
                demandNotice.demandNoticeStatus = DemandNoticeStatus.ERROR.ToString();
                Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                if (response.code == MsgCode_Enum.SUCCESS)
                {
                    logger.LogInformation($"{demandNotice.id} has been completed");
                }
                logger.LogError(x.Message);
            }
        }

        public async Task GenerateBulkDemandNotice()
        {
            BatchDemandNoticeModel bdnm = await batchDwnRequestService.Dequeue();

            if (bdnm != null)
            {

            }
        }
        
        public async Task TaxpayerPenalty()
        {
            // scan through demandnoticeitem() check the year/penalty duration compare, if passed then create penalty
            DateTime startDuration = new DateTime(DateTime.Now.Year, 1, 1);
            List<DemandNoticeItem> demandNoticeItem = await demandNoticePenaltyDao.OverDueDemandNotice();
            List<string> lstOfdurations = CommonList.CurrentDurations(startDuration);
            string query = string.Empty;
            string ids = string.Empty;
            foreach (var tm in demandNoticeItem)
            {
                if (!lstOfdurations.Contains(tm.duration))
                {
                    continue;
                }

                ids = ids + $"{tm.id}|";
                // add to demand notice penalty
                DemandNoticeItemPenalty dnp = new DemandNoticeItemPenalty()
                {
                    billingNo = tm.billingNo,
                    amountPaid = 0,
                    billingYr = tm.billingYr,
                    itemId = tm.itemId,
                    itemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                    originatedYear = tm.billingYr,
                    taxpayerId = tm.taxpayerId,
                    totalAmount = tm.penaltyAmount.Value
                };
                query = query + demandNoticePenaltyDao.AddQuery(dnp);
            }

            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    Response response = demandNoticePenaltyDao.RunQuery(query);
                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation($"Ids: {ids}, description: {response.description}");
                    }
                    else
                    {
                        logger.LogError($"Ids: {ids}, description: {response.description}");
                    }
                }
                catch (Exception x)
                {
                    logger.LogError(x.Message);
                }
            }

        }

        private async Task RunArrears(DemandNoticeTaxpayersDetail dntd)
        {
            // check for unpaid arrears  paid or partly paid
            // change current arrear status to move
            // create a new arrears with the status unpaid
            //get previous year demand notice

            DN_ArrearsModel dN_ArrearsModel = new DN_ArrearsModel()
            {
                arrearstatus = DemandNoticeStatus.PENDING.ToString(),
                billingNo = dntd.billingNumber,
                billingYr = dntd.billingYr,
                previousBillingYr = dntd.billingYr - 1,
                createdBy = dntd.createdBy,
                taxpayerId = dntd.taxpayerId
            };

            Response responseUnpaidArrears = await demandNoticeArrearDao.AddUnpaidArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidArrears.code != MsgCode_Enum.SUCCESS)
            {
                // logger error
                logger.LogError(responseUnpaidArrears.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid arrears section : {responseUnpaidArrears.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dntd.dnId
                };
                bool result = await errorDao.Add(error);
            }

            Response responseUnpaidDemandNotice = await demandNoticeArrearDao.AddUnpaidDemandNoticeToArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidDemandNotice.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(responseUnpaidDemandNotice.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid demand notice arrears section: {responseUnpaidDemandNotice.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dntd.dnId
                };
                bool result = await errorDao.Add(error);
            }

            await MovedUnpaidPenalty(dN_ArrearsModel);
        }

        private async Task MovedUnpaidPenalty(DN_ArrearsModel dN_ArrearsModel)
        {
            Response responseUnpaidPenalty = await demandNoticePenaltyDao.AddUnpaidPenaltyAsync(dN_ArrearsModel);
            if (responseUnpaidPenalty.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(responseUnpaidPenalty.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid demand notice Penalty movement section: {responseUnpaidPenalty.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dN_ArrearsModel.dnId
                };
                bool result = await errorDao.Add(error);
            }
        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayersDetail dntd)
        {
            Response response = await demandNoticeItemDao.Add(dntd);
            if (response.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(response.description, dntd);
            }
        }
    }
}
