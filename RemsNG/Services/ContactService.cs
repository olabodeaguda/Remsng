using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class ContactService : IContactService
    {
        private ContactDao contactDao;
        public ContactService(RemsDbContext db)
        {
            contactDao = new ContactDao(db);
        }

        public async Task<bool> Add(ContactDetail contactDetail)
        {
            return await contactDao.Add(contactDetail);
        }

        public async Task<ContactDetail> ByContactValue(ContactDetail contactDetail)
        {
            return await contactDao.ByContactValue(contactDetail);
        }

        public async Task<ContactDetail> ById(Guid id)
        {
            return await contactDao.ById(id);
        }

        public async Task<List<ContactDetail>> ByOwnerId(Guid ownerId)
        {
            return await contactDao.ByOwnerId(ownerId);
        }

        public async Task<bool> Remove(ContactDetail contactDetail)
        {
            return await contactDao.Remove(contactDetail);
        }

        public void sample(ref int x)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(ContactDetail contactDetail)
        {
            return await contactDao.Update(contactDetail);
        }
    }
}
