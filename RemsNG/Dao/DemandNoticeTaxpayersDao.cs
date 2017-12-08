﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using RemsNG.Utilities;
using System.Text;

namespace RemsNG.Dao
{
    public class DemandNoticeTaxpayersDao : AbstractDao
    {
        public DemandNoticeTaxpayersDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> getTaxpayerByIds(string[] ids, int billingYr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string tIds = stringBuilder.AppendJoin(',', ids).ToString();
           
            string query = $"select * from tbl_demandNoticeTaxpayers where taxpayerId in ({tIds}) and billingYr = {billingYr}";
            return await db.Set<DemandNoticeTaxpayersDetail>().FromSql(query).ToListAsync();
        }

        public async Task<Response> Add(DemandNoticeTaxpayersDetail demandNoticeTaxpayersDetails)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNoticeTaxpayer @p0,@p1,@p2,@p3", new object[]{
                demandNoticeTaxpayersDetails.dnId,
                demandNoticeTaxpayersDetails.billingYr,
                demandNoticeTaxpayersDetails.createdBy,
                demandNoticeTaxpayersDetails.taxpayerId
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = dbResponse.msg,
                    description = "Taxpayer has been successfully added"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

    }
}
