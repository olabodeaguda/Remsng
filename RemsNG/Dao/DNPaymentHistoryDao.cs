using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class DNPaymentHistoryDao : AbstractDao
    {
        public DNPaymentHistoryDao(RemsDbContext _db) : base(_db)
        {}

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<DemandNoticePaymentHistoryExt>().FromSql("sp_paymenthistoryByLcda @p0,@p1,@p2", new object[] { lcdaId, pageModel.PageSize, pageModel.PageNum }).ToListAsync();
            int totalCount = 0;
            if (results.Count > 0)
            {
                totalCount = results[0].totalSize;
            }

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }
    }
}
