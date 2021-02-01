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
        private readonly IDemandNoticePaymentHistoryRepository _paymentRepository;
        private readonly IDemandNoticeItemRepository _demandNoticeRepository;
        private readonly IDemandNoticeArrearRepository _arrearsRepo;
        public PenaltyManager(IHttpContextAccessor httpContextAccessor,
            IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            IDemandNoticePenaltyRepository demandNoticePenaltyRepository, IDemandNoticePaymentHistoryRepository demandNoticePaymentHistoryRepository,
            IDemandNoticeItemRepository demandNoticeItemRepository,
            IDemandNoticeArrearRepository demandNoticeArrearRepository)
        {
            _arrearsRepo = demandNoticeArrearRepository;
            _demandNoticeRepository = demandNoticeItemRepository;
            _paymentRepository = demandNoticePaymentHistoryRepository;
            _dNTaxpayersRep = demandNoticeTaxpayersRepository;
            _penaltyRepo = demandNoticePenaltyRepository;
            _httpAccessor = httpContextAccessor;
        }

        public async Task<bool> AddPenalty2(Guid[] dnTaxpayerIds)
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

        public async Task<bool> AddPenalty(Guid[] demandNoticeIds)
        {
            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            var demandNoticeTaxpayer = await _dNTaxpayersRep.ById(demandNoticeIds);
            if (demandNoticeTaxpayer.Length <= 0) return false;

            foreach (var tm in demandNoticeTaxpayer)
            {
                List<DemandNoticePenaltyModel> newPenalty = new List<DemandNoticePenaltyModel>();
                var demandNotice = await _dNTaxpayersRep.getPendingDemandNoticeByTaxpayerByIds(tm.TaxpayerId);

                var items = await _demandNoticeRepository.ByBillingNumber(demandNotice.Select(a => a.BillingNumber).ToArray());
                DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(demandNotice.Select(x => x.TaxpayerId).ToArray());
                DemandNoticePenaltyModel[] previousPenalty = await _penaltyRepo.ByTaxpayerId(demandNotice.Select(x => x.TaxpayerId).ToArray());
                // get amount paid
                var payments = await _paymentRepository.ByBillingNumbers(demandNotice.Select(x => x.BillingNumber).ToArray());

                var itemAmount = items.Where(a => a.BillingNo == tm.BillingNumber);//.Sum(s => s.ItemAmount);
                var arrears = previousArrears.Where(a => a.TaxpayerId == tm.TaxpayerId);//.Sum(a => a.TotalAmount);
                var amountPaid = payments.Where(a => a.BillingNumber == tm.BillingNumber);//.Sum(q => q.Amount);
                var penalty = previousPenalty.Where(a => a.TaxpayerId == tm.TaxpayerId);//.Sum(s => s.TotalAmount);

                decimal amount = (itemAmount.Sum(s => s.ItemAmount) + arrears.Sum(a => a.TotalAmount) + penalty.Sum(a => a.TotalAmount))
                    - amountPaid.Sum(q => q.Amount);
                if (amount > 0)
                {
                    newPenalty.Add(new DemandNoticePenaltyModel()
                    {
                        AmountPaid = 0,
                        ItemPenaltyStatus = "PENDING",
                        BillingNo = tm.BillingNumber,
                        BillingYear = DateTime.Now.Year,
                        CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Id = Guid.NewGuid(),
                        OriginatedYear = DateTime.Now.Year,
                        TaxpayerId = tm.TaxpayerId,
                        TotalAmount = amount * (decimal)0.1,
                        CurrentAmount = 0
                    });
                }

                if (newPenalty.Count > 0)
                {
                    bool result1 = await _penaltyRepo.CreatePenalty(newPenalty.ToArray());
                    if (result1)
                    {
                        await _penaltyRepo.UpdatePenaltyStatus(previousPenalty, "CLOSED");
                        await _dNTaxpayersRep.UpdatePenaltyStatus(demandNotice.ToArray(), true);
                    }
                }
            }

            return true;

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
