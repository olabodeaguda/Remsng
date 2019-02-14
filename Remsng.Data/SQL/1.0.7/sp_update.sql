ALTER procedure [dbo].[sp_currentMaxBilling]
as 
begin
	declare @billingNo bigint;
	declare @success bit;
	set @success = 1;
	if (select count(*) from tbl_demandNoticeTaxpayers) < 1
		begin
			set @billingNo = 0;
		end
	else
		begin
			select @billingNo = Max(CAST(billingNumber AS bigint))  from tbl_demandNoticeTaxpayers;
		end
	select NEWID() as id,cast(@billingNo as varchar(20)) as msg, @success as success
end
go

CREATE procedure sp_cancelpreviousDemandNoticeArrears
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

	UPDATE tbl_demandNoticeItem set itemStatus = 'CANCEL' where id in (select id from #currentItemDemandNotice);

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
go
ALTER procedure [dbo].[sp_updateTaxpayer]
(
	@id uniqueidentifier,
	@companyId uniqueidentifier,
	@streetId uniqueidentifier,
	@addressId uniqueidentifier,
	@lastmodifiedby varchar(100),
	@surname varchar(100),
	@firstname varchar(100),
	@lastname varchar(100)	
)
as
begin
	declare @msg varchar(100);
	declare @success bit;
	if not exists(select * from tbl_taxPayer where id=@id)
	begin
		set @msg = 'Company does not exist';
		set @success = 0;
	end
	else
	begin
		update tbl_taxPayer set companyId=@companyId,addressId=@addressId,lastmodifiedby=@lastmodifiedby,
		streetId=@streetId,lastModifiedDate=getDate(),surname=@surname, firstname=@firstname, lastname=@lastname where id=@id;
		
		update tbl_demandNoticeTaxpayers set taxpayersName = CONCAT(@surname,' ',@firstname,' ',@lastname) where taxpayerId=@id;	

		if @@Rowcount > 0
		begin
			set @msg = 'Request has been updated successfully';
			set @success = 1;
		end
	end

	select newid() as id,@msg as msg,@success as success;
end
