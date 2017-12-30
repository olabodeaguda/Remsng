ALTER procedure [dbo].[sp_getBillingNumberTotalDue]
(
	@billingno varchar(50)
)
as
begin
	 select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus,tm.itemDescription,'ARREARS' as category
	  from tbl_demandNoticeArrears as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	 union
	  select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus,tm.itemDescription,'PENALTY' as category
	  from tbl_demandNoticePenalty as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	  union
	  select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus,tm.itemDescription,'ITEMS' as category
	  from tbl_demandNoticeItem as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
end
go
