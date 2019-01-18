ALTER procedure sp_getBillingNumberTotalDue
(
	@billingno varchar(50)
)
as
begin
	select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus,
	 'description' as itemDescription,'ARREARS' as category,dn.itemId, dn.billingNo
	  from tbl_demandNoticeArrears as dn
	 --inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	 union
	  select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus,
	  'Penalty' as itemDescription,'PENALTY' as category,dn.itemId, dn.billingNo
	  from tbl_demandNoticePenalty as dn
	  where taxpayerId = (select top 1 taxpayerId from tbl_demandNoticeTaxpayers where billingNumber = @billingno)
	  union
	  select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus,
	  tm.itemDescription,'ITEMS' as category,dn.itemId, dn.billingNo
	  from tbl_demandNoticeItem as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
end