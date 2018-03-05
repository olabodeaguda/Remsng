using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Models;
using RemsNG.ORM;

namespace RemsNG.Dao
{
    public class SyncDao : AbstractDao
    {
        public SyncDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<SyncDataModel>> Get() =>
             await db.SyncDataModels.FromSql("sp_paymentReportSync").ToListAsync();

        public async Task<List<SyncDataModel>> GetApprovalUpdate() =>
            await db.SyncDataModels.FromSql("sp_paymentUpdateSync").ToListAsync();

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
                    $" lastmodifiedby= '{tm.lastmodifiedby}', lastModifiedDate = '{tm.lastModifiedDate}' where id '{tm.Id}'; ";
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
