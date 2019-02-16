using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class AddressManagers : IAddressManagers
    {
        private readonly AddressRepository addressDao;

        public AddressManagers(RemsDbContext _db)
        {
            addressDao = new AddressRepository(_db);
        }

        public async Task<Response> Add(AddressModel address)
        {
            return await addressDao.Add(address);
        }

        public async Task<string> AddressByOwnerId(Guid ownerId)
        {
            string address = string.Empty;
            var addLst = await ByOwnersId(ownerId);
            if (addLst.Count > 0)
            {
                AddressModel d = addLst.FirstOrDefault();
                if (d != null)
                {
                    address = $"{d.Addressnumber}";//, {d.streetName}";
                }
            }

            return address;
        }

        public async Task<AddressModel> ById(Guid id)
        {
            return await addressDao.ById(id);
        }

        public async Task<List<AddressModel>> ByOwnersId(Guid ownerId)
        {
            return await addressDao.ByOwnersId(ownerId);
        }

        public async Task<List<AddressModel>> ByOwnersId(Guid ownerId, Guid lcdaId)
        {
            return await addressDao.ByOwnersId(ownerId, lcdaId);
        }

        public async Task<Response> Remove(AddressModel address)
        {
            return await addressDao.Remove(address);
        }

        public async Task<Response> Update(AddressModel address)
        {
            return await addressDao.Update(address);
        }
    }
}
