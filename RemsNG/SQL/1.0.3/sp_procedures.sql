
ALTER procedure [dbo].[sp_paymentSummaryByItems]
(
	@startEnd datetime,
	@endDate datetime
)
as
begin
	select dnItem.id,dnItem.itemAmount,dnItem.amountPaid,dnItem.billingNo,
	'ITEMS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId,item.itemCode from tbl_demandNoticeItem as dnItem
	inner join tbl_taxPayer as tp on tp.id = dnItem.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnItem.itemId
	where ward.wardStatus = 'ACTIVE' and (dnItem.lastModifiedDate between @startEnd and @endDate)
	union

	select dnArrears.id,dnArrears.totalAmount as itemAmount,dnArrears.amountPaid,dnArrears.billingNo,
	'ARREARS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId,item.itemCode  from tbl_demandNoticeArrears as dnArrears
	inner join tbl_taxPayer as tp on tp.id = dnArrears.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnArrears.itemId
	where ward.wardStatus = 'ACTIVE' and (dnArrears.lastModifiedDate between @startEnd and @endDate)
	union 

	select dnPenalty.id,dnPenalty.totalAmount as itemAmount,dnPenalty.amountPaid,dnPenalty.billingNo,
	'PENALTY' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId,item.itemCode from tbl_demandNoticePenalty as dnPenalty
	inner join tbl_taxPayer as tp on tp.id = dnPenalty.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnPenalty.itemId
	where ward.wardStatus = 'ACTIVE' and (dnPenalty.lastModifiedDate between @startEnd and @endDate)
	
end