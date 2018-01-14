 
 ALTER procedure [dbo].[sp_addDemandNotice]
  (
	@id uniqueidentifier,
	@query varchar(MAX),
	@batchno varchar(20),
	@demandNoticeStatus varchar(50),
	@billingYear int,
	@lcdaId uniqueidentifier,
	@createdBy varchar(100)
  )
  as
  begin
  declare @msg varchar(100);
  declare @success bit;

	--if exists(select * from tbl_demandnotice where query = @query and lcdaId = @lcdaId and billingyear = @billingYear and demandNoticeStatus <> 'CANCEL')
	--begin
	--	set @msg = 'Demand notice already exist';	
	--	set @success = 0;
	--end
	--else
	begin
		insert into tbl_demandnotice(id,query,batchNo,demandNoticeStatus,billingYear,lcdaId,createdBy,dateCreated) 
		values(@id,@query,@batchno,@demandNoticeStatus,@billingYear,@lcdaId,@createdBy,GETDATE())
		
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
GO

ALTER procedure [dbo].[sp_paymentSummaryByItems]
(
	@startEnd datetime,
	@endDate datetime
)
as
begin
	select dnItem.id,dnItem.itemAmount,dnItem.amountPaid,dnItem.billingNo
	'ITEMS',ward.id as wardId,ward.wardName from tbl_demandNoticeItem as dnItem
	inner join tbl_taxPayer as tp on tp.id = dnItem.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	where ward.wardStatus = 'ACTIVE' and (dnItem.lastModifiedDate between @startEnd and @endDate)
	union

	select dnArrears.id,dnArrears.totalAmount,dnArrears.amountPaid,dnArrears.billingNo
	'ITEMS',ward.id as wardId,ward.wardName from tbl_demandNoticeArrears as dnArrears
	inner join tbl_taxPayer as tp on tp.id = dnArrears.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	where ward.wardStatus = 'ACTIVE' and (dnArrears.lastModifiedDate between @startEnd and @endDate)
	union 

	select dnPenalty.id,dnPenalty.totalAmount,dnPenalty.amountPaid,dnPenalty.billingNo
	'ITEMS',ward.id as wardId,ward.wardName from tbl_demandNoticePenalty as dnPenalty
	inner join tbl_taxPayer as tp on tp.id = dnPenalty.taxpayerId
	inner join tbl_street as street on street.id = tp.streetId
	inner join tbl_ward as ward on ward.id = street.wardId
	where ward.wardStatus = 'ACTIVE' and (dnPenalty.lastModifiedDate between @startEnd and @endDate)
	
end

GO
ALTER procedure [dbo].[sp_getBillingNumberTotalDue]
(
	@billingno varchar(50)
)
as
begin
	select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus,
	 tm.itemDescription,'ARREARS' as category,dn.itemId
	  from tbl_demandNoticeArrears as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	 union
	  select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus,
	  tm.itemDescription,'PENALTY' as category,dn.itemId
	  from tbl_demandNoticePenalty as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	  union
	  select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus,
	  tm.itemDescription,'ITEMS' as category,dn.itemId
	  from tbl_demandNoticeItem as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
end
go
