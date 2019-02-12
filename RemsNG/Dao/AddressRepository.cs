using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class AddressRepository : AbstractRepository
    {
        public AddressRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Address address)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_addAddress @p0,@p1,@p2,@p3,@p4, @p5", new object[] {
                    address.id,
                    address.addressnumber,
                    address.streetId,
                    address.ownerId,
                    address.lcdaid,
                    address.createdBy
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

        public async Task<Response> Update(Address address)
        {
            DbResponse dbResponse = await db.Set<DbResponse>()
                .FromSql("sp_updateAddress @p0,@p1,@p2,@p3,@p4,@p5", new object[] {
                    address.addressnumber,
                    address.streetId,
                    address.ownerId,
                    address.lcdaid,
                    address.lastmodifiedby,
                    address.id
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

        public async Task<Response> Remove(Address address)
        {
            var results = await db.Addresses.FromSql($"select tbl_address.*,'none' as streetName from tbl_address where ownerId = {address.ownerId} ").ToListAsync();
            if (results.Count() < 2)
            {
                throw new UserValidationException("An account must have atleast one address!!!");
            }

            var res = await ById(address.id);
            if (res == null)
            {
                throw new NotFoundException("Address not found");
            }

            int rows = await db.Database.ExecuteSqlCommandAsync($"delete from tbl_address where id = {address.id} ");

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

        public async Task<List<Address>> ByOwnersId(Guid id, Guid lcdaId)
        {
            return await db.Addresses.FromSql("sp_lcdaAddressByOwnerId @p0, @p1", new object[] { id, lcdaId }).ToListAsync();
        }

        public async Task<List<Address>> ByOwnersId(Guid id)
        {
            return await db.Addresses.FromSql("sp_AddressByOwnerId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<Address> ById(Guid id)
        {
            return await db.Addresses.FromSql($"select tbl_address.*,'none' as streetName from tbl_address where id = {id} ").FirstOrDefaultAsync();
        }
    }
}
