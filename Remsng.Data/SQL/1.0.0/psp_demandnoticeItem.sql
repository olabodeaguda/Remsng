
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticeItem') AND type IN (N'U'))
CREATE TABLE tbl_demandNoticeItem
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	billingNo varchar(50) not null,
	dn_taxpayersDetailsId UNIQUEIDENTIFIER not null foreign key references tbl_demandnotice(id),
	taxpayerId uniqueidentifier not null,
	itemId uniqueidentifier not null foreign key references tbl_item(id),
	itemName varchar(100) not null,
	itemAmount decimal(8,2) not null default 0.0,
	amountPaid decimal(8,2) not null default 0.0,
	itemStatus varchar(100) not null,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addTaxpayerDemandNoticeItem') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addTaxpayerDemandNoticeItem
  GO

  create procedure sp_addTaxpayerDemandNoticeItem(
	@demandNoticeId uniqueidentifier,
	@taxpayerId uniqueidentifier,
	@billinYr int,
	@createdBy varchar(100)
  )
  as
  begin
	  declare @msg varchar(100);
	  declare @success bit;
		insert into tbl_demandNoticeItem(id,dn_taxpayersDetailsId,taxpayerId,itemId,itemName,itemAmount,itemStatus,billingNo,amountPaid,createdBy)
		select NEWID(),@demandNoticeId,tbl_demandNoticeTaxpayers.taxpayerId,tbl_companyItem.itemId,tbl_item.itemDescription
		,tbl_companyItem.amount,'PENDING',tbl_demandNoticeTaxpayers.billingNumber,0.0,@createdBy from tbl_demandNoticeTaxpayers(nolock)
		inner join tbl_companyItem on tbl_companyItem.taxpayerId = tbl_demandNoticeTaxpayers.taxpayerId
		inner join tbl_item on tbl_item.id = tbl_companyItem.itemId
		 where tbl_demandNoticeTaxpayers.billingYr = @billinYr and 
		 tbl_demandNoticeTaxpayers.dnId = @demandNoticeId and tbl_demandNoticeTaxpayers.taxpayerId = @taxpayerId;

		 if @@rowcount > 0
		 begin
			set @msg = 'Request was successfully';
			set @success = 1;
		 end
		 else
		 begin
			set @msg = 'Zero rows affected';
			set @success = 0;
		 end

		 select newid() as id,@msg as msg,@success as success;
  end

GO

