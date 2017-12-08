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
        public RunDemandNoticeService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            taxpayerDao = new TaxpayerDao(_db);
            demandNoticeDao = new DemandNoticeDao(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersDao(_db);
            errorDao = new ErrorDao(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemDao(_db);
            demandNoticeArrearDao = new DemandNoticeArrearDao(_db);
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
                    // get all demandnotice by taxpayer and year in domainnotce taxpayer table
                    if (taxpayers.Count > 0)
                    {
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

                                Response response = await demandNoticeTaxpayersDao.Add(dntd);
                                // billing number = response.data

                                if (response.code == MsgCode_Enum.SUCCESS)
                                {
                                    dntd.billingNumber = response.data.ToString();
                                    //run arrears 
                                    await RunArrears(dntd);
                                    //run penalties to current year


                                    await RunDemandNoticeItem(dntd); //run items 
                                }
                                else
                                {
                                    // log error
                                    Error error = new Error()
                                    {
                                        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        errorvalue = response.description,
                                        ownerId = dntd.dnId
                                    };
                                    bool result = await errorDao.Add(error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
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

            //

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


        //penalty jobs by demand notice
        private void TaxpayerPenalty()
        {
            // scan through demandnoticeitem check the year/penalty duration compare if not passed then create penalty
        }
    }
}
