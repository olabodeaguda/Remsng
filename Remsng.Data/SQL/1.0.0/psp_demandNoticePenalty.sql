IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticePenalty') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandNoticePenalty
			  (
				  id uniqueidentifier not null primary key,
				  billingNo varchar(50) not null,
				  taxpayerId uniqueidentifier not null,
				  totalAmount decimal(18,2) not null,
				  amountPaid decimal(18,2) not null,
				  itemId uniqueidentifier not null,
				  originatedYear int not null,
				  billingYear int not null,
				  itemPenaltyStatus varchar(50) default 'PENDING',
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_moveTaxpayersPenalty') AND type IN (N'P', N'PC'))
		  drop procedure sp_moveTaxpayersPenalty
GO
create procedure sp_moveTaxpayersPenalty
(
	@billingNo varchar(50),
	@taxpayerId uniqueidentifier,
	@billingYr int,
	@createdBy varchar(100)
)
as 
begin
	declare @msg varchar(200);
	declare @success bit;
	
	select * into #currentItemDemandNotice from tbl_demandNoticePenalty as dnp
	where dnp.taxpayerId = @taxpayerId 
	and dnp.itemPenaltyStatus in ('PENDING','PART_PAYMENT') and billingYear < @billingYr;

	insert into tbl_demandNoticePenalty(id,billingNo,taxpayerId,totalAmount, amountPaid,itemId,originatedYear,
	billingYear,itemPenaltyStatus,createdBy,dateCreated,lastmodifiedby,lastModifiedDate)
	select NEWID(),@billingNo,@taxpayerId,totalAmount, amountPaid,itemId,originatedYear,@billingYr,
	'PENDING',@createdBy,GETDATE(),@createdBy,GETDATE() from #currentItemDemandNotice;

	UPDATE tbl_demandNoticePenalty set itemPenaltyStatus = 'MOVED' where id in (select id from #currentItemDemandNotice);

	if @@ROWCOUNT > 0
	begin
		set @msg = @@ROWCOUNT+' penalty has been treated for billing number '+@billingNo;
		set @success = 1;
	end
	else
	begin
		set @msg = @@ROWCOUNT+' has been treated';
		set @success = 0;
	end
	select NEWID() as id,@msg as msg, @success as success;
end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_penaltyTracker') AND type IN (N'P', N'PC'))
		  drop procedure sp_penaltyTracker
GO
create procedure sp_penaltyTracker
as
begin
	
	select CONCAT(taxpayerId,billingNo,itemId) as identityV into #penalizeAmount from tbl_demandNoticeItem where 
	tbl_demandNoticeItem.itemStatus in ('PENDING','PART_PAYMENT');

	;with alreadyPenalized(taxpayerId) as (
			select taxpayerId from tbl_demandNoticePenalty 
			where CONCAT(taxpayerId,billingNo,itemId) = (select top 1 identityV from #penalizeAmount)
	)
	select distinct tbl_demandNoticeItem.*,tbl_itempenalty.amount as penaltyAmount,tbl_itempenalty.duration from tbl_demandNoticeItem
	inner join tbl_itempenalty on tbl_itempenalty.itemId = tbl_demandNoticeItem.itemId
	inner join tbl_demandNotice as dd on dd.id = tbl_demandNoticeItem.dn_taxpayersDetailsId
	where tbl_itempenalty.penaltyStatus = 'ACTIVE' and tbl_demandNoticeItem.itemStatus in ('PENDING','PART_PAYMENT')
	and( tbl_demandNoticeItem.taxpayerId not in(select taxpayerId from alreadyPenalized))

end

GO
