﻿using Microsoft.AspNetCore.Http;
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
    public class ArrearsManager : IArrearsManager
    {
        private IPenaltyManager _penaltyManager;
        private IHttpContextAccessor _httpAccessor;
        private readonly IDemandNoticeArrearRepository _arrearsRepo;
        private readonly IDemandNoticeTaxpayersRepository _dNTaxpayersRep;
        private readonly IDemandNoticePaymentHistoryRepository _paymentRepository;
        private readonly IDemandNoticeItemRepository _demandNoticeRepository;
        private readonly IDemandNoticePenaltyRepository _penaltyRepo;
        public ArrearsManager(IHttpContextAccessor httpContextAccessor,
            IDemandNoticeArrearRepository demandNoticeArrearRepository,
            IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            IDemandNoticePaymentHistoryRepository demandNoticePaymentHistoryRepository
            , IPenaltyManager penaltyManager
            , IDemandNoticeItemRepository demandNoticeItemRepository, IDemandNoticePenaltyRepository demandNoticePenaltyRepository)
        {
            _penaltyRepo = demandNoticePenaltyRepository;
            _demandNoticeRepository = demandNoticeItemRepository;
            _penaltyManager = penaltyManager;
            _paymentRepository = demandNoticePaymentHistoryRepository;
            _dNTaxpayersRep = demandNoticeTaxpayersRepository;
            _arrearsRepo = demandNoticeArrearRepository;
            _httpAccessor = httpContextAccessor;
        }

        public async Task<bool> AddArrears(Guid dntId, decimal amount, Guid itemId)
        {
            if (dntId == default(Guid))
            {
                throw new UserValidationException("Demand Notice is required");
            }
            if (amount <= 0)
            {
                throw new UserValidationException("Amount is required");
            }

            var dnt = await _dNTaxpayersRep.ById(dntId);
            if (dnt == null)
            {
                throw new NotFoundException("No record found");
            }

            DemandNoticeArrearsModel[] previousArrears = (await _arrearsRepo.ByTaxpayer(dnt.TaxpayerId)).ToArray();
            var pArrears = previousArrears.Sum(t => (t.TotalAmount - t.AmountPaid));
            DemandNoticeArrearsModel dnArrears = new DemandNoticeArrearsModel()
            {
                AmountPaid = 0,
                ArrearsStatus = "PENDING",
                BillingNo = dnt.BillingNumber,
                BillingYear = DateTime.Now.Year,
                CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                DateCreated = DateTime.Now,
                Id = Guid.NewGuid(),
                ItemId = itemId,
                OriginatedYear = dnt.BillingYr,
                TaxpayerId = dnt.TaxpayerId,
                WardName = dnt.WardName,
                TotalAmount = pArrears + amount,
                CurrentAmount = amount
            };

            bool result = await _arrearsRepo.AddArrears(dnArrears);
            if (result)
            {
                await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
            }

            return result;
        }
        public async Task<bool> RunTaxpayerArrears1(Guid[] dnTaxpayerIds)
        {
            if (dnTaxpayerIds.Length <= 0)
            {
                throw new UserValidationException("Taxpayer is required");
            }

            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            DemandNoticeTaxpayersModel[] dnTaxpayer = (await _dNTaxpayersRep.ById(dnTaxpayerIds))
                .Where(d => payableStatus.Any(s => s == d.DemandNoticeStatus) && !d.IsRunArrears).ToArray();

            if (dnTaxpayer.Length <= 0)
            {
                throw new NotFoundException("No record found");
            }

            var payments = await _paymentRepository.ByBillingNumbers(dnTaxpayer.Select(x => x.BillingNumber).ToArray());


            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(dnTaxpayer.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
            foreach (var tm in dnTaxpayer.GroupBy(x => x.TaxpayerId))
            {
                var ndTaxpayer = tm.FirstOrDefault();
                decimal currAmount = tm.SelectMany(x => x.DemandNoticeItem)
                    .Sum(x => x.ItemAmount);

                var pArrears = previousArrears.Where(x => x.TaxpayerId == tm.Key)
                    .Sum(t => t.TotalAmount);

                var amountPaid = payments.
                    Where(x => x.BillingNumber == ndTaxpayer.BillingNumber)
                    .Sum(x => x.Amount);

                decimal newArrear = (currAmount + pArrears) - amountPaid;
                DemandNoticeArrearsModel dnArrears = new DemandNoticeArrearsModel()
                {
                    AmountPaid = 0,
                    ArrearsStatus = "PENDING",
                    BillingNo = ndTaxpayer.BillingNumber,
                    BillingYear = DateTime.Now.Year,
                    CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ItemId = default(Guid),
                    OriginatedYear = ndTaxpayer.BillingYr,
                    TaxpayerId = ndTaxpayer.TaxpayerId,
                    WardName = ndTaxpayer.WardName,
                    TotalAmount = newArrear,
                    CurrentAmount = currAmount
                };
                newArrears.Add(dnArrears);
            }

            bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
            if (result)
            {
                await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
                await _dNTaxpayersRep.UpdateArrearsStatus(dnTaxpayer, true);
            }
            return result;
        }
        public async Task<bool> RemoveTaxpayerArrears(Guid[] dnTaxpayerIds)
        {
            if (dnTaxpayerIds.Length <= 0)
            {
                throw new UserValidationException("Taxpayer is required");
            }

            DemandNoticeTaxpayersModel[] dnTaxpayer = (await _dNTaxpayersRep.ById(dnTaxpayerIds))
               .Where(d => d.IsRunArrears).ToArray();

            if (dnTaxpayer.Length <= 0)
            {
                throw new NotFoundException("No record found");
            }

            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(dnTaxpayer.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();

            foreach (var tm in dnTaxpayer.GroupBy(x => x.TaxpayerId))
            {
                var ndTaxpayer = tm.FirstOrDefault();
                decimal currAmount = tm.SelectMany(x => x.DemandNoticeItem).Sum(x => (x.ItemAmount - x.AmountPaid));
                var pArrears = previousArrears.Where(x => x.TaxpayerId == tm.Key).Sum(t => (t.TotalAmount - t.AmountPaid));

                DemandNoticeArrearsModel dnArrears = new DemandNoticeArrearsModel()
                {
                    AmountPaid = 0,
                    ArrearsStatus = "PENDING",
                    BillingNo = ndTaxpayer.BillingNumber,
                    BillingYear = DateTime.Now.Year,
                    CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    OriginatedYear = ndTaxpayer.BillingYr,
                    TaxpayerId = ndTaxpayer.TaxpayerId,
                    WardName = ndTaxpayer.WardName,
                    TotalAmount = pArrears - currAmount,
                    CurrentAmount = 0
                };
                if (dnArrears.TotalAmount > 0)
                {
                    newArrears.Add(dnArrears);
                }
            }

            if (newArrears.Count > 0)
            {
                bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
            }
            await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
            await _dNTaxpayersRep.UpdateArrearsStatus(dnTaxpayer, false);

            return true;
        }
        public async Task<bool> RunTaxpayerArrears2(Guid[] demandNoticeIds)
        {
            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            var demandNoticeTaxpayer = await _dNTaxpayersRep.ById(demandNoticeIds);
            if (demandNoticeTaxpayer.Length <= 0) return false;

            var demandNotice = demandNoticeTaxpayer.Where(x => payableStatus.Any(t => t == x.DemandNoticeStatus) && !x.IsRunArrears).ToArray();
            if (demandNotice.Length <= 0) return false;

            var payments = await _paymentRepository.ByBillingNumbers(demandNotice.Select(x => x.BillingNumber).ToArray());
            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(demandNotice.Select(x => x.TaxpayerId).ToArray());
            previousArrears = previousArrears.Where(d => d.ArrearsStatus == "ACTIVE").ToArray();
            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
            foreach (var tm in demandNotice)
            {
                decimal items = tm.DemandNoticeItem.Sum(x => x.ItemAmount);
                var arr = previousArrears.Where(x => x.TaxpayerId == tm.TaxpayerId).ToArray();
                decimal arrears = arr.Sum(x => x.TotalAmount);
                var payment = payments.Where(x => x.BillingNumber == tm.BillingNumber).Sum(x => x.Amount);

                decimal currentArrears = (items + arrears) - payment;

                if (currentArrears > 0)
                {
                    newArrears.Add(new DemandNoticeArrearsModel()
                    {
                        AmountPaid = 0,
                        ArrearsStatus = "PENDING",
                        BillingNo = tm.BillingNumber,
                        BillingYear = DateTime.Now.Year,
                        CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Id = Guid.NewGuid(),
                        OriginatedYear = tm.BillingYr,
                        TaxpayerId = tm.TaxpayerId,
                        WardName = tm.WardName,
                        TotalAmount = currentArrears,
                        CurrentAmount = 0
                    });
                }
            }

            if (newArrears.Count > 0)
            {
                bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
                if (result)
                {
                    await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
                    await _dNTaxpayersRep.UpdateArrearsStatus(demandNotice, true);
                    await _dNTaxpayersRep.UpdateDemandNoticeStatus(demandNotice.Select(x => x.Id).ToArray(), Common.Utilities.DemandNoticeStatus.CLOSED);
                }
            }

            return true;
        }
        public async Task<bool> RunTaxpayerArrears(Guid[] demandNoticeIds, DemandNoticeRequestModel model, int billingYr)
        {
            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            var demandNoticeTaxpayer = await _dNTaxpayersRep.ById(demandNoticeIds);
            if (demandNoticeTaxpayer.Length <= 0) return false;

            var demandNotice = demandNoticeTaxpayer.Where(x => payableStatus.Any(t => t == x.DemandNoticeStatus) && !x.IsRunArrears).ToArray();
            if (demandNotice.Length <= 0) return false;

            var payments = await _paymentRepository.ByBillingNumbers(demandNotice.Select(x => x.BillingNumber).ToArray());
            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(demandNotice.Select(x => x.TaxpayerId).ToArray());
            previousArrears = previousArrears.Where(d => d.ArrearsStatus == "PENDING" || d.ArrearsStatus == "PART_PAYMENT").ToArray();
            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
            List<DemantNoticePenaltyModelExt2> newPenalty = new List<DemantNoticePenaltyModelExt2>();
            foreach (var tm in demandNotice)
            {
                decimal items = tm.DemandNoticeItem.Sum(x => x.ItemAmount);
                var arr = previousArrears.Where(x => x.TaxpayerId == tm.TaxpayerId).ToArray();
                decimal arrears = arr.Sum(x => x.TotalAmount);
                var payment = payments.Where(x => x.BillingNumber == tm.BillingNumber).Sum(x => x.Amount);
                decimal currentArrears = (items + arrears) - payment;
                decimal penalty = 0;
                if (model.RunPenalty)
                {
                    penalty = currentArrears * 0.1m;
                    newPenalty.Add(new DemantNoticePenaltyModelExt2
                    {
                        TaxpayerId = tm.TaxpayerId,
                        TotalAmount = penalty,
                        BillingNo = tm.BillingNumber,
                        BillingYear = billingYr,
                        CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Id = Guid.NewGuid(),
                        OriginatedYear = tm.BillingYr,
                        CurrentAmount = 0,
                        ItemPenaltyStatus = "PENDING",
                        DemandNoticeId = tm.Id
                    });
                }

                if (currentArrears > 0)
                {
                    newArrears.Add(new DemandNoticeArrearsModel()
                    {
                        AmountPaid = 0,
                        ArrearsStatus = "PENDING",
                        BillingNo = tm.BillingNumber,
                        BillingYear = billingYr,
                        CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Id = Guid.NewGuid(),
                        OriginatedYear = tm.BillingYr,
                        TaxpayerId = tm.TaxpayerId,
                        WardName = tm.WardName,
                        TotalAmount = currentArrears,
                        CurrentAmount = 0
                    });
                }
            }

            if (newArrears.Count > 0)
            {
                bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
                if (result)
                {
                    await _penaltyManager.AddPenalty(newPenalty.ToArray());
                    await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
                    await _dNTaxpayersRep.UpdateArrearsStatus(demandNotice, true);
                    await _dNTaxpayersRep.UpdateDemandNoticeStatus(demandNotice.Select(x => x.Id).ToArray(), Common.Utilities.DemandNoticeStatus.CLOSED);
                }
            }

            return true;
        }

        public async Task<bool> RunTaxpayerArrears(Guid[] demandNoticeIds)
        {
            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            var demandNoticeTaxpayer = await _dNTaxpayersRep.ById(demandNoticeIds);
            if (demandNoticeTaxpayer.Length <= 0) return false;

            foreach (var tm in demandNoticeTaxpayer)
            {
                List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
                var demandNotice = await _dNTaxpayersRep.getPendingDemandNoticeByTaxpayerByIds(tm.TaxpayerId);

                var items = await _demandNoticeRepository.ByBillingNumber(demandNotice.Select(a => a.BillingNumber).ToArray());
                DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(demandNotice.Select(x => x.TaxpayerId).ToArray());
                // get amount paid
                var payments = await _paymentRepository.ByBillingNumbers(demandNotice.Select(x => x.BillingNumber).ToArray());

                var itemAmount = items.Where(a => a.BillingNo == tm.BillingNumber);//.Sum(s => s.ItemAmount);
                var arrears = previousArrears.Where(a => a.TaxpayerId == tm.TaxpayerId);//.Sum(a => a.TotalAmount);
                var amountPaid = payments.Where(a => a.BillingNumber == tm.BillingNumber);//.Sum(q => q.Amount);

                decimal amount = (itemAmount.Sum(s => s.ItemAmount) + arrears.Sum(a => a.TotalAmount))
                    - amountPaid.Sum(q => q.Amount);
                if (amount > 0)
                {
                    newArrears.Add(new DemandNoticeArrearsModel()
                    {
                        AmountPaid = 0,
                        ArrearsStatus = "PENDING",
                        BillingNo = tm.BillingNumber,
                        BillingYear = DateTime.Now.Year,
                        CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Id = Guid.NewGuid(),
                        OriginatedYear = tm.BillingYr,
                        TaxpayerId = tm.TaxpayerId,
                        WardName = tm.WardName,
                        TotalAmount = amount,
                        CurrentAmount = 0
                    });
                }


                var e = demandNotice.Where(a => a.Id != tm.Id).ToList();
                bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
                if (result)
                {
                    await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
                    await _dNTaxpayersRep.UpdateArrearsStatus(demandNotice.ToArray(), true);
                    if (e.Count > 0)
                        await _dNTaxpayersRep.UpdateDemandNoticeStatus(e.Select(x => x.Id).ToArray(), Common.Utilities.DemandNoticeStatus.CLOSED);
                }
            }

            return true;
        }

        public async Task<bool> RunTaxpayerArrears(DemandNoticeTaxpayersModel[] model, DemandNoticeRequestModel req)
        {
            string[] payableStatus = { "PENDING", "PART_PAYMENT" };
            if (model.Length <= 0)
                return true;

            var demandNotice = model.Where(x => payableStatus.Any(t => t == x.DemandNoticeStatus) && !x.IsRunArrears).ToArray();
            if (demandNotice.Length <= 0) return false;
            // get items
            var items = await _demandNoticeRepository.ByBillingNumber(demandNotice.Select(a => a.BillingNumber).ToArray());
            // get arrears
            DemandNoticeArrearsModel[] previousArrears = await _arrearsRepo.ByTaxpayer(demandNotice.Select(x => x.TaxpayerId).ToArray());
            // get amount paid
            var payments = await _paymentRepository.ByBillingNumbers(model.Select(x => x.BillingNumber).ToArray());
            // penalties
            DemandNoticePenaltyModel[] previousPenalty = await _penaltyRepo.ByTaxpayerId(model.Select(x => x.TaxpayerId).ToArray());

            List<DemandNoticeArrearsModel> newArrears = new List<DemandNoticeArrearsModel>();
            List<DemandNoticePenaltyModel> newPenalty = new List<DemandNoticePenaltyModel>();
            foreach (var tm in model)
            {
                var itemAmount = items.Where(a => a.BillingNo == tm.BillingNumber);//.Sum(s => s.ItemAmount);
                var arrears = previousArrears.Where(a => a.TaxpayerId == tm.TaxpayerId);//.Sum(a => a.TotalAmount);
                var penalty = previousPenalty.Where(a => a.TaxpayerId == tm.TaxpayerId);//.Sum(s => s.TotalAmount);
                var amountPaid = payments.Where(a => a.BillingNumber == tm.BillingNumber);//.Sum(q => q.Amount);

                decimal amount = (itemAmount.Sum(s => s.ItemAmount) + arrears.Sum(a => a.TotalAmount) + penalty.Sum(a => a.TotalAmount))
                    - amountPaid.Sum(q => q.Amount);

                if (amount > 0)
                {
                    if (req.RunArrears)
                        newArrears.Add(new DemandNoticeArrearsModel()
                        {
                            AmountPaid = 0,
                            ArrearsStatus = "PENDING",
                            BillingNo = tm.BillingNumber,
                            BillingYear = DateTime.Now.Year,
                            CreatedBy = _httpAccessor.HttpContext.User.Identity.Name,
                            DateCreated = DateTime.Now,
                            Id = Guid.NewGuid(),
                            OriginatedYear = tm.BillingYr,
                            TaxpayerId = tm.TaxpayerId,
                            WardName = tm.WardName,
                            TotalAmount = amount,
                            CurrentAmount = 0
                        });
                    if (req.RunPenalty)
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
            }

            //close arrears
            if (newArrears.Count > 0)
            {
                bool result = await _arrearsRepo.AddArrears(newArrears.ToArray());
                if (result)
                {
                    await _arrearsRepo.UpdateArrearsStatus(previousArrears, "CLOSED");
                    await _dNTaxpayersRep.UpdateArrearsStatus(demandNotice, true);
                    await _dNTaxpayersRep.UpdateDemandNoticeStatus(demandNotice.Select(x => x.Id).ToArray(), Common.Utilities.DemandNoticeStatus.CLOSED);
                }
            }

            if (newPenalty.Count > 0)
            {
                bool result1 = await _penaltyRepo.CreatePenalty(newPenalty.ToArray());
                if (result1)
                {
                    await _penaltyRepo.UpdatePenaltyStatus(previousPenalty, "CLOSED");
                    await _dNTaxpayersRep.UpdatePenaltyStatus(demandNotice, true);
                }

            }
            return true;
        }
    }
}
