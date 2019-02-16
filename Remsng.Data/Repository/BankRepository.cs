﻿using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class BankRepository : AbstractRepository
    {
        public BankRepository(DbContext _db) : base(_db)
        { }

        public async Task<List<BankModel>> GetBankAsync()
        {
            var bk = await db.Set<Bank>().FromSql("select * from tbl_bank order by bankName").ToListAsync();

            return bk.Select(x => new BankModel
            {
                BankName = x.BankName,
                DateCreated = x.DateCreated,
                Id = x.Id
            }).ToList();
        }

        public async Task<BankCategory[]> GetBankCategory(Guid lcdaId)
        {

            return new BankCategory[] { };
        }

    }
}
