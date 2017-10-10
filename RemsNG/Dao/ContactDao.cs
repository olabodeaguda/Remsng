using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class ContactDao : AbstractDao
    {
        public ContactDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<bool> Add(ContactDetail contactDetail)
        {
            db.Add(contactDetail);
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> Update(ContactDetail contactDetail)
        {
            ContactDetail cd = await db.ContactDetails.FindAsync(new object[] { contactDetail.id });

            if (cd == null)
            {
                throw new NotFoundException("Contact can't be found");
            }
            cd.contactType = contactDetail.contactType;
            cd.contactValue = contactDetail.contactValue;
            cd.lastModifiedDate = DateTime.Now;
            cd.lastmodifiedby = contactDetail.lastmodifiedby;

            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<ContactDetail>> ByOwnerId(Guid ownerId)
        {
            return await db.ContactDetails.Where(x => x.ownerId == ownerId).ToListAsync();
        }

        public async Task<ContactDetail> ByContactValue(ContactDetail contactDetail)
        {
            return await db.ContactDetails.Where(x => x.contactValue.ToLower() == contactDetail.contactValue &&
            x.contactType == contactDetail.contactType).FirstOrDefaultAsync();
        }

        public async Task<ContactDetail> ById(Guid id)
        {
            return await db.ContactDetails.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<bool> Remove(ContactDetail contactDetail)
        {
            db.ContactDetails.Remove(contactDetail);

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
