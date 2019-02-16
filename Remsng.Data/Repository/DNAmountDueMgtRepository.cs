using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DNAmountDueMgtRepository : AbstractRepository
    {
        public DNAmountDueMgtRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string billingno)
        {
            var result = await db.Set<DNAmountDueModel>()
                .FromSql("sp_getBillingNumberTotalDue @p0", new object[] { billingno }).ToListAsync();
            return result;
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string[] bills)
        {
            string bnos = bills.FormatString();

            string query = $"select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus, " +
                $"'description' as itemDescription,'ARREARS' as category,dn.itemId, dn.billingNo " +
                $"from tbl_demandNoticeArrears as dn " +
                //$"inner join tbl_item as tm on tm.id = dn.itemId " +
                $"where dn.billingNo in ({bnos})  and dn.arrearsStatus in ('PART_PAYMENT','PENDING')" +
                $"union " +
                $"select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus, " +
                $"tm.itemDescription,'PENALTY' as category,dn.itemId, dn.billingNo " +
                $"from tbl_demandNoticePenalty as dn " +
                $"inner join tbl_item as tm on tm.id = dn.itemId " +
                $"where billingNo in ({bnos})  and dn.itemPenaltyStatus in ('PART_PAYMENT','PENDING')" +
                $"union " +
                $"select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus, " +
                $"tm.itemDescription,'ITEMS' as category,dn.itemId, dn.billingNo " +
                $"from tbl_demandNoticeItem as dn " +
                $"inner join tbl_item as tm on tm.id = dn.itemId " +
                $"where billingNo in ({bnos})  and dn.itemStatus in ('PART_PAYMENT','PENDING')";
            return await db.Set<DNAmountDueModel>()
                .FromSql(query).ToListAsync();
        }

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            int count = 0;
            if (dnamount.category == "ARREARS")
            {
                string query = $"Update tbl_demandNoticeArrears set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";

                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "PENALTY")
            {
                string query = $"Update tbl_demandNoticePenalty set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "ITEMS")
            {
                string query = $"Update tbl_demandNoticeItem set itemAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }

            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Update was not successful.Please try again!!!"
                };
            }

        }

        public string PaymentQuery(List<DNAmountDueModel> paymentDueList,
            DemandNoticePaymentHistoryModel dnph, string status, string createdby)
        {
            string query = "";

            if (paymentDueList.Count > 0)
            {
                foreach (var tm in paymentDueList)
                {
                    switch (tm.category)
                    {
                        case "ARREARS":
                            query = query + $"update tbl_demandNoticeArrears set amountPaid = {tm.amountPaid}, arrearsStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        case "PENALTY":
                            query = query + $"update tbl_demandNoticePenalty set amountPaid = {tm.amountPaid}, itemPenaltyStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        case "ITEMS":
                            query = query + $"update tbl_demandNoticeItem set amountPaid = {tm.amountPaid}, itemStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        default:
                            break;
                    }
                }

                query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where billingNumber = '{dnph.BillingNumber}';";
                query = query + $"update tbl_demandNoticePaymentHistory set paymentStatus = 'APPROVED' where id = '{dnph.Id}';";
            }

            return query;
        }
    }
}
