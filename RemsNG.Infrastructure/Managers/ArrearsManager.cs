﻿using Microsoft.AspNetCore.Http;
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

        public async Task<bool> RunTaxpayerArrears(Guid[] dnTaxpayerIds)
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
                    ItemId = default(Guid),
                    OriginatedYear = ndTaxpayer.BillingYr,
                    TaxpayerId = ndTaxpayer.TaxpayerId,
                    WardName = ndTaxpayer.WardName,
                    TotalAmount = currAmount + pArrears,
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

    }
}