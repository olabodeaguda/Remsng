using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ContactManagers : IContactManagers
    {
        private ContactRepository contactDao;
        public ContactManagers(DbContext db)
        {
            contactDao = new ContactRepository(db);
        }

        public async Task<bool> Add(ContactDetailModel contactDetail)
        {
            return await contactDao.Add(contactDetail);
        }

        public async Task<ContactDetailModel> ByContactValue(ContactDetailModel contactDetail)
        {
            return await contactDao.ByContactValue(contactDetail);
        }

        public async Task<ContactDetailModel> ById(Guid id)
        {
            return await contactDao.ById(id);
        }

        public async Task<List<ContactDetailModel>> ByOwnerId(Guid ownerId)
        {
            return await contactDao.ByOwnerId(ownerId);
        }

        public async Task<bool> Remove(ContactDetailModel contactDetail)
        {
            return await contactDao.Remove(contactDetail);
        }
        
        public async Task<bool> Update(ContactDetailModel contactDetail)
        {
            return await contactDao.Update(contactDetail);
        }
    }
}
