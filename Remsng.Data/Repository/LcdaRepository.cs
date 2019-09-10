using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class LcdaRepository : ILcdaRepository
    {
        private readonly DbContext db;
        private readonly ILogger logger;
        public LcdaRepository(DbContext _db, ILoggerFactory loggerFactory)
        {
            db = _db;
            logger = loggerFactory.CreateLogger("LCDA Dao");
        }

        public async Task<LcdaModel> Get(Guid id)
        {
            var result = await db.Set<Lcda>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return null;
            }
            return new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            };
        }

        public async Task<List<LcdaModel>> All()
        {
            var r = await db.Set<Lcda>()
                .OrderBy(x => x.LcdaName).ToListAsync();
            return r.Select(result => new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            }).ToList();
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Set<Lcda>()
                .Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Set<Lcda>()
                .Count();
                return new
                {
                    data = results.Select(result => new LcdaModel()
                    {
                        AddressId = result.AddressId,
                        Charges = result.Charges,
                        CreatedBy = result.CreatedBy,
                        DateCreated = result.DateCreated,
                        DomainId = result.DomainId,
                        Id = result.Id,
                        Lastmodifiedby = result.Lastmodifiedby,
                        LastModifiedDate = result.LastModifiedDate,
                        LcdaCode = result.LcdaCode,
                        LcdaName = result.LcdaName,
                        LcdaStatus = result.LcdaStatus
                    }).ToList(),
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<List<LcdaModel>> ActiveLCDAByDomainId(Guid domainId)
        {
            var f = await db.Set<Lcda>()
                .Where(x => x.DomainId == domainId && x.LcdaStatus == UserStatus.ACTIVE.ToString()).ToListAsync();

            return f.Select(result => new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            }).ToList();
        }

        public async Task<UserLcdaModel> UserLcdaByIds(Guid lgdaId, Guid userId)
        {
            var result = await db.Set<UserLcda>()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LgdaId == lgdaId);
            if (result == null)
            {
                return null;
            }
            return new UserLcdaModel()
            {
                LgdaId = result.LgdaId,
                UserId = result.UserId
            };
        }

        public async Task<bool> Add(LcdaModel lcda)
        {
            db.Set<Lcda>()
                .Add(new Lcda
                {
                    AddressId = lcda.AddressId,
                    Charges = lcda.Charges,
                    CreatedBy = lcda.CreatedBy,
                    DateCreated = lcda.DateCreated,
                    DomainId = lcda.DomainId,
                    Id = lcda.Id,
                    Lastmodifiedby = lcda.Lastmodifiedby,
                    LastModifiedDate = lcda.LastModifiedDate,
                    LcdaCode = lcda.LcdaCode,
                    LcdaName = lcda.LcdaName,
                    LcdaStatus = lcda.LcdaStatus
                });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Update(LcdaModel lcda)
        {
            var oldlcda = await db.Set<Lcda>()
                .FirstOrDefaultAsync(x => x.Id == lcda.Id);
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
            var oldlcda = await db.Set<Lcda>()
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<LcdaModel> byLcdaCode(string lcdaCode)
        {
            var result = await db.Set<Lcda>().FirstOrDefaultAsync(x => x.LcdaCode.ToLower() == lcdaCode.ToLower());
            if (result == null)
            {
                return null;
            }
            return new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            };
        }

        public async Task<List<LcdaModel>> getLcdaByUsername(string username)
        {
            var r = await db.Set<Lcda>().FromSql("sp_getUserLCDAByUsername @p0", new object[] { username }).ToListAsync();
            return r.Select(result => new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            }).ToList();
        }

        public async Task<List<LcdaModel>> UserDomainByUserId(Guid id)
        {
            var r = await db.Set<Lcda>().FromSql("sp_getUserLCDAByuserId @p0", new object[] { id }).ToListAsync();
            return r.Select(result => new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            }).ToList();
        }

        public async Task<List<LcdaModel>> unAssignUserDomainByUserId(Guid userid)
        {
            var r = await db.Set<Lcda>()
                .FromSql("sp_unAssignUserDomainByuserId @p0", new object[] { userid }).ToListAsync();
            return r.Select(result => new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            }).ToList();
        }

        public async Task<bool> RemoveUserFromLCDA(UserLcdaModel userLcda)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_removeUserFromLCDA @p0, @p1",
                new object[] { userLcda.UserId, userLcda.LgdaId }).FirstOrDefaultAsync();
            if (dbResponse.success)
            {
                return true;
            }

            logger.LogError(dbResponse.msg, userLcda); //new object[] { userLcda.userId, userLcda.lgdaId });
            return false;
        }

        public async Task<LcdaModel> ByStreet(Guid streetId)
        {
            string query = $"select distinct tbl_lcda.* from tbl_lcda " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id  " +
                $"inner join tbl_street on tbl_street.wardId = tbl_ward.id where tbl_street.id= '{streetId}'";
            var result = await db.Set<Lcda>()
                .FromSql(query).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            };
        }

        public async Task<DomainModel> GetDomain(Guid lcdaId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $" where tbl_lcda.id = '{lcdaId}'";

            var r = await db.Set<Domain>()
                .FromSql(query).FirstOrDefaultAsync();
            if (r == null)
            {
                return null;
            }
            return new DomainModel()
            {
                AddressId = r.AddressId,
                Datecreated = r.Datecreated,
                DomainCode = r.DomainCode,
                DomainName = r.DomainName,
                DomainStatus = r.DomainStatus,
                DomainType = r.DomainType,
                Id = r.Id,
                StateId = r.StateId
            };
        }

        public async Task<LcdaModel> GetLcdaExtension(Guid lcdaId)
        {
            var result = await db.Set<Lcda>()
                .FromSql($"select * from tbl_lcda where id = '{lcdaId}'").FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            };
        }

        public async Task<LcdaModel> ByBillingNumber(long billingno)
        {
            string query = $"select top 1 lc.* from tbl_demandNoticeTaxpayers as dnt ";
            query = query + $"inner join tbl_demandnotice as dn on dn.id = dnt.dnId ";
            query = query + $"inner join tbl_lcda as lc on lc.id = dn.lcdaId ";
            query = query + $"where dnt.billingNumber = '{billingno.ToString()}'";

            var result = await db.Set<Lcda>()
                .FromSql(query).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return new LcdaModel()
            {
                AddressId = result.AddressId,
                Charges = result.Charges,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                DomainId = result.DomainId,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                LcdaCode = result.LcdaCode,
                LcdaName = result.LcdaName,
                LcdaStatus = result.LcdaStatus
            };
        }
    }
}
