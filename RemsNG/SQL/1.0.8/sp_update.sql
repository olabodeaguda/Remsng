
Create procedure sp_paymentSummaryByItems2
(
	@startEnd datetime,
	@endDate datetime
)
as
begin
	select dnItem.id,dnItem.itemAmount,dnItem.amountPaid,dnItem.billingNo,
	'ITEMS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId,
	item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName,
	dnt.addressName,dnItem.lastModifiedDate
	 from tbl_demandNoticeItem as dnItem
	inner join tbl_taxPayer as tp on tp.id = dnItem.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnItem.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnItem.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnItem.dateCreated between @startEnd and @endDate) 
	or (dnItem.lastModifiedDate between @startEnd and @endDate)) and dnItem.itemStatus in ('PAID', 'PENDING','PART_PAYMENT')
	union

	select dnArrears.id,dnArrears.totalAmount as itemAmount,dnArrears.amountPaid,dnArrears.billingNo,
	'ARREARS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName,
	dnt.addressName,dnArrears.lastModifiedDate  
	from tbl_demandNoticeArrears as dnArrears
	inner join tbl_taxPayer as tp on tp.id = dnArrears.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnArrears.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnArrears.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnArrears.dateCreated between @startEnd and @endDate)
	 or (dnArrears.lastModifiedDate between @startEnd and @endDate)) 
	 and dnArrears.arrearsStatus in ('PAID', 'PENDING','PART_PAYMENT')
	union 

	select dnPenalty.id,dnPenalty.totalAmount as itemAmount,dnPenalty.amountPaid,dnPenalty.billingNo,
	'PENALTY' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName,
	dnt.addressName,dnPenalty.lastModifiedDate 
	from tbl_demandNoticePenalty as dnPenalty
	inner join tbl_taxPayer as tp on tp.id = dnPenalty.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnPenalty.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnPenalty.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnPenalty.dateCreated between @startEnd and @endDate)
	or (dnPenalty.lastModifiedDate between @startEnd and @endDate)) 
	and dnPenalty.itemPenaltyStatus in ('PAID', 'PENDING','PART_PAYMENT')
	
end

go

ALTER procedure sp_paymentSummaryByItems
(
	@startEnd datetime,
	@endDate datetime
)
as
begin
	select dnItem.id,dnItem.itemAmount,dnItem.amountPaid,dnItem.billingNo,
	'ITEMS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId,
	item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName from tbl_demandNoticeItem as dnItem
	inner join tbl_taxPayer as tp on tp.id = dnItem.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnItem.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnItem.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnItem.dateCreated between @startEnd and @endDate) 
	or (dnItem.lastModifiedDate between @startEnd and @endDate)) and dnItem.itemStatus in ('PAID', 'PENDING','PART_PAYMENT')
	union

	select dnArrears.id,dnArrears.totalAmount as itemAmount,dnArrears.amountPaid,dnArrears.billingNo,
	'ARREARS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName  from tbl_demandNoticeArrears as dnArrears
	inner join tbl_taxPayer as tp on tp.id = dnArrears.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnArrears.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnArrears.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnArrears.dateCreated between @startEnd and @endDate)
	 or (dnArrears.lastModifiedDate between @startEnd and @endDate)) 
	 and dnArrears.arrearsStatus in ('PAID', 'PENDING','PART_PAYMENT')
	union 

	select dnPenalty.id,dnPenalty.totalAmount as itemAmount,dnPenalty.amountPaid,dnPenalty.billingNo,
	'PENALTY' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName from tbl_demandNoticePenalty as dnPenalty
	inner join tbl_taxPayer as tp on tp.id = dnPenalty.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnPenalty.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnPenalty.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnPenalty.dateCreated between @startEnd and @endDate)
	or (dnPenalty.lastModifiedDate between @startEnd and @endDate)) 
	and dnPenalty.itemPenaltyStatus in ('PAID', 'PENDING','PART_PAYMENT')
	
end
