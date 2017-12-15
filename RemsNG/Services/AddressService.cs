using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class AddressService : IAddress
    {
        private AddressDao addressDao;

        public AddressService(RemsDbContext _db)
        {
            addressDao = new AddressDao(_db);
        }

        public async Task<Response> Add(Address address)
        {
            return await addressDao.Add(address);
        }

        public async Task<string> AddressByOwnerId(Guid ownerId)
        {
            string address = string.Empty;
            var addLst = await ByOwnersId(ownerId);
            if (addLst.Count > 0)
            {
                Address d = addLst.FirstOrDefault();
                if (d != null)
                {
                    address = $"{d.addressnumber}, {d.streetName}";
                }
            }

            return address;
        }

        public async Task<Address> ById(Guid id)
        {
            return await addressDao.ById(id);
        }

        public async Task<List<Address>> ByOwnersId(Guid ownerId)
        {
            return await addressDao.ByOwnersId(ownerId);
        }

        public async Task<List<Address>> ByOwnersId(Guid ownerId, Guid lcdaId)
        {
            return await addressDao.ByOwnersId(ownerId, lcdaId);
        }

        public async Task<Response> Remove(Address address)
        {
            return await addressDao.Remove(address);
        }

        public async Task<Response> Update(Address address)
        {
            return await addressDao.Update(address);
        }
    }
}
