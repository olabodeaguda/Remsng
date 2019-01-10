ALTER procedure sp_addDemandNotice
  (
	@id uniqueidentifier,
	@query varchar(MAX),
	@batchno varchar(20),
	@demandNoticeStatus varchar(50),
	@billingYear int,
	@lcdaId uniqueidentifier,
	@createdBy varchar(100),
	@wardId uniqueidentifier,
	@streetId uniqueidentifier,
	@isUnbilled bit
  )
  as
  begin
  declare @msg varchar(100);
  declare @success bit;

	begin
		insert into tbl_demandnotice(id,query,batchNo,demandNoticeStatus,billingYear,lcdaId,createdBy,
		dateCreated,wardId,streetId,isUnbilled) 
		values(@id,@query,@batchno,@demandNoticeStatus,@billingYear,@lcdaId,@createdBy,GETDATE(),
		@wardId,@streetId, @isUnbilled)
		
		if @@ROWCOUNT > 0
		begin
			set @msg = 'Demand notice has been submited for onward processing';
			set @success = 1
		end
		else
		begin
			set @msg = 'Database Error: An error occur while trying to submit demand notice request';
			set @success = 0;
		end
	end

	select NEWID() as id, @msg  as msg,@success as success
  end
  
  go

  ALTER procedure sp_paymentSummaryByItems2
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
	or (dnItem.lastModifiedDate between @startEnd and @endDate)) 
	and dnItem.itemStatus in ('PAID', 'PENDING','PART_PAYMENT')
	and dnt.isUnbilled = 0
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
	and dnt.isUnbilled = 0
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
	and dnt.isUnbilled = 0
	
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
	item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName
	,dnt.addressName,dnItem.lastModifiedDate from tbl_demandNoticeItem as dnItem
	inner join tbl_taxPayer as tp on tp.id = dnItem.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnItem.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnItem.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnItem.dateCreated between @startEnd and @endDate) 
	or (dnItem.lastModifiedDate between @startEnd and @endDate))
	and dnt.isUnbilled = 0
	union

	select dnArrears.id,dnArrears.totalAmount as itemAmount,dnArrears.amountPaid,dnArrears.billingNo,
	'ARREARS' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName,
	dnt.addressName,dnArrears.lastModifiedDate  from tbl_demandNoticeArrears as dnArrears
	inner join tbl_taxPayer as tp on tp.id = dnArrears.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnArrears.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnArrears.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnArrears.dateCreated between @startEnd and @endDate)
	 or (dnArrears.lastModifiedDate between @startEnd and @endDate)) 
	and dnt.isUnbilled = 0
	union 

	select dnPenalty.id,dnPenalty.totalAmount as itemAmount,dnPenalty.amountPaid,dnPenalty.billingNo,
	'PENALTY' as category,ward.id as wardId,ward.wardName,item.itemDescription,item.id as itemId
	,item.itemCode,CONCAT(tp.firstname,' ',tp.lastname,' ',tp.surname) as taxpayersName,
	dnt.addressName,dnPenalty.lastModifiedDate  from tbl_demandNoticePenalty as dnPenalty
	inner join tbl_taxPayer as tp on tp.id = dnPenalty.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	inner join tbl_item as item on item.id = dnPenalty.itemId
	inner join tbl_demandNoticeTaxpayers as dnt on dnt.taxpayerId = dnPenalty.taxpayerId
	inner join tbl_demandnotice on tbl_demandnotice.id = dnt.dnId
	where ward.wardStatus = 'ACTIVE' and ((dnPenalty.dateCreated between @startEnd and @endDate)
	or (dnPenalty.lastModifiedDate between @startEnd and @endDate)) 
	and dnt.isUnbilled = 0
	
end
go
create procedure sp_taxpayerpaymentHistory
(@taxPayerId uniqueidentifier)
as
begin
select ndh.* from tbl_demandNoticePaymentHistory as ndh
inner join tbl_demandNoticeTaxpayers as dnt on dnt.billingNumber = ndh.billingNumber
where ndh.paymentStatus = 'APPROVED' and  dnt.taxpayerId = @taxPayerId
order by ndh.dateCreated desc
end
