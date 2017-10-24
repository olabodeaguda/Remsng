
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_address') AND type IN (N'U'))
			  CREATE TABLE	tbl_address
			  (
				  id uniqueidentifier not null primary key,
				  addressnumber varchar(100) not null,
				  streetId uniqueidentifier not null,
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )

GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_lcdaAddressByOwnerId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_lcdaAddressByOwnerId
  GO
  create PROCEDURE sp_lcdaAddressByOwnerId
(
	@ownerId uniqueidentifier,
	@lcdaId uniqueidentifier
)
AS
BEGIN
	select tbl_address.*, tbl_street.streetName from tbl_address
	inner join tbl_street on tbl_street.id = tbl_address.streetId
	where tbl_address.ownerId = @ownerId and tbl_address.lcdaid = @lcdaId
	order by tbl_street.streetName 
END
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addAddress') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addAddress
GO
create PROCEDURE sp_addAddress
(
	@id uniqueidentifier,
	@addressnumber varchar(100),
	@streetId uniqueidentifier,
	@ownerId uniqueidentifier,
	@lcdaId uniqueidentifier,
	@createdBy varchar(100)
)
AS
BEGIN
		declare @msg varchar(200);
		declare @success bit;

		if exists(select * from tbl_address where addressnumber = @addressnumber and streetId = @streetId and ownerId=@ownerId)
		begin
			set @msg = 'Address already exist';
			set @success = 0;
		end
		else
		begin
			insert into tbl_address(id,addressnumber,streetId,createdBy,ownerId,dateCreated,lcdaid) 
			values(@id,@addressnumber,@streetId,@createdBy,@ownerId,GETDATE(),@lcdaId);
			
			if @@ROWCOUNT > 0
			begin
				set @msg = 'Address has been added successfully';
				set @success = 1;
			end
		end
		select NEWID() as id, @msg as msg, @success as success;
END
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateAddress') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateAddress
GO
create PROCEDURE sp_updateAddress
(
	@addressnumber varchar(100),
	@streetId uniqueidentifier,
	@ownerId uniqueidentifier,
	@lcdaId uniqueidentifier,
	@lastmodifiedby varchar(100),
	@id uniqueidentifier
)
AS
BEGIN
		declare @msg varchar(200);
		declare @success bit;

		if not exists(select * from tbl_address where id = @id)
		begin
			set @msg = 'Address does not exist';
			set @success = 0;
		end
		else
		begin
		update tbl_address set addressnumber= @addressnumber,streetId=@streetId, lastmodifiedby = @lastmodifiedby,
		lastModifiedDate = GETDATE() where id=@id
			
			if @@ROWCOUNT > 0
			begin
				set @msg = 'Address have been added success';
				set @success = 1;
			end
		end
		select NEWID() as id, @msg as msg, @success as success;
END
GO

