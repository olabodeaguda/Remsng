﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
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
        public ContactRepository(RemsDbContext _db) : base(_db)
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

        public async Task<List<ContactDetailModel>> ByOwnerId(Guid ownerId)
        {
            var entities = await db.ContactDetails.Where(x => x.OwnerId == ownerId).ToListAsync();
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
            var x = await db.ContactDetails.Where(p => p.ContactValue.ToLower() == contactDetail.ContactValue &&
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
            var x = await db.ContactDetails.FirstOrDefaultAsync(p => p.Id == id);
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