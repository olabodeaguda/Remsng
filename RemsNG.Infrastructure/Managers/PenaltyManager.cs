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
    public class PenaltyManager : IPenaltyManager
    {
        private IHttpContextAccessor _httpAccessor;
        private readonly DemandNoticeTaxpayersRepository _dNTaxpayersRep;
        private readonly DemandNoticePenaltyRepository _penaltyRepo;
        public PenaltyManager(DbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _dNTaxpayersRep = new DemandNoticeTaxpayersRepository(db);
            _penaltyRepo = new DemandNoticePenaltyRepository(db);
            _httpAccessor = httpContextAccessor;
        }

        public async Task<bool> AddPenalty(Guid[] dnTaxpayerIds)
        {
            if (dnTaxpayerIds.Length <= 0)
            {
                throw new UserValidationException("Taxpayer is required");
            }

            DemandNoticeTaxpayersModel[] dnTaxpayer = (await _dNTaxpayersRep.ById(dnTaxpayerIds))
               .Where(d => !d.IsRunPenalty).ToArray();

            if (dnTaxpayer.Length <= 0)
            {
                throw new NotFoundException("No record found");
            }

            DemandNoticePenaltyModel[] previousPenalty = await _penaltyRepo.ByTaxpayerId(dnTaxpayer.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticePenaltyModel> newPenalty = new List<DemandNoticePenaltyModel>();

            foreach (var tm in dnTaxpayer.GroupBy(x => x.TaxpayerId))
            {
                var ndTaxpayer = tm.FirstOrDefault();
                decimal currAmount = (tm.SelectMany(x => x.DemandNoticeItem).Sum(x => (x.ItemAmount - x.AmountPaid))) * (decimal)0.1;
                var pArrears = previousPenalty.Where(x => x.TaxpayerId == tm.Key).Sum(t => (t.TotalAmount - t.AmountPaid));

                DemandNoticePenaltyModel dnpPenalty = new DemandNoticePenaltyModel()
                {
                    AmountPaid = 0,
                    ItemPenaltyStatus = "PENDING",
                    BillingNo = ndTaxpayer.BillingNumber,
                    BillingYear = DateTime.Now.Year,
                    CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    OriginatedYear = ndTaxpayer.BillingYr,
                    TaxpayerId = ndTaxpayer.TaxpayerId,
                    TotalAmount = pArrears + currAmount,
                    CurrentAmount = currAmount
                };
                if (dnpPenalty.TotalAmount > 0)
                {
                    newPenalty.Add(dnpPenalty);
                }
            }

            bool result = await _penaltyRepo.CreatePenalty(newPenalty.ToArray());
            if (result)
            {
                await _penaltyRepo.UpdatePenaltyStatus(previousPenalty, "CLOSED");
                await _dNTaxpayersRep.UpdatePenaltyStatus(dnTaxpayer, true);
            }
            return result;
        }

        public async Task<bool> RemovePenalty(Guid[] dnTaxpayerIds)
        {
            if (dnTaxpayerIds.Length <= 0)
            {
                throw new UserValidationException("Taxpayer is required");
            }

            DemandNoticeTaxpayersModel[] dnTaxpayer = (await _dNTaxpayersRep.ById(dnTaxpayerIds))
               .Where(d => !d.IsRunPenalty).ToArray();

            if (dnTaxpayer.Length <= 0)
            {
                throw new NotFoundException("No record found");
            }

            DemandNoticePenaltyModel[] previousPenalty = await _penaltyRepo.ByTaxpayerId(dnTaxpayer.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticePenaltyModel> newPenalty = new List<DemandNoticePenaltyModel>();

            foreach (var tm in dnTaxpayer.GroupBy(x => x.TaxpayerId))
            {
                var ndTaxpayer = tm.FirstOrDefault();
                decimal currAmount = (tm.SelectMany(x => x.DemandNoticeItem).Sum(x => (x.ItemAmount - x.AmountPaid))) * (decimal)0.1;
                var pArrears = previousPenalty.Where(x => x.TaxpayerId == tm.Key).Sum(t => (t.TotalAmount - t.AmountPaid));

                DemandNoticePenaltyModel dnpPenalty = new DemandNoticePenaltyModel()
                {
                    AmountPaid = 0,
                    ItemPenaltyStatus = "PENDING",
                    BillingNo = ndTaxpayer.BillingNumber,
                    BillingYear = DateTime.Now.Year,
                    CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    OriginatedYear = ndTaxpayer.BillingYr,
                    TaxpayerId = ndTaxpayer.TaxpayerId,
                    TotalAmount = pArrears - currAmount,
                    CurrentAmount = 0
                };
                if (dnpPenalty.TotalAmount > 0)
                {
                    newPenalty.Add(dnpPenalty);
                }
            }

            bool result = await _penaltyRepo.CreatePenalty(newPenalty.ToArray());
            if (result)
            {
                await _penaltyRepo.UpdatePenaltyStatus(previousPenalty, "CLOSED");
                await _dNTaxpayersRep.UpdatePenaltyStatus(dnTaxpayer, false);
            }
            return result;
        }


    }
}
