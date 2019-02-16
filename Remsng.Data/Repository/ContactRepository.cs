using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ContactRepository : AbstractRepository
    {
        public ContactRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<bool> Add(ContactDetailModel contactDetail)
        {
            db.Add(contactDetail);
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> Update(ContactDetailModel contactDetail)
        {
            ContactDetail cd = await db.Set<ContactDetail>().FindAsync(new object[] { contactDetail.Id });

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

        public async Task<List<ContactDetailModel>> ByOwnerId(Guid ownerId)
        {
            var entities = await db.Set<ContactDetail>().Where(x => x.OwnerId == ownerId).ToListAsync();
            return entities.Select(x => new ContactDetailModel()
            {
                ContactType = x.ContactType,
                ContactValue = x.ContactValue,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId
            }).ToList();
        }

        public async Task<ContactDetailModel> ByContactValue(ContactDetailModel contactDetail)
        {
            var x = await db.Set<ContactDetail>().Where(p => p.ContactValue.ToLower() == contactDetail.ContactValue &&
            p.ContactType == contactDetail.ContactType).FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }

            return new ContactDetailModel()
            {
                ContactType = x.ContactType,
                ContactValue = x.ContactValue,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId
            };
        }

        public async Task<ContactDetailModel> ById(Guid id)
        {
            var x = await db.Set<ContactDetail>().FirstOrDefaultAsync(p => p.Id == id);
            if (x == null)
            {
                return null;
            }

            return new ContactDetailModel()
            {
                ContactType = x.ContactType,
                ContactValue = x.ContactValue,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId
            };
        }

        public async Task<bool> Remove(ContactDetailModel contactDetail)
        {
            db.Set<ContactDetail>().Remove(new ContactDetail()
            {
                ContactType = contactDetail.ContactType,
                ContactValue = contactDetail.ContactValue,
                CreatedBy = contactDetail.CreatedBy,
                DateCreated = contactDetail.DateCreated,
                Id = contactDetail.Id,
                Lastmodifiedby = contactDetail.Lastmodifiedby,
                LastModifiedDate = contactDetail.LastModifiedDate,
                OwnerId = contactDetail.OwnerId
            });

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
