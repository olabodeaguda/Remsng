using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class AddressRepository : AbstractRepository
    {
        public AddressRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(AddressModel address)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_addAddress @p0,@p1,@p2,@p3,@p4, @p5", new object[] {
                    address.Id,
                    address.Addressnumber,
                    address.StreetId,
                    address.OwnerId,
                    address.Lcdaid,
                    address.CreatedBy
                }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
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

        public async Task<Response> Update(AddressModel address)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_updateAddress @p0,@p1,@p2,@p3,@p4,@p5", new object[] {
                    address.Addressnumber,
                    address.StreetId,
                    address.OwnerId,
                    address.Lcdaid,
                    address.Lastmodifiedby,
                    address.Id
                }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Address has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "An error occur.Please contact administrator for help or try again"
                };
            }
        }

        public async Task<Response> Remove(AddressModel address)
        {
            var results = await db.Addresses.FromSql($"select tbl_address.*,'none' as streetName from tbl_address where ownerId = {address.OwnerId} ").ToListAsync();
            if (results.Count() < 2)
            {
                throw new UserValidationException("An account must have atleast one address!!!");
            }

            var res = await ById(address.Id);
            if (res == null)
            {
                throw new NotFoundException("Address not found");
            }

            int rows = await db.Database.ExecuteSqlCommandAsync($"delete from tbl_address where id = {address.Id} ");

            if (rows > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Address has been deleted successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "We encounter an error while trying to remove the address. Please try again or contactan administrator"
                };
            }

        }

        public async Task<List<AddressModel>> ByOwnersId(Guid id, Guid lcdaId)
        {
            var address = await db.Addresses.FromSql("sp_lcdaAddressByOwnerId @p0, @p1", new object[] { id, lcdaId }).ToListAsync();
            return address.Select(x => new AddressModel
            {
                Addressnumber = x.Addressnumber,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                Lcdaid = x.Lcdaid,
                OwnerId = x.OwnerId,
                StreetId = x.StreetId
            }).ToList();
        }

        public async Task<List<AddressModel>> ByOwnersId(Guid id)
        {
            var address = await db.Addresses.FromSql("sp_AddressByOwnerId @p0", new object[] { id }).ToListAsync();
            return address.Select(x => new AddressModel
            {
                Addressnumber = x.Addressnumber,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                Lcdaid = x.Lcdaid,
                OwnerId = x.OwnerId,
                StreetId = x.StreetId
            }).ToList();
        }

        public async Task<AddressModel> ById(Guid id)
        {
            var x = await db.Addresses.FromSql($"select tbl_address.*,'none' as streetName from tbl_address where id = {id} ").FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }

            return new AddressModel
            {
                Addressnumber = x.Addressnumber,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                Lcdaid = x.Lcdaid,
                OwnerId = x.OwnerId,
                StreetId = x.StreetId
            };
        }
    }
}
