
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_companyItem') AND type IN (N'U'))
CREATE TABLE tbl_companyItem
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	companyId UNIQUEIDENTIFIER not null foreign key references tbl_company(id),
	itemId uniqueidentifier not null foreign key references tbl_item(id),
	amount decimal(8,2) not null default 0.0,
	billingYear int not null,		
	createdBy varchar(100) NOT NULL,
	dateCreated datetime NOT NULL DEFAULT GETDATE(),
	lastmodifiedby varchar(100) NULL,
	lastModifiedDate [datetime] NULL,
	companyStatus varchar(100)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_createCompanyItem') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_createCompanyItem
GO
create procedure sp_createCompanyItem
(
	@id uniqueidentifier,
	@companyid uniqueidentifier,
	@itemId uniqueidentifier,
	@amount decimal,
	@billingYear int,
	@createdBy varchar(100),
	@companyStatus varchar(100)
)
as
begin
	declare @msg varchar(200);
	declare @success bit;

	if exists(select * from tbl_companyItem where companyId=@companyid and itemId= @itemId)
	begin
		set @msg = 'Item already exist';
		set @success = 0;
	end
	else
	begin
		insert into tbl_companyItem(id,companyId,itemId,amount,billingYear,createdBy,dateCreated,companyStatus) 
		values(@id,@companyid,@itemId,@amount,@billingYear,@createdBy,GETDATE(),@companyStatus);

		if @@ROWCOUNT > 0
		begin
			set @msg = 'Item has been added successfully';
			set @success = 1;
		end
		else
		begin
			set @msg = 'Database error: please contact your administrator or try again';
			set @success = 0;
		end

	end
	select NEWID() as id,@msg as msg,@success as success;
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateCompanyItem') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateCompanyItem
GO
create procedure sp_updateCompanyItem
(
	@id uniqueidentifier,
	@companyid uniqueidentifier,
	@itemId uniqueidentifier,
	@amount decimal,
	@billingYear int,
	@createdBy varchar(100)
)
as
begin
	declare @msg varchar(200);
	declare @success bit;
	
	if not exists(Select * from tbl_companyItem where id = @id)
	begin
		set @msg = 'Item does not exist';
		set @success = 0;
	end
	else
	begin
		update tbl_companyItem set companyId=@companyid, itemId = @itemId, amount = @amount,
		billingYear=@billingYear, createdBy = @createdBy where id=@id;

		if @@ROWCOUNT > 0
		begin
			set @msg = 'Request has been updated successfully';
			set @success = 1;
		end
		else
		begin
			set @msg = 'Request failed. Try again or contact an administrator';
			set @success = 0;
		end
	end

end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateCompanyItemStatus') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateCompanyItemStatus
GO
create procedure sp_updateCompanyItemStatus
(
	@id uniqueidentifier,	
	@companyStatus varchar(100)
)
as
begin
	declare @msg varchar(200);
	declare @success bit;
	
	if not exists(Select * from tbl_companyItem where id = @id)
	begin
		set @msg = 'Item does not exist';
		set @success = 0;
	end
	else
	begin
		update tbl_companyItem set companyStatus=@companyStatus where id=@id;

		if @@ROWCOUNT > 0
		begin
			set @msg = 'Request has been updated successfully';
			set @success = 1;
		end
		else
		begin
			set @msg = 'Request failed. Try again or contact an administrator';
			set @success = 0;
		end
	end
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_companyItemByCompanyId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_companyItemByCompanyId
GO
create procedure sp_companyItemByCompanyId
(
	@companyId uniqueidentifier
)
as
begin
	select tbl_companyItem.*,tbl_company.companyName, tbl_item.itemDescription as itemName from tbl_companyItem 
	inner join tbl_company on tbl_company.id = tbl_companyItem.id
	inner join tbl_item on tbl_item.id = tbl_companyItem.itemId
	where tbl_companyItem.companyId = @companyId
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_companyItemById') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_companyItemById
GO
create procedure sp_companyItemById
(
	@id uniqueidentifier
)
as
begin
	select tbl_companyItem.*,tbl_company.companyName, tbl_item.itemDescription as itemName from tbl_companyItem 
	inner join tbl_company on tbl_company.id = tbl_companyItem.id
	inner join tbl_item on tbl_item.id = tbl_companyItem.itemId
	where tbl_companyItem.id = @id
end

GO
