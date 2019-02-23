using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeItemManagers : IDemandNoticeItemManagers
    {
        private readonly CompanyItemRepository _companyItemDao;
        private readonly DemandNoticeItemRepository dnDao;
        private readonly ILogger logger;
        public DemandNoticeItemManagers(DbContext _db, ILoggerFactory loggerFactory)
        {
            dnDao = new DemandNoticeItemRepository(_db);
            _companyItemDao = new CompanyItemRepository(_db);
            logger = logger = loggerFactory.CreateLogger("Demand Notice Item manager");
        }

        public async Task<List<DemandNoticeItemModel>> ByBillingNumber(string billingno)
        {
            return await dnDao.ByBillingNumber(billingno);
        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayers dntd)
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
            Response response = await dnDao.Add(dniModel);
            if (response.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(response.description, dntd);
            }
        }
        
    }
}
