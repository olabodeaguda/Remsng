using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class LcdaRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public LcdaRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("LCDA Dao");
        }

        public async Task<Lcda> Get(Guid id)
        {
            return await db.lgdas.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<object> All()
        {
            return await db.lgdas.OrderBy(x => x.LcdaName).ToListAsync();
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.lgdas.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.lgdas.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<List<Lcda>> ActiveLCDAByDomainId(Guid domainId)
        {
            return await db.lgdas.Where(x => x.DomainId == domainId && x.LcdaStatus == UserStatus.ACTIVE.ToString()).ToListAsync();

        }

        public async Task<UserLcda> UserLcdaByIds(Guid lgdaId, Guid userId)
        {
            return await db.UserLcdas.FirstOrDefaultAsync(x => x.UserId == userId && x.LgdaId == lgdaId);
        }

        public async Task<bool> Add(Lcda lcda)
        {
            db.lgdas.Add(lcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Lcda lcda)
        {
            var oldlcda = await db.lgdas.FirstOrDefaultAsync(x => x.Id == lcda.Id);
            if (oldlcda == null)
            {
                throw new NotFoundException($"{lcda.LcdaName} not found");
            }

            oldlcda.LcdaName = lcda.LcdaName;
            oldlcda.LcdaCode = lcda.LcdaCode;
            oldlcda.LastModifiedDate = DateTime.Now;
            oldlcda.Lastmodifiedby = lcda.Lastmodifiedby;
            oldlcda.DomainId = lcda.DomainId;

            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Changetatus(Guid id, string lcdastatus)
        {
            var oldlcda = await db.lgdas.FirstOrDefaultAsync(x => x.Id == id);
            if (oldlcda == null)
            {
                throw new NotFoundException($"selected LCDA does not exist not found");
            }

            oldlcda.LcdaStatus = lcdastatus;
            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<Lcda> byLcdaCode(string lcdaCode)
        {
            return await db.lgdas.FirstOrDefaultAsync(x => x.LcdaCode.ToLower() == lcdaCode.ToLower());
        }

        public async Task<List<Lcda>> getLcdaByUsername(string username)
        {
            return await db.lgdas.FromSql("sp_getUserLCDAByUsername @p0", new object[] { username }).ToListAsync();
        }

        public async Task<bool> AssignUserToLgda(UserLcda userLcda)
        {
            db.UserLcdas.Add(userLcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<Lcda>> UserDomainByUserId(Guid id)
        {
            return await db.lgdas.FromSql("sp_getUserLCDAByuserId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<List<UserLcda>> UserRoleDomainbyUserId(Guid id)
        {
            return await db.UserLcdas.Include("role").Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<List<Lcda>> unAssignUserDomainByUserId(Guid userid)
        {
            return await db.lgdas.FromSql("sp_unAssignUserDomainByuserId @p0", new object[] { userid }).ToListAsync();
        }

        public async Task<bool> RemoveUserFromLCDA(UserLcda userLcda)
        {
            DbResponse dbResponse = await db.DbResponses
                .FromSql("sp_removeUserFromLCDA @p0, @p1",
                new object[] { userLcda.UserId, userLcda.LgdaId }).FirstOrDefaultAsync();
            if (dbResponse.success)
            {
                return true;
            }

            logger.LogError(dbResponse.msg, userLcda); //new object[] { userLcda.userId, userLcda.lgdaId });
            return false;
        }

        public async Task<Lcda> ByStreet(Guid streetId)
        {
            string query = $"select distinct tbl_lcda.* from tbl_lcda " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id  " +
                $"inner join tbl_street on tbl_street.wardId = tbl_ward.id where tbl_street.id= '{streetId}'";
            return await db.lgdas.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<Domain> GetDomain(Guid lcdaId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $" where tbl_lcda.id = '{lcdaId}'";

            return await db.Domains.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<Lcda> GetLcdaExtension(Guid lcdaId)
        {
            return await db.lgdas.FromSql($"select * from tbl_lcda where id = '{lcdaId}'").FirstOrDefaultAsync();
        }

        public async Task<Lcda> ByBillingNumber(String billingno)
        {
            //string query = $"select top 1 lc.* from tbl_demandNoticeTaxpayers as dnt ";
            //query = query + $"inner join tbl_taxPayer as tp on tp.id = dnt.taxpayerId ";
            //query = query + $"inner join tbl_street as st on st.id = tp.streetId ";
            //query = query + $"inner join tbl_ward as wd on wd.id = st.wardId ";
            //query = query + $"inner join tbl_lcda as lc on lc.id = wd.lcdaId ";
            //query = query + $"where dnt.billingNumber = '{billingno}'";

            string query = $"select top 1 lc.* from tbl_demandNoticeTaxpayers as dnt ";
            query = query + $"inner join tbl_demandnotice as dn on dn.id = dnt.dnId ";
            query = query + $"inner join tbl_lcda as lc on lc.id = dn.lcdaId ";
            query = query + $"where dnt.billingNumber = '{billingno}'";

            return await db.lgdas.FromSql(query).FirstOrDefaultAsync();
        }
    }
}
