using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class DNAmountDueMgtService : IDNAmountDueMgtService
    {
        private DNAmountDueMgtDao dNAmountDueMgtDao;
        public DNAmountDueMgtService(RemsDbContext _db)
        {
            dNAmountDueMgtDao = new DNAmountDueMgtDao(_db);
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string billingno)
        {
            return await dNAmountDueMgtDao.ByBillingNo(billingno);
        }        

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            return await dNAmountDueMgtDao.UpdateAmount(dnamount);
        }
    }
}
