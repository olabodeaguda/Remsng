using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ArrearsManager : IArrearsManager
    {
        private IHttpContextAccessor _httpAccessor;
        private readonly DemandNoticeArrearRepository _arrearsRepo;
        private readonly DemandNoticeTaxpayersRepository _dNTaxpayersRep;
        public ArrearsManager(DbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _dNTaxpayersRep = new DemandNoticeTaxpayersRepository(db);
            _arrearsRepo = new DemandNoticeArrearRepository(db);
            _httpAccessor = httpContextAccessor;
        }
        public Task RunTaxpayerArrears(Guid taxpayerId)
        {
            // get amount status
            throw new NotImplementedException();
        }

        public async Task RunTaxpayerArrears(Guid[] dnTaxpayerIds)
        {
            if (dnTaxpayerIds.Length <= 0)
            {
                throw new UserValidationException("Taxpayer is required");
            }

            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            DemandNoticeTaxpayersModel[] dnTaxpayer = (await _dNTaxpayersRep.ById(dnTaxpayerIds)).Where(d => payableStatus.Any(s => s == d.DemandNoticeStatus)).ToArray();

            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(dnTaxpayer.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
            foreach (var tm in dnTaxpayer)
            {
                var pArrears = previousArrears.FirstOrDefault(x => x.TaxpayerId == tm.TaxpayerId);
                DemandNoticeArrearsModel dnArrears = new DemandNoticeArrearsModel()
                {
                    AmountPaid = 0,
                    ArrearsStatus = "PENDING",
                    BillingNo = tm.BillingNumber,
                    BillingYear = DateTime.Now.Year,
                    CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ItemId = default(Guid),
                    OriginatedYear = tm.BillingYr,
                    TaxpayerId = tm.TaxpayerId,
                    WardName = tm.WardName,
                    TotalAmount = 0
                };
                newArrears.Add(dnArrears);
            }



            throw new NotImplementedException();
        }
    }
}
