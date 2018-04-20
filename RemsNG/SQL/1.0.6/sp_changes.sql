ALTER procedure sp_previousDemandNoticeArrears
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
	@streetId uniqueidentifier
  )
  as
  begin
  declare @msg varchar(100);
  declare @success bit;

	begin
		insert into tbl_demandnotice(id,query,batchNo,demandNoticeStatus,billingYear,lcdaId,createdBy,
		dateCreated,wardId,streetId) 
		values(@id,@query,@batchno,@demandNoticeStatus,@billingYear,@lcdaId,@createdBy,GETDATE(),
		@wardId,@streetId)
		
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

  CREATE procedure sp_searchdemandNoticePaginated2
  (
	@pageNum int,
	@pageSize int,
	@wardId uniqueidentifier,
	@streetId uniqueidentifier
  )
  as
  begin
		IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

		declare @count int;
		select @count = count(*) from tbl_demandnotice where wardId=@wardId and streetId = @streetId;

		select tbl_demandnotice.*,@count as totalSize from tbl_demandnotice
		where wardId=@wardId and streetId = @streetId 
		order by dateCreated desc
		  OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
  end

