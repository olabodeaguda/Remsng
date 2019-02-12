using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class ContactRepository : AbstractRepository
    {
        public ContactRepository(RemsDbContext _db) : base(_db)
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
            ContactDetail cd = await db.ContactDetails.FindAsync(new object[] { contactDetail.Id });

            if (cd == null)
            {
                throw new NotFoundException("Contact can't be found");
            }
            cd.ContactType = contactDetail.ContactType;
            cd.ContactValue = contactDetail.ContactValue;
            cd.LastModifiedDate = DateTime.Now;
            cd.Lastmodifiedby = contactDetail.Lastmodifiedby;

            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<ContactDetail>> ByOwnerId(Guid ownerId)
        {
            return await db.ContactDetails.Where(x => x.OwnerId == ownerId).ToListAsync();
        }

        public async Task<ContactDetail> ByContactValue(ContactDetail contactDetail)
        {
            return await db.ContactDetails.Where(x => x.ContactValue.ToLower() == contactDetail.ContactValue &&
            x.ContactType == contactDetail.ContactType).FirstOrDefaultAsync();
        }

        public async Task<ContactDetail> ById(Guid id)
        {
            return await db.ContactDetails.FirstOrDefaultAsync(x => x.Id == id);
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
