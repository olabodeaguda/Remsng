﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
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
    public class ReportManager : IReportManager
    {
        private readonly IDemandNoticePenaltyRepository dnPenaltyDao;
        private readonly IDemandNoticeArrearRepository dnArrearsDao;
        private readonly IDemandNoticeItemRepository dnitemDao;
        private readonly IReportRepository reportDao;
        public ReportManager(IDemandNoticePenaltyRepository demandNoticePenaltyRepository,
            IDemandNoticeArrearRepository demandNoticeArrearRepository,
            IDemandNoticeItemRepository demandNoticeItemRepository,
            IReportRepository reportRepository)
        {
            reportDao = reportRepository;
            dnitemDao = demandNoticeItemRepository;
            dnArrearsDao = demandNoticeArrearRepository;
            dnPenaltyDao = demandNoticePenaltyRepository;
        }

        public async Task<(long[] billNumbers, Guid[] taxpayerIds)> AllIdsByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.AllIdsByDate(startDate, endDate);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate(startDate, endDate);
        }

        public async Task<List<ChartReportModel>> ReportByCurrentYear()
        {
            return await reportDao.ReportByYear();
        }

        public async Task<List<DemandNoticeItemModel>> ReportitemsByCategory(DateTime startDate, DateTime endDate)
        {
            var ids = await reportDao.AllIdsByDate(startDate, endDate);
            if (ids.billNumbers.Length <= 0)
                throw new Exception("Empty Record");

            return await dnitemDao.ReportByCategory(ids.billNumbers);
        }

        public async Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnArrearsDao.ReportByCategory(startDate, endDate);
        }

        public async Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnPenaltyDao.ReportByCategory(startDate, endDate);
        }

        public async Task<List<DemandNoticeItemModel>> ReportitemsByCategory(long[] billnumbers)
        {
            return await dnitemDao.ReportByCategory(billnumbers);
        }

        public async Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(Guid[] taxpayerIds)
        {
            return await dnArrearsDao.ReportByCategory(taxpayerIds);
        }

        public async Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(Guid[] taxpayerIds)
        {
            return await dnPenaltyDao.ReportByCategory(taxpayerIds);
        }
    }
}
