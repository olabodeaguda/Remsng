using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class SyncRepository : ISyncRepository
    {
        private readonly DbContext db;
        public SyncRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<SyncDataModel>> Get()
        {
            return await db.Set<SyncDataModel>()
                .FromSql("sp_paymentReportSync").ToListAsync();
        }

        public async Task<List<SyncDataModel>> GetApprovalUpdate()
        {
            return await db.Set<SyncDataModel>()
                .FromSql("sp_paymentUpdateSync").ToListAsync();
        }

        public async Task<bool> UpdateSyncStatus(Guid[] ids)
        {
            string prms = "";
            for (int i = 0; i < ids.Length; i++)
            {
                prms = prms + $"'{ids[i]}'";
                if (i <= (ids.Length - 2))
                {
                    prms = prms + ",";
                }
            }
            string query = $"update tbl_demandNoticePaymentHistory set syncStatus = 1 where id in({prms})";
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePaymentStatus(List<SyncDataModel> approveBills)
        {
            string query = "";
            foreach (var tm in approveBills)
            {
                query = query + $"update tbl_demandNoticePaymentHistory set paymentStatus = '{tm.paymentStatus}'," +
                    $" lastmodifiedby= '{tm.lastmodifiedby}', lastModifiedDate = '{tm.lastModifiedDate}' where id = '{tm.Id}'; ";
            }

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        private string selectPayment()
        {
            return $"update top(100) tbl_demandNoticePaymentHistory set syncStatus = 1  output inserted.* where paymentStatus='COMPLETED' ";
        }
    }
}
