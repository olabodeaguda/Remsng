using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class RunDemandNoticeManager : IRunDemandNoticeManager
    {
        private readonly CompanyItemRepository _companyItemDao;
        private readonly DNPaymentHistoryRepository _dnPaymentHistoryDao;
        private readonly DemandNoticePaymentHistoryRepository _dnpHisotryDao;
        private readonly IDNAmountDueMgtManager _admService;
        private readonly TaxpayerRepository taxpayerDao;
        private readonly DemandNoticeRepository demandNoticeDao;
        private readonly DemandNoticeTaxpayersRepository demandNoticeTaxpayersDao;
        private readonly ErrorRepository errorDao;
        private readonly ILogger logger;
        private readonly DemandNoticeItemRepository demandNoticeItemDao;
        private readonly DemandNoticeArrearRepository demandNoticeArrearDao;
        private readonly DemandNoticePenaltyRepository demandNoticePenaltyDao;
        private readonly LcdaRepository lcdaDao;
        private readonly WardRepository wardDao;
        private readonly StreetRepository streetDao;
        private readonly IAddressManager address;
        private readonly ILcdaManager lcdaService;
        private readonly IStateManagers stateService;
        private readonly IImageManager imageService;
        private readonly IDnDownloadManager dnDownloadService;
        private readonly IBatchDwnRequestManager batchDwnRequestService;
        private readonly IDemandNoticeTaxpayerManager demandNoticeTaxpayerService;
        private readonly ITaxpayerManager taxpayerService;
        private readonly IListPropertyManager listPropertyService;
        private IHostingEnvironment _hostEnvironment;
        private INodeServices nodeServices;
        private IServiceProvider serviceProvider;
        private DbContext db;
        public RunDemandNoticeManager(DbContext _db,
            ILoggerFactory loggerFactory, IAddressManager _address,
            ILcdaManager _lcdaService, IStateManagers _stateService,
            IImageManager _imageService,
            IDnDownloadManager _dnDownloadService, IBatchDwnRequestManager _batchDwnRequestService,
            IDemandNoticeTaxpayerManager _demandNoticeTaxpayerService
            , ITaxpayerManager _taxpayerService,
            IListPropertyManager _listPropertyService,
            INodeServices _nodeServices,
            IHostingEnvironment hostingEnvironment,
            IServiceProvider _serviceProvider,
            IDNAmountDueMgtManager dNAmountDueMgtService)
        {
            db = _db;
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            nodeServices = _nodeServices;
            _hostEnvironment = hostingEnvironment;
            serviceProvider = _serviceProvider;
            taxpayerDao = new TaxpayerRepository(_db);
            demandNoticeDao = new DemandNoticeRepository(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersRepository(_db);
            errorDao = new ErrorRepository(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemRepository(_db);
            demandNoticeArrearDao = new DemandNoticeArrearRepository(_db);
            demandNoticePenaltyDao = new DemandNoticePenaltyRepository(_db);
            lcdaDao = new LcdaRepository(_db, loggerFactory);
            wardDao = new WardRepository(_db);
            streetDao = new StreetRepository(_db, loggerFactory);
            address = _address;
            lcdaService = _lcdaService;
            imageService = _imageService;
            stateService = _stateService;
            dnDownloadService = _dnDownloadService;
            batchDwnRequestService = _batchDwnRequestService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
            listPropertyService = _listPropertyService;
            taxpayerService = _taxpayerService;
            _admService = dNAmountDueMgtService;
            _dnpHisotryDao = new DemandNoticePaymentHistoryRepository(_db);
            _dnPaymentHistoryDao = new DNPaymentHistoryRepository(_db);
            _companyItemDao = new CompanyItemRepository(_db);
        }

        //public async Task RegisterTaxpayer()
        //{
        //    // DemandNoticeModel demandNotice = await demandNoticeDao.DequeueDemandNotice();
        //    //try
        //    //{
        //    //    if (demandNotice != null)
        //    //    {
        //    //        string query = EncryptDecryptUtils.FromHexString(demandNotice.Query);
        //    //        DemandNoticeRequestModel demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(query);
        //    //        demandNoticeRequest.createdBy = demandNotice.CreatedBy;
        //    //        List<TaxPayerModel> taxpayers = await taxpayerDao.GetActiveTaxpayers(demandNoticeRequest);
        //    //        string domainName = string.Empty;
        //    //        if (taxpayers.Count > 0)
        //    //        {
        //    //            LcdaModel lcda = await lcdaService.Get(demandNoticeRequest);
        //    //            if (lcda != null)
        //    //            {
        //    //                DomainModel dd = await lcdaDao.GetDomain(lcda.Id);
        //    //                if (dd != null)
        //    //                {
        //    //                    domainName = dd.DomainName;
        //    //                }
        //    //            }

        //    //            List<DemandNoticeTaxpayersModel> dt =
        //    //                await demandNoticeTaxpayersDao.getTaxpayerByIds(taxpayers
        //    //                .Select(x => string.Format("'{0}'", x.Id)).ToArray(), demandNotice.BillingYear);

        //    //            foreach (var tm in taxpayers)
        //    //            {
        //    //                var itExist = dt.FirstOrDefault(x => x.TaxpayerId == tm.Id);
        //    //                if (itExist != null)
        //    //                {
        //    //                    ErrorModel error = new ErrorModel()
        //    //                    {
        //    //                        ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
        //    //                        Errorvalue = $"Demand notice have already been raised for {tm.Surname} {tm.Firstname} {tm.Lastname} " +
        //    //                          $"for billing year {demandNotice.BillingYear}",
        //    //                        OwnerId = demandNotice.Id
        //    //                    };
        //    //                    bool result = await errorDao.Add(error);
        //    //                    continue;
        //    //                }
        //    //                else
        //    //                {
        //    //                    DemandNoticeTaxpayersModel dntd = new DemandNoticeTaxpayersModel();
        //    //                    dntd.BillingYr = demandNotice.BillingYear;
        //    //                    dntd.DnId = demandNotice.Id;
        //    //                    dntd.CreatedBy = demandNoticeRequest.createdBy;
        //    //                    dntd.TaxpayerId = tm.Id;
        //    //                    dntd.DomainName = domainName;
        //    //                    dntd.LcdaAddress = await address.AddressByOwnerId(lcda.Id);
        //    //                    dntd.LcdaState = await stateService.StateNameByLcda(lcda.Id);
        //    //                    dntd.LcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
        //    //                        ImgTypesEnum.LOGO.ToString());
        //    //                    dntd.RevCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
        //    //                        ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
        //    //                    dntd.CouncilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
        //    //                        ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());
        //    //                    dntd.IsUnbilled = demandNotice.IsUnbilled;

        //    //                    // get taxpayer company item
        //    //                    var items = await _companyItemDao.ByTaxpayer(tm.Id);
        //    //                    if (items.Count < 1)
        //    //                    {
        //    //                        continue;
        //    //                    }

        //    //                    Response response1 = await demandNoticeTaxpayersDao.Add(dntd);

        //    //                    if (response1.code == MsgCode_Enum.SUCCESS)
        //    //                    {
        //    //                        dntd.BillingNumber = response1.data.ToString().Trim();
        //    //                        if (demandNoticeRequest.RunPenalty)
        //    //                        {
        //    //                            await RunTaxpayerPenalty(tm.Id, dntd.BillingNumber, dntd.BillingYr);
        //    //                        }

        //    //                        if (demandNoticeRequest.RunArrears)
        //    //                        {
        //    //                            await RunArrears(dntd, dntd.BillingNumber, demandNoticeRequest.RunArrearsCategory);
        //    //                        }

        //    //                        await RunDemandNoticeItem(dntd); //run items
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        // log error
        //    //                        ErrorModel error = new ErrorModel()
        //    //                        {
        //    //                            ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
        //    //                            Errorvalue = response1.description,
        //    //                            OwnerId = dntd.DnId
        //    //                        };
        //    //                        bool result = await errorDao.Add(error);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            ErrorModel error = new ErrorModel()
        //    //            {
        //    //                ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
        //    //                Errorvalue = $"{taxpayers.Count} taxpayer was found from the query request ",
        //    //                OwnerId = demandNotice.Id
        //    //            };
        //    //            bool result = await errorDao.Add(error);
        //    //        }
        //    //        //update demand notice
        //    //        demandNotice.DemandNoticeStatus = DemandNoticeStatus.COMPLETED.ToString();
        //    //        Response response = await demandNoticeDao.UpdateStatus(demandNotice);
        //    //        if (response.code == MsgCode_Enum.SUCCESS)
        //    //        {
        //    //            logger.LogInformation($"{demandNotice.Id} has been completed");
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception x)
        //    //{
        //    //    // cancel no retry
        //    //    demandNotice.DemandNoticeStatus = DemandNoticeStatus.ERROR.ToString();
        //    //    Response response = await demandNoticeDao.UpdateStatus(demandNotice);
        //    //    if (response.code == MsgCode_Enum.SUCCESS)
        //    //    {
        //    //        logger.LogInformation($"{demandNotice.Id} has been completed");
        //    //    }
        //    //    logger.LogError(x.Message);
        //    //}
        //}

        public async Task GenerateBulkDemandNotice()
        {
            BatchDemandNoticeModel bdnm = null;// await batchDwnRequestService.Dequeue();
            try
            {
                bdnm = await batchDwnRequestService.Dequeue();
                if (bdnm != null)
                {
                    List<DemandNoticeTaxpayersModel> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        LcdaModel lgda = await taxpayerService.getLcda(firstTaxpayer.TaxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.Id);

                        string rootUrl = _hostEnvironment.WebRootPath == null ? @"C:\" : _hostEnvironment.WebRootPath;

                        string rootPath = Path.Combine(rootUrl, "zipReports", bdnm.batchNo);
                        if (!Directory.Exists(rootPath))
                        {
                            Directory.CreateDirectory(rootPath);
                        }

                        List<string> lstContent = new List<string>();
                        var htmlContent = await File.ReadAllTextAsync($"{rootUrl}/templates/{template}");
                        string htmlContents = string.Empty;
                        for (int i = 0; i < lstOfDN.Count; i++)
                        {
                            string s = await dnDownloadService.PopulateReportHtml(htmlContent, lstOfDN[i].BillingNumber, rootUrl, bdnm.createdBy);

                            htmlContents = htmlContents + (s == null || string.IsNullOrEmpty(s) ? "" : s);
                            if (i < 1)
                            {
                                continue;
                            }
                            if (i % 90 == 0 || i == (lstOfDN.Count - 1))
                            {
                                lstContent.Add(htmlContents);
                                htmlContents = string.Empty;
                            }
                        }

                        using (FileStream zipToOpen = new FileStream($"{rootPath}/{bdnm.batchNo}.zip", FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                int count = 0;
                                foreach (var dnt in lstContent)
                                {
                                    var result = await nodeServices.InvokeAsync<byte[]>("./pdf", dnt);

                                    string filePath = Path.Combine(rootPath, $"Batch {count + 1}.pdf");
                                    using (FileStream fs = System.IO.File.Create(filePath))
                                    {
                                        await fs.WriteAsync(result, 0, result.Length);
                                        fs.Flush();
                                    }
                                    archive.CreateEntryFromFile(filePath, $"batch {count + 1}.pdf");
                                    count++;
                                }
                            }
                        }
                    }

                    //update request
                    Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                    {
                        id = bdnm.id,
                        batchFileName = $"{bdnm.batchNo}.zip",
                        requestStatus = "COMPLETED",
                        createdBy = "APPLICATION"

                    });
                }
            }
            catch (Exception x)
            {
                if (bdnm != null)
                {
                    Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                    {
                        id = bdnm.id,
                        batchFileName = $"{bdnm.batchNo}.zip",
                        requestStatus = "ERROR",
                        createdBy = "APPLICATION"
                    });
                }
                logger.LogError(x.Message);
            }
        }

        private async Task<bool> RunArrears(DemandNoticeTaxpayersModel dntd, string billNumber, int arrearCategory)
        {
            int billingYr = arrearCategory > 3 ? dntd.BillingYr - 1 : dntd.BillingYr;
            try
            {
                List<DemandNoticeItemModel> items = await demandNoticeItemDao.UnpaidBillsByTaxpayerId(dntd.TaxpayerId, billNumber, billingYr);
                if (items.Count > 0)
                {
                    string oldBillnumber = items.FirstOrDefault().BillingNo;
                    List<DemandNoticePaymentHistoryModel> payments = await _dnpHisotryDao.ApprovedPaymentHistory(dntd.TaxpayerId, billingYr);
                    var arrears = await demandNoticeArrearDao.ByTaxpayer(dntd.TaxpayerId);
                    var penalty = await demandNoticePenaltyDao.ByTaxpayerId(dntd.TaxpayerId, billingYr);

                    decimal amountDue = items.Sum(x => x.ItemAmount) + arrears.Sum(x => x.TotalAmount)
                        + penalty.Sum(x => x.TotalAmount)
                         - payments.Sum(x => x.Amount);

                    if (amountDue > 0)
                    {
                        DemandNoticeArrearsModel dna = new DemandNoticeArrearsModel()
                        {
                            AmountPaid = payments.Sum(x => x.Amount),
                            ArrearsStatus = DemandNoticeStatus.PENDING.ToString(),
                            BillingNo = billNumber,
                            BillingYear = dntd.BillingYr,
                            CreatedBy = "Application",
                            Id = Guid.NewGuid(),
                            ItemId = dntd.TaxpayerId,
                            OriginatedYear = billingYr,
                            TaxpayerId = dntd.TaxpayerId,
                            TotalAmount = amountDue
                        };

                        string query = string.Empty;
                        query = query + demandNoticeArrearDao.AddQuery(dna);
                        string itemQ = items.Select(x => x.Id.ToString()).ToArray().FormatString();
                        if (!string.IsNullOrEmpty(itemQ))
                        {
                            query = query + $"update tbl_demandNoticeItem set itemStatus='MOVE_TO_ARREARS' " +
                                        $"where id in ({itemQ});";
                        }

                        if (arrears.Count > 0)
                        {
                            string arrearsQ = arrears.Select(x => x.Id.ToString()).ToArray().FormatString();
                            query = query + $"update tbl_demandNoticeArrears set arrearsStatus = 'MOVED' " +
                                              $"where id in ({arrearsQ});";
                        }

                        query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = 'CLOSED' where billingNumber = '{oldBillnumber}'";

                        if (!string.IsNullOrEmpty(query))
                        {
                            return await demandNoticeArrearDao.AddArrears(query);
                        }
                    }
                    else if (amountDue < 0)
                    {
                        // register the prepayment
                        //await _dnPaymentHistoryDao.AddPrepaymentForAlreadyRegisterdAmount(new Prepayment()
                        //{
                        //    amount = amountDue * -1,
                        //    prepaymentStatus = "ACTIVE",
                        //    taxpayerId = dntd.taxpayerId
                        //});
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayersModel dntd)
        {
            var companyItems = await _companyItemDao.ByTaxpayer(dntd.TaxpayerId);
            DemandNoticeItemModel[] dniModel = companyItems.Where(x => x.CompanyStatus == CompanyStatus.ACTIVE.ToString())
                .Select(x => new DemandNoticeItemModel()
                {
                    BillingNo = dntd.BillingNumber,
                    CreatedBy = dntd.CreatedBy,
                    DateCreated = DateTime.Now,
                    DnTaxpayersDetailsId = dntd.Id,
                    Id = Guid.NewGuid(),
                    ItemAmount = x.Amount,
                    ItemId = x.ItemId,
                    ItemName = x.ItemName,
                    TaxpayerId = dntd.TaxpayerId,
                    ItemStatus = DemandNoticeItemStatus.PENDING.ToString(),
                    AmountPaid = 0
                }).ToArray();
            Response response = await demandNoticeItemDao.Add(dniModel);
            if (response.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(response.description, dntd);
            }
        }

        public async Task RunTaxpayerPenalty(Guid taxpayerId, string billingNumber, int billingYr)
        {
            Guid[] taxpayerIds = new Guid[] { taxpayerId };
            var recievables = await demandNoticeTaxpayersDao.GetAllReceivables(taxpayerIds);// unpaid taxpayer

            var recievable = recievables.FirstOrDefault();
            List<DemandNoticeItemModel> items =
                    await demandNoticeItemDao.UnpaidBillsByTaxpayerId(taxpayerId, billingNumber, billingYr - 1);
            if (recievable != null && items.Count > 0)
            {
                List<DemandNoticePaymentHistoryModel> payments = await _dnpHisotryDao.ApprovedPaymentHistory(taxpayerId, billingYr);
                var arrears = await demandNoticeArrearDao.ByTaxpayer(taxpayerId);
                decimal amountDue = items.Sum(x => x.ItemAmount) + arrears.Sum(x => x.TotalAmount)
                     - payments.Sum(x => x.Amount);

                string query = string.Empty;
                if (amountDue > 0)
                {
                    DemandNoticePenaltyModel dnp = new DemandNoticePenaltyModel()
                    {
                        BillingNo = billingNumber,
                        AmountPaid = 0,
                        BillingYear = billingYr,
                        ItemId = Guid.Empty,
                        ItemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                        OriginatedYear = recievable.BillingYr,
                        TaxpayerId = taxpayerId,
                        TotalAmount = amountDue * (decimal)(0.1)
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
                            logger.LogInformation("Error running penalty " + response.description);
                        }
                        else
                        {
                            logger.LogInformation("Error running penalty " + response.description);
                        }
                    }
                    catch (Exception x)
                    {
                        logger.LogError(x.Message);
                    }
                }
            }
        }

        public async Task ReconcileDemandNotice()

        {
            //var result = await db.Set<DemandNotice>()
            //    .Include(x => x.DemandNoticeItem)
            //    .Include(x => x.DemandNoticeTaxpayers).Select(x =>
            //    new
            //    {
            //        x.DemandNoticeItem,
            //        x.DemandNoticeTaxpayers
            //    }).Take(100).ToListAsync();

            //foreach (var tm in result)
            //{

            //}

        }
    }
}
