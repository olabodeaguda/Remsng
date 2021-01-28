using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
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
        private readonly IDemandNoticeTaxpayersRepository _dNTaxpayersRep;
        private readonly IDemandNoticePenaltyRepository _penaltyRepo;
        public PenaltyManager(IHttpContextAccessor httpContextAccessor,
            IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            IDemandNoticePenaltyRepository demandNoticePenaltyRepository)
        {
            _dNTaxpayersRep = demandNoticeTaxpayersRepository;
            _penaltyRepo = demandNoticePenaltyRepository;
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

        public async Task<bool> AddPenalty(DemantNoticePenaltyModelExt2[] model)
        {
            if (model.Length < 0)
                return false;

            DemandNoticePenaltyModel[] previousPenalty = await _penaltyRepo.ByTaxpayerId(model.Select(x => x.TaxpayerId).ToArray());
            List<DemandNoticePenaltyModel> newPenalty = new List<DemandNoticePenaltyModel>();
            foreach (var tm in model)
            {
                var prev = previousPenalty.Where(x => x.TaxpayerId == tm.TaxpayerId).ToList();
                if (prev.Count > 0)
                {
                    tm.TotalAmount = tm.TotalAmount + prev.Sum(x => x.TotalAmount);
                }
                newPenalty.Add(tm);
            }

            bool result = await _penaltyRepo.CreatePenalty(newPenalty.ToArray());
            if (result)
            {
                await _penaltyRepo.UpdatePenaltyStatus(previousPenalty, "CLOSED");
                await _dNTaxpayersRep.UpdatePenaltyStatus(model.Select(x => x.DemandNoticeId).ToArray(), true);
            }

            return true;
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
