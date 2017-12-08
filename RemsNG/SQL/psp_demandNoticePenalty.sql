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
	and dnp.itemPenaltyStatus in ('PENDING','PART_PAYMENT');

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

