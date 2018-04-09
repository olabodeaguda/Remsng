ALTER procedure [dbo].[sp_previousDemandNoticeArrears]
(
	@billingNo varchar(50),
	@taxpayerId uniqueidentifier,
	@previousBillingYr int,
	@billingYr int,
	@createdBy varchar(100)
)
as 
begin
	declare @msg varchar(200);
	declare @success bit;

	
	select tbl_demandNoticeItem.id,tbl_demandNoticeItem.itemId,tbl_demandNoticeItem.itemAmount,
	tbl_demandNoticeItem.itemName,tbl_demandNoticeItem.itemStatus,
	tbl_demandNoticeItem.amountPaid into #currentItemDemandNotice from tbl_demandNoticeItem
	inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.taxpayerId = tbl_demandNoticeItem.taxpayerId
	where tbl_demandNoticeTaxpayers.billingYr <= @previousBillingYr 
	and tbl_demandNoticeTaxpayers.taxpayerId = @taxpayerId 
	and tbl_demandNoticeItem.itemStatus in ('PENDING','PART_PAYMENT');

	insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,arrearsStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate)
	select NEWID(),@billingNo,@taxpayerId,itemAmount, amountPaid,itemId,@previousBillingYr,@billingYr,
	itemStatus,@createdBy,GETDATE(),@createdBy,GETDATE() from #currentItemDemandNotice;

	UPDATE tbl_demandNoticeItem set itemStatus = 'MOVE_TO_ARREARS' where id in (select id from #currentItemDemandNotice);

	if @@ROWCOUNT > 0
	begin
		set @msg = CONVERT(varchar(10), @@ROWCOUNT )+'items for previous year '+ CONVERT(varchar(10), @previousBillingYr )+ ' has been moved to arrears for the current year '+CONVERT(varchar(10),  @billingYr);
		set @success = 1;
	end
	else
	begin
		set @msg = CONVERT(varchar(10), @@ROWCOUNT )+' has been treated';
		set @success = 0;
	end
	select NEWID() as id,@msg as msg, @success as success;
end

GO
