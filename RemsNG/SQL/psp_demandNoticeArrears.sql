IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticeArrears') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandNoticeArrears
			  (
				  id uniqueidentifier not null primary key,
				  billingNo varchar(50) not null,
				  taxpayerId uniqueidentifier not null,
				  totalAmount decimal(18,2) not null,
				  amountPaid decimal(18,2) not null,
				  itemId uniqueidentifier not null,
				  originatedYear int not null,
				  billingYear int not null,
				  arrearsStatus varchar(50) default 'PENDING',
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_MovedDemandNoticeArrears') AND type IN (N'P', N'PC'))
		  drop procedure sp_MovedDemandNoticeArrears
GO
create procedure sp_MovedDemandNoticeArrears
(
	@billingNo varchar(50),
	@taxpayerId uniqueidentifier,
	@billingYr int,
	@arrearstatus varchar(50),
	@createdBy varchar(100)
)
as
begin
	declare @msg varchar(200);
	declare @success bit;

	select id,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,arrearsStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate into #CurrentArrears from tbl_demandNoticeArrears
	where  arrearsStatus  not in ('PAID','MOVED','CANCEL') and taxpayerId = @taxpayerId;

	;with taxPayerCurrentArrears(id,oldId,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,arrearsStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate) 
	as 
	(
		select NEWID(),id,@billingNo,@taxpayerId,totalAmount, amountPaid,itemId,originatedYear,@billingYr,
		@arrearstatus,@createdBy,GETDATE(),@createdBy,GETDATE() from #CurrentArrears
	)
	insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,arrearsStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate)
	select newid(),@billingNo,@taxpayerId,totalAmount, amountPaid,itemId,originatedYear,@billingYr,
	@arrearstatus,@createdBy,GETDATE(),@createdBy,GETDATE() from taxPayerCurrentArrears;

	update tbl_demandNoticeArrears set @arrearstatus = 'MOVED' WHERE id in(select id from #CurrentArrears)

	if @@ROWCOUNT > 0
	begin
		set @msg = @@ROWCOUNT+' has been moved to the current year '+@billingYr;
		set @success = 1;
	end
	else
	begin
		set @msg = @@ROWCOUNT+' has been moved to the current year '+@billingYr;
		set @success = 0;
	end
	select NEWID() as id,@msg as msg, @success as success;
end

GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_previousDemandNoticeArrears') AND type IN (N'P', N'PC'))
		  drop procedure sp_previousDemandNoticeArrears
GO
create procedure sp_previousDemandNoticeArrears
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
	inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.id = tbl_demandNoticeItem.dn_taxpayersDetailsId
	where tbl_demandNoticeTaxpayers.billingYr = @previousBillingYr 
	and tbl_demandNoticeTaxpayers.taxpayerId = @taxpayerId 
	and tbl_demandNoticeItem.itemStatus not in ('PENDING','PART_PAYMENT');

	insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,arrearsStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate)
	select NEWID(),@billingNo,@taxpayerId,itemAmount, amountPaid,itemId,@previousBillingYr,@billingYr,
	itemStatus,@createdBy,GETDATE(),@createdBy,GETDATE() from #currentItemDemandNotice;

	UPDATE tbl_demandNoticeItem set itemStatus = 'MOVE_TO_ARREARS' where id in (select id from #currentItemDemandNotice);

	if @@ROWCOUNT > 0
	begin
		set @msg = @@ROWCOUNT+'items for previous year '+@previousBillingYr+ ' has been moved to arrears for the current year '+@billingYr;
		set @success = 1;
	end
	else
	begin
		set @msg = @@ROWCOUNT+' has been treated';
		set @success = 0;
	end
	select NEWID() as id,@msg as msg, @success as success;
end


