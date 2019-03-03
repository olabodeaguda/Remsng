using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
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
    }
}
