﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;

namespace RemsNG.Dao
{
    public class ReportDao : AbstractDao
    {
        public ReportDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await db.ItemReportSummaryModels.FromSql("sp_paymentSummaryByItems @p0,@p1",
                new object[]
                {
                    startDate,endDate
                }).ToListAsync();
        }
    }
}
