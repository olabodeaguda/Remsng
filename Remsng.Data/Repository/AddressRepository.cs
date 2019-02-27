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
    public class AddressRepository : AbstractRepository
    {
        public AddressRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(AddressModel address)
        {
            db.Set<Address>().Add(new Address
            {
                Addressnumber = address.Addressnumber,
                StreetId = address.StreetId,
                OwnerId = address.OwnerId,
                Lcdaid = address.Lcdaid,
                CreatedBy = address.CreatedBy
            });
            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = $"Address has been added sucessfully"
            };

        }

        public async Task<Response> Update(AddressModel address)
        {
            var entity = await db.Set<Address>().FindAsync(address.Id);
            if (entity == null)
            {
                throw new NotFoundException("Address does not exist");
            }

            entity.Addressnumber = address.Addressnumber;
            entity.StreetId = address.StreetId;
            entity.OwnerId = address.OwnerId;
            entity.Lcdaid = address.Lcdaid;
            entity.Lastmodifiedby = address.Lastmodifiedby;
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Address has been updated successfully"
            };
        }

        public async Task<Response> Remove(AddressModel address)
        {
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
            var models = await db.Set<Address>()
                .Join(db.Set<Street>(), x => x.StreetId, str => str.Id, (x, str) => new AddressModel
                {
                    Addressnumber = x.Addressnumber,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    Lcdaid = x.Lcdaid,
                    OwnerId = x.OwnerId,
                    StreetId = x.StreetId,
                    StreetName = str.StreetName
                }).Where(x => x.OwnerId == id && x.Lcdaid == lcdaId).ToListAsync();
            return models;
        }

        public async Task<List<AddressModel>> ByOwnersId(Guid id)
        {
            var address = await db.Set<Address>()
                .Include(x => x.Street).Select(x => new AddressModel
                {
                    Addressnumber = x.Addressnumber,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    Lcdaid = x.Lcdaid,
                    OwnerId = x.OwnerId,
                    StreetId = x.StreetId,
                    StreetName = x.Street.StreetName
                })
                .Where(x => x.OwnerId == id).ToListAsync();//.FromSql("sp_AddressByOwnerId @p0", new object[] { id }).ToListAsync();
            return address;
        }

        public async Task<AddressModel> ById(Guid id)
        {
            var model = await db.Set<Address>()
                .Join(db.Set<Street>(), x => x.StreetId, str => str.Id, (x, str) => new AddressModel
                {
                    Addressnumber = x.Addressnumber,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    Lcdaid = x.Lcdaid,
                    OwnerId = x.OwnerId,
                    StreetId = x.StreetId,
                    StreetName = str.StreetName
                }).FirstOrDefaultAsync(x => x.Id == id); //await db.Set<Address>().FromSql($"select tbl_address.*,'none' as streetName from tbl_address where id = {id} ").FirstOrDefaultAsync();
            if (model == null)
            {
                return null;
            }

            return model;
        }
    }
}
