
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_sector') AND type IN (N'U'))
CREATE TABLE tbl_sector
(
	id uniqueidentifier,
	sectorName varchar(100) not null,
	lcdaId uniqueidentifier not null foreign key references tbl_lcda(id),
	createdBy varchar(100) not null,
	dateCreated datetime not null default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_createSector') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_createSector
GO

create procedure sp_createSector
(
	@sectorName varchar(100),
	@lcdaId uniqueidentifier,
	@createdBy varchar(100))
as
begin
declare @msg varchar(250);
declare @success bit;
	if not exists(select * from tbl_lcda where id = @lcdaId)
	begin
		set @msg = 'lcda does not exist';
		set @success = 0
	end
	else if exists(select * from tbl_sector where lcdaId=@lcdaId and sectorName=@sectorName)
	begin
		set @msg = 'lcda already exist';
		set @success = 0
	end
	else
	begin
		insert into tbl_sector(id,sectorName,lcdaId,createdBy) values(NEWID(), @sectorName,@lcdaId,@createdBy);
		if @@ROWCOUNT > 0
		begin
			set @msg = 'Sector has been added successfully';
			set @success = 1;
		end
		else
		begin
			set @msg = 'An error occur while processing the transaction.Please try again pr contact administrator';
			set @success = 0
		end
	end
	select newid() as id, @msg as msg, @success as success
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateSector') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateSector
GO

create procedure sp_updateSector
(
	@id uniqueidentifier,
	@sectorName varchar(100),
	@lastmodifiedBy varchar(100))
as
begin
	declare @msg varchar(250);
	declare @success bit;
	if not exists(select * from tbl_sector where id = @id)
	begin
		set @msg = 'Sector does not exist';
		set @success = 0
	end
	else
	begin
		update tbl_sector set sectorName=@sectorName,lastmodifiedby=@lastmodifiedBy, lastModifiedDate=GETDATE() where id=@id;
		if @@ROWCOUNT > 0
		begin
			set @msg = 'Sector has been updated successfully';
			set @success = 1;
		end
		else
		begin
			set @msg = 'An error occur while processing the transaction.Please try again pr contact administrator';
			set @success = 0
		end
	end
	select newid() as id, @msg as msg, @success as success
end
GO



