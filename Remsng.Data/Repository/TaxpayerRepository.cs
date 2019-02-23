using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class TaxpayerRepository : AbstractRepository
    {
        public TaxpayerRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Create(TaxPayerModel taxpayer, bool confirmCompany)
        {
            var r = await Get(taxpayer.StreetId.Value, taxpayer.CompanyId);

            if (r != null && confirmCompany)
            {
                throw new DuplicateCompanyException("Company already exist on the street");
            }
            if (r != null)
            {
                if (taxpayer.Lastname == r.Lastname &&
                    taxpayer.Surname == r.Surname
                    && taxpayer.Firstname == r.Firstname)
                {
                    throw new DuplicateCompanyException("Taxpayer already exist");
                }
            }

            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", new object[] {
                    taxpayer.Id,
                    taxpayer.CompanyId,
                    taxpayer.StreetId,
                    taxpayer.AddressId == null?Guid.Empty:taxpayer.AddressId,
                    taxpayer.CreatedBy,
                    taxpayer.Surname,
                    taxpayer.Firstname,
                    taxpayer.Lastname
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Tax Payer has been added successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur when creating the taxpayer. Please try again or inform your admnistrator for assitance"
                };
            }
        }

        public async Task<TaxPayerModel> Get(Guid streetId, Guid companyId)
        {
            //string query = $"select tbl_taxPayer.*,'-1' as streetNumber from tbl_taxPayer" +
            //    $" where streetId = '{streetId}' and companyId='{companyId}'";
            var result = await db.Set<TaxPayer>()
                .FirstOrDefaultAsync(x => x.StreetId == streetId && x.CompanyId == companyId);
            //.FromSql(query).FirstOrDefaultAsync();

            return new TaxPayerModel()
            {
                AddressId = result.AddressId,
                CompanyId = result.CompanyId,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Firstname = result.Firstname,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                StreetId = result.StreetId,
                Surname = result.Surname,
                TaxpayerStatus = result.TaxpayerStatus
            };
        }

        public async Task<List<TaxPayerModel>> GetActiveTaxpayers(DemandNoticeRequestModel demandNoticeRequest)
        {
            List<TaxPayer> tx = new List<TaxPayer>();
            if (demandNoticeRequest.streetId != Guid.Empty && demandNoticeRequest.streetId != null)
            {
                tx = await db.Set<TaxPayer>()
                    .FromSql($"select tbl_taxPayer.* from tbl_taxPayer " +
                    $"where streetId = '{demandNoticeRequest.streetId}' and taxpayerStatus='ACTIVE'").ToListAsync();
            }
            else if (demandNoticeRequest.wardId != Guid.Empty && demandNoticeRequest.wardId != null)
            {
                string query = $"select tbl_taxPayer.* from tbl_taxPayer " +
                    $"inner join tbl_street on tbl_taxPayer.streetId = tbl_street.id where tbl_street.wardId = '{demandNoticeRequest.wardId}' and taxpayerStatus='ACTIVE'";
                tx = await db.Set<TaxPayer>()
                    .FromSql(query).ToListAsync();
            }

            return tx.Select(result => new TaxPayerModel()
            {
                AddressId = result.AddressId,
                CompanyId = result.CompanyId,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Firstname = result.Firstname,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                StreetId = result.StreetId,
                Surname = result.Surname,
                TaxpayerStatus = result.TaxpayerStatus
            }).ToList();
        }

        public async Task<TaxPayerModel> ById(Guid id)
        {
            var result = await db.Set<TaxPayer>().FirstOrDefaultAsync(x => x.Id == id);//.FromSql("sp_TaxpayerById @p0", new object[] { id }).FirstOrDefaultAsync();
            if (result == null || result.TaxpayerStatus == TaxPayerEnum.DELETED.ToString())
            {
                return null;
            }

            return new TaxPayerModel()
            {
                AddressId = result.AddressId,
                CompanyId = result.CompanyId,
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Firstname = result.Firstname,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                StreetId = result.StreetId,
                Surname = result.Surname,
                TaxpayerStatus = result.TaxpayerStatus
            };
        }

        public async Task<List<TaxPayerModel>> ByStreetId(Guid streetId)
        {
            var result = await db.Set<TaxPayer>()
                .Where(x => x.StreetId == streetId
                && x.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString())
                .Select(p => new TaxPayerModel()
                {
                    AddressId = p.AddressId,
                    CompanyId = p.CompanyId,
                    CreatedBy = p.CreatedBy,
                    DateCreated = p.DateCreated,
                    Firstname = p.Firstname,
                    Id = p.Id,
                    Lastmodifiedby = p.Lastmodifiedby,
                    LastModifiedDate = p.LastModifiedDate,
                    Lastname = p.Lastname,
                    StreetId = p.StreetId,
                    Surname = p.Surname,
                    TaxpayerStatus = p.TaxpayerStatus
                }).ToListAsync();//.FromSql("sp_TaxpayerByStreetId @p0", new object[] { streetId }).ToListAsync();
            //result = result.Where(x => x.taxpayerStatus == TaxPayerEnum.ACTIVE.ToString()).ToList();
            return result;
        }

        public async Task<object> ByStreetId(Guid streetId, PageModel pageModel)
        {
            var query = db.Set<TaxPayer>()
               .Include(x => x.Company)
               .Include(x => x.Address)
               .Include(p => p.Street)
               .Include(q => q.Street.Ward)
               .Where(p => p.StreetId == streetId && p.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString())
               .Select(x => new TaxPayerModel()
               {
                   AddressId = x.AddressId,
                   CompanyId = x.CompanyId,
                   StreetId = x.StreetId,
                   companyName = x.Company.CompanyName,
                   CreatedBy = x.CreatedBy,
                   DateCreated = x.DateCreated.Value,
                   Firstname = x.Firstname,
                   Id = x.Id,
                   Lastmodifiedby = x.Lastmodifiedby,
                   LastModifiedDate = x.LastModifiedDate,
                   Lastname = x.Lastname,
                   StreetNumber = x.Address.Addressnumber,
                   Surname = x.Surname,
                   TaxpayerStatus = x.TaxpayerStatus,
                   WardName = x.Street.Ward.WardName,
                   StreetName = x.Street.StreetName
               });

            var results = await query.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();
            int totalCount = await query.CountAsync();

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<TaxPayerModel>> ByCompanyId(Guid companyId)
        {
            var res = await db.Set<TaxPayer>()
                .Where(x => x.CompanyId == companyId && x.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString())
                .Select(p => new TaxPayerModel
                {
                    AddressId = p.AddressId,
                    CompanyId = p.CompanyId,
                    CreatedBy = p.CreatedBy,
                    DateCreated = p.DateCreated,
                    Firstname = p.Firstname,
                    Id = p.Id,
                    Lastmodifiedby = p.Lastmodifiedby,
                    LastModifiedDate = p.LastModifiedDate,
                    Lastname = p.Lastname,
                    StreetId = p.StreetId,
                    Surname = p.Surname,
                    TaxpayerStatus = p.TaxpayerStatus
                }).ToListAsync();

            //var results = await db.Set<TaxpayerExtensionModel>().FromSql("sp_TaxpayerByCompanyId @p0", new object[] { companyId }).ToListAsync();
            //results = results.Where(x => x.taxpayerStatus == TaxPayerEnum.ACTIVE.ToString()).ToList();
            return res;
        }

        public async Task<Response> Update(TaxPayerModel taxpayer)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_updateTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", new object[] {
            //        taxpayer.Id,
            //        taxpayer.CompanyId,
            //        taxpayer.StreetId,
            //        taxpayer.AddressId,
            //        taxpayer.Lastmodifiedby,
            //        taxpayer.Surname,
            //        taxpayer.Firstname,
            //        taxpayer.Lastname
            //}).FirstOrDefaultAsync();

            var entity = await db.Set<TaxPayer>().FindAsync(taxpayer.Id);
            if (entity == null)
            {
                throw new NotFoundException($"{taxpayer.Surname} {taxpayer.Firstname} {taxpayer.Lastname} deos not exist");
            }

            entity.CompanyId = taxpayer.CompanyId;
            entity.StreetId = taxpayer.StreetId;
            entity.AddressId = taxpayer.AddressId;
            entity.Lastmodifiedby = taxpayer.Lastmodifiedby;
            entity.Surname = taxpayer.Surname;
            entity.Firstname = taxpayer.Firstname;
            entity.Lastname = taxpayer.Lastname;

            Response response = new Response();
            response.description = "Update was successful";
            response.code = MsgCode_Enum.SUCCESS;

            return response;
        }

        public async Task<List<TaxPayerModel>> ByLcdaId(Guid lcdaId)
        {
            return await db.Set<TaxPayer>()
                .Include(x => x.Company)
                .Include(x => x.Address)
                .Where(x => x.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString()).Select(x => new TaxPayerModel()
                {
                    AddressId = x.AddressId.Value,
                    CompanyId = x.CompanyId,
                    companyName = x.Company.CompanyName,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Firstname = x.Firstname,
                    Id = x.Id,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    Lastname = x.Lastname,
                    StreetId = x.StreetId.Value,
                    StreetNumber = x.Address.Addressnumber
                }).ToListAsync();
        }

        public async Task<List<TaxPayerModel>> Search(Guid lcdaId, string qu)
        {
            string[] q = qu.Split(new char[] { ' ' });

            //string query = $"select tbl_taxPayer.*,tbl_company.companyName as companyName, tbl_Address.addressnumber as streetNumber,-1 as totalSize  from tbl_taxPayer " +
            //   $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
            //   $"inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId " +
            //   $"inner join tbl_ward on tbl_ward.id = tbl_street.wardId " +
            //   $"inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id " +
            //   $"where tbl_ward.lcdaId = '{lcdaId}' " +
            //   $"and (tbl_taxPayer.surname like '%{qu}%' or tbl_taxPayer.firstname like '%{qu}%' " +
            //   $"or tbl_taxPayer.lastname like '%{qu}%' " +
            //   $" or tbl_company.companyName like '%{qu}%')";


            var query = db.Set<TaxPayer>()
                .Include(x => x.Company)
                .Include(x => x.Street)
                .ThenInclude(x => x.Ward)
                .Where(x => x.Company.LcdaId == lcdaId);


            string subQuery = string.Empty;

            for (int i = 0; i < q.Length; i++)
            {
                if (string.IsNullOrEmpty(q[i].Trim()))
                {
                    continue;
                }
                //if (query != string.Empty && i < q.Length)
                //{
                //    query = query + $" union ";
                //}
                //string t = $" (tbl_taxPayer.surname like '%{q[i]}%' or tbl_taxPayer.firstname like '%{q[i]}%' " +
                //$"or tbl_taxPayer.lastname like '%{q[i]}%' " +
                //$" or tbl_company.companyName like '%{q[i]}%')";

                //if (!string.IsNullOrEmpty(subQuery))
                //{
                //    subQuery = subQuery + " and ";
                //}
                //subQuery = subQuery + t;
                query.Where(x => EF.Functions.Like(x.Surname, $"%{q[i]}%") || EF.Functions.Like(x.Firstname, $"%{q[i]}%")
                || EF.Functions.Like(x.Lastname, $"%{q[i]}%") || EF.Functions.Like(x.Company.CompanyName, $"%{q[i]}%"));
            }

            //query = query + (string.IsNullOrEmpty(subQuery) ? "" : " or ") + (string.IsNullOrEmpty(subQuery) ? "" : $" ( {subQuery} ) ");

            var results = await query.Select(x => new TaxPayerModel
            {
                AddressId = x.AddressId.Value,
                CompanyId = x.CompanyId,
                companyName = x.Company.CompanyName,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Firstname = x.Firstname,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                Lastname = x.Lastname,
                StreetId = x.StreetId.Value,
                StreetNumber = x.Address.Addressnumber
            }).ToListAsync();

            return results.Distinct().Where(x => x.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString()).OrderBy(x => x.Firstname).ToList();

        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var query = db.Set<TaxPayer>()
                .Include(x => x.Company)
                .Include(x => x.Address)
                .Where(x => x.TaxpayerStatus == TaxPayerEnum.ACTIVE.ToString()).Select(x => new TaxPayerModel()
                {
                    AddressId = x.AddressId.Value,
                    CompanyId = x.CompanyId,
                    companyName = x.Company.CompanyName,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Firstname = x.Firstname,
                    Id = x.Id,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    Lastname = x.Lastname,
                    StreetId = x.StreetId.Value,
                    StreetNumber = x.Address.Addressnumber,
                    Surname = x.Surname,
                    TaxpayerStatus = x.TaxpayerStatus
                });

            var results = query.OrderBy(x => x.Surname).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize); // await db.Set<TaxpayerExtensionModel>().FromSql("sp_TaxpayerByLcdaIdpaginated @p0,@p1,@p2", new object[] { lcdaId, pageModel.PageSize, pageModel.PageNum }).ToListAsync();
            int totalCount = await query.CountAsync();

            return new
            {
                data = results.ToList(),
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<LcdaModel> getLcda(Guid taxpayerId)
        {
            string query = $"select distinct ld.* from tbl_lcda as ld ";
            query = query + $"inner join tbl_ward as wd on wd.lcdaId = ld.id ";
            query = query + $"inner join tbl_street as st on st.wardId = wd.id ";
            query = query + $"inner join tbl_taxPayer as tp on tp.streetId = st.id ";
            query = query + $"where tp.id= '{taxpayerId}'";
            var result = await db.Set<Lcda>().FromSql(query).FirstOrDefaultAsync();
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

        public async Task<List<DemandNoticePaymentHistoryModel>> PaymentHistory(Guid taxpayerId)
        {
            //string query1 = $"select ndh.*,bank.bankName  from tbl_demandNoticePaymentHistory as ndh " +
            //    $"inner join tbl_bank bank on bank.id = ndh.bankId " +
            //    $"inner join tbl_demandNoticeTaxpayers as dnt on dnt.billingNumber = ndh.billingNumber " +
            //    $"where ndh.paymentStatus = 'APPROVED' and  dnt.taxpayerId = '{taxpayerId}' order by =>dnt.BillingNumber,)


            var res = await db.Set<DemandNoticePaymentHistory>()
            .Include(p => p.Bank)
            .Join(db.Set<DemandNoticeTaxpayers>(), dnph => dnph.BillingNumber, dnt => dnt.BillingNumber, (p, dnt) =>
                  new DemandNoticePaymentHistoryModel()
                  {
                      Amount = p.Amount,
                      BankId = p.BankId,
                      BillingNumber = p.BillingNumber,
                      Charges = p.Charges,
                      CreatedBy = p.CreatedBy,
                      DateCreated = p.DateCreated,
                      Id = p.Id,
                      IsWaiver = p.IsWaiver,
                      Lastmodifiedby = p.Lastmodifiedby,
                      LastModifiedDate = p.LastModifiedDate,
                      OwnerId = p.OwnerId,
                      PaymentMode = p.PaymentMode,
                      PaymentStatus = p.PaymentStatus,
                      ReferenceNumber = p.ReferenceNumber,
                      SyncStatus = p.SyncStatus,
                      BankName = p.Bank.BankName
                  })
            .Where(p => p.PaymentStatus == "APPROVED" && p.OwnerId == taxpayerId).ToListAsync();

            //.FromSql(query1).ToListAsync();

            return res;
        }

        public async Task<int> UpdateStatus(Guid id, string status)
        {
            string query = $"update tbl_taxPayer set taxpayerStatus = '{status}' where id = '{id}'";
            return await db.Database.ExecuteSqlCommandAsync(query);
        }

        public async Task<List<TaxPayerModel>> SearchInStreet(Guid streetid, string queryParams)
        {
            string[] str = queryParams.Split(new char[] { ' ' });
            string query = string.Empty;

            List<TaxPayer> lst = new List<TaxPayer>();
            var qry = db.Set<TaxPayer>()
               .Include(x => x.Company)
               .Include(x => x.Address)
               .Include(p => p.Street)
               .Include(q => q.Street.Ward);

            for (int i = 0; i < str.Length; i++)
            {
                var ru = await qry.Where(x => (EF.Functions.Like(x.Firstname, $"%{str[i]}%") ||
                  EF.Functions.Like(x.Lastmodifiedby, $"%{str[i]}%") ||
                  EF.Functions.Like(x.Surname, $"%{str[i]}%")) && x.StreetId == streetid).ToArrayAsync();
                if (ru.Length > 0)
                {
                    lst.AddRange(ru);
                }

            }

            var results = lst.Select(x => new TaxPayerModel()
            {
                AddressId = x.AddressId,
                CompanyId = x.CompanyId,
                StreetId = x.StreetId,
                companyName = x.Company.CompanyName,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated.Value,
                Firstname = x.Firstname,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                Lastname = x.Lastname,
                StreetNumber = x.Address.Addressnumber,
                Surname = x.Surname,
                TaxpayerStatus = x.TaxpayerStatus,
                WardName = x.Street.Ward.WardName
            });

            var re = results
                .GroupBy(x => x.Id)
                .Select(p => p.FirstOrDefault())
                .ToList();

            return re;
        }

        public async Task<TaxPayerModel[]> GetUnbilledTaxpayer(int billingYear)
        {
            //string query = $"select tbl_taxPayer.*,tbl_company.companyName, tbl_street.streetName, tbl_address.addressnumber, tbl_ward.wardName from tbl_taxPayer " +
            //    $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
            //    $"inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId " +
            //    $"inner join tbl_ward on tbl_ward.id = tbl_street.wardId " +
            //    $"inner join tbl_address on tbl_address.id = tbl_taxPayer.addressId " +
            //    $"where tbl_taxPayer.id  " +
            //    $"not in(select taxpayerId from tbl_demandNoticeTaxpayers where " +
            //    $"billingYr= {billingYear} and tbl_demandNoticeTaxpayers.demandNoticeStatus <> 'CANCEL') " +
            //    $"and tbl_taxPayer.taxpayerStatus = 'ACTIVE' ";

            var alreadyBilledTaxpayerIds = await db.Set<DemandNoticeTaxpayers>()
                .Where(x => x.BillingYr == billingYear && x.DemandNoticeStatus != "CANCEL")
                .Select(x => x.TaxpayerId).ToArrayAsync();

            var result = await db.Set<TaxPayer>()
               .Include(x => x.Company)
               .Include(x => x.Address)
               .Include(p => p.Street)
               .Include(q => q.Street.Ward)
               .Where(p => p.TaxpayerStatus == "ACTIVE" && alreadyBilledTaxpayerIds.Any(x => x != p.Id))
               .Select(x => new TaxPayerModel()
               {
                   AddressId = x.AddressId,
                   CompanyId = x.CompanyId,
                   StreetId = x.StreetId,
                   companyName = x.Company.CompanyName,
                   CreatedBy = x.CreatedBy,
                   DateCreated = x.DateCreated.Value,
                   Firstname = x.Firstname,
                   Id = x.Id,
                   Lastmodifiedby = x.Lastmodifiedby,
                   LastModifiedDate = x.LastModifiedDate,
                   Lastname = x.Lastname,
                   StreetNumber = x.Address.Addressnumber,
                   Surname = x.Surname,
                   TaxpayerStatus = x.TaxpayerStatus,
                   WardName = x.Street.Ward.WardName,
                   StreetName = x.Street.StreetName
               }).ToArrayAsync();

            return result;
        }
    }
}
