ALTER procedure sp_addPayment
(
	@taxpayerId uniqueidentifier,
	@billingNumber varchar(30),
	@amount decimal(18,2),
	@charges decimal(18,2),
	@paymentMode varchar(30),
	@referenceNumber varchar(30),
	@bankId uniqueidentifier,
	@createdBy varchar(100),
	@dateCreated datetime
)
as
begin
	declare @msg varchar(200);
	declare @success bit;

	insert into tbl_demandNoticePaymentHistory(id,ownerId,billingNumber,amount,charges,paymentMode,
	referenceNumber,bankId,createdBy,dateCreated,paymentStatus)
	values(NEWID(),@taxpayerId,@billingNumber,@amount,@charges,@paymentMode,@referenceNumber,@bankId,
	@createdBy,@dateCreated,'COMPLETED');

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Payment has been added';
		set @success = 1;
	end
	else
	begin
		set @msg = 'An error occure, Please try again, Payment has been added';
		set @success = 0;
	end
	select NEWID() as id, @msg as msg,@success as success;
end

go

  create procedure sp_searchdemandNoticePaginated
  (
	@pageNum int,
	@pageSize int,
	@query varchar(Max)
  )
  as
  begin
		IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

		declare @count int;
		select @count = count(*) from tbl_demandnotice where query = @query;

		select tbl_demandnotice.*,@count as totalSize from tbl_demandnotice
		where query = @query 
		order by dateCreated desc
		  OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
  end

GO
Create procedure sp_MoveDemandNoticeToArrears
(
	@billingNo varchar(50),
	@taxpayerId uniqueidentifier,
	@previousBillingYr int,
	@billingYr int,
	@createdBy varchar(100),
	@query varchar(50)
)
as 
begin
	declare @msg varchar(200);
	declare @success bit;

	
	select tbl_demandNoticeItem.id,tbl_demandNoticeItem.itemId,tbl_demandNoticeItem.itemAmount,
	tbl_demandNoticeItem.itemName,tbl_demandNoticeItem.itemStatus,
	tbl_demandNoticeItem.amountPaid into #currentItemDemandNotice from tbl_demandNoticeItem
	inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.taxpayerId = tbl_demandNoticeItem.taxpayerId
	inner join tbl_itempenalty itemP on itemP.itemId = tbl_demandNoticeItem.id
	where tbl_demandNoticeTaxpayers.billingYr = @previousBillingYr 
	and tbl_demandNoticeTaxpayers.taxpayerId = @taxpayerId 
	and tbl_demandNoticeItem.itemStatus in ('PENDING','PART_PAYMENT')
	and itemP.duration = @query;

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


