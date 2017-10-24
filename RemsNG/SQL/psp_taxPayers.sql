
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_taxPayer') AND type IN (N'U'))
			  CREATE TABLE tbl_taxPayer
			  (
				id UNIQUEIDENTIFIER not null primary key,
				companyId UNIQUEIDENTIFIER not null foreign key references tbl_company(id),
				streetId uniqueidentifier foreign key references tbl_street(id),
				addressId uniqueidentifier null ,
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime,
				taxpayerStatus varchar(50),
				surname varchar(100),
				firstname varchar(100),
				lastname varchar(100)
			  )
			  GO


IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addTaxpayer') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addTaxpayer
GO
create procedure sp_addTaxpayer
(
	@id uniqueidentifier,
	@companyId varchar(200),
	@streetId uniqueidentifier,
	@addressId uniqueidentifier,
	@createdBy varchar(100),
	@surname varchar(100),
	@firstname varchar(100),
	@lastname varchar(100)
)
as
begin
	declare @msg varchar(100);
	declare @success bit;
	if not exists(select * from tbl_street where id = @streetId)
	begin
		set @msg = 'Street does not exist';
		set @success = 0;
	end
	else
	begin	
		insert into tbl_taxPayer(id,companyId,addressId,createdby,streetId,surname,firstname,lastname) 
		values(@id,@companyId,@addressId,@createdBy,@streetId,@surname,@firstname,@lastname)

		if @@Rowcount > 0
		begin
			set @msg = 'Request has been added successfully';
			set @success = 1;
		end
	end

	select newid() as id,@msg as msg,@success as success;
end

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerById') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerById
GO
create procedure sp_TaxpayerById
(
	@id uniqueidentifier
)
as
begin
	declare @count int;

	(select @count = Count(*) from tbl_taxPayer 
	inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId);

	select tbl_taxPayer.*,tbl_company.companyName as companyName, tbl_Address.addressnumber as streetNumber,@count as totalSize from tbl_taxPayer 
	inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	where tbl_taxPayer.id = @id

end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerByStreetId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerByStreetId
GO
create procedure sp_TaxpayerByStreetId
(
	@streetId uniqueidentifier
)
as
begin
	select tbl_taxPayer.*,tbl_company.companyName as companyName,@count as totalSize,tbl_Address.addressnumber as streetNumber from tbl_taxPayer 
	inner join tbl_address on tbl_address.id = ownerId.tbl_taxPayer.id
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	where tbl_taxPayer.streetId = @streetId

end

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerByStreetIdPaginated') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerByStreetIdPaginated
GO
create procedure sp_TaxpayerByStreetIdPaginated
(
	@streetId uniqueidentifier,
	@pageSize int,
	@pageNum int
)
as
begin

declare @count int;
select @count = count(*) from tbl_taxPayer 
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	where tbl_taxPayer.streetId = @streetId;

IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;
	select tbl_taxPayer.*,tbl_company.companyName as companyName, tbl_Address.addressnumber as streetNumber,@count as totalSize from tbl_taxPayer 
	inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	where tbl_taxPayer.streetId = @streetId
	ORDER BY tbl_company.companyName desc
                 OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
end


GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerByCompanyId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerByCompanyId
GO
create procedure sp_TaxpayerByCompanyId
(
	@companyId uniqueidentifier
)
as
begin
	select top 1 tbl_taxPayer.*,tbl_company.companyName as companyName from tbl_taxPayer 
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	where tbl_taxPayer.companyId = @companyId
end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateTaxpayer') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateTaxpayer
GO
create procedure sp_updateTaxpayer
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
		streetId=@streetId,lastModifiedDate=getDate(),surname=@surname, firstname=@firstname, lastname=@lastname where id=@id	

		if @@Rowcount > 0
		begin
			set @msg = 'Request has been updated successfully';
			set @success = 1;
		end
	end

	select newid() as id,@msg as msg,@success as success;
end

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateStatusTaxpayer') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateStatusTaxpayer
GO

create procedure sp_updateStatusTaxpayer(
	@id uniqueidentifier,
	@status varchar(50)
)
as
begin
	declare @msg varchar(100);
	declare @success bit;
	
	if not exists(select * from tbl_taxPayer where id=@id)
	begin
		set @msg = 'Tax payer does not exist'
		set @success = 0;
	end
	else 
	begin
		update tbl_taxPayer set taxpayerStatus = @status where id=@id
		if @@Rowcount > 0
				begin
					set @msg = 'Request has been updated successfully';
					set @success = 1;
				end
			end

	select newid() as id,@msg as msg,@success as success;
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerByLcdaId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerByLcdaId
GO
create procedure sp_TaxpayerByLcdaId
(
	@lcdaId uniqueidentifier
)
as
begin
	select tbl_taxPayer.*,tbl_company.companyName as companyName from tbl_taxPayer 
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
	inner join tbl_ward on tbl_ward.id = tbl_street.wardId
	where tbl_ward.lcdaId = @lcdaId
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_TaxpayerByLcdaIdpaginated') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_TaxpayerByLcdaIdpaginated
GO
create procedure sp_TaxpayerByLcdaIdpaginated
(
	@lcdaId uniqueidentifier,
	@pageSize int,
	@pageNum int
)
as
begin
IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

declare @count int;

(select @count = count(*) from tbl_taxPayer 
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
	inner join tbl_ward on tbl_ward.id = tbl_street.wardId
	where tbl_ward.lcdaId = @lcdaId)

	select tbl_taxPayer.*,tbl_company.companyName as companyName, @count as totalSize from tbl_taxPayer 
	inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId
	inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
	inner join tbl_ward on tbl_ward.id = tbl_street.wardId
	where tbl_ward.lcdaId = @lcdaId
	ORDER BY tbl_company.companyName desc
                 OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
end
