

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_street') AND type IN (N'U'))
CREATE TABLE tbl_street
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	wardId uniqueidentifier not null foreign key references tbl_ward(id),
	streetName varchar(200) NOT NULL,
	numberOfHouse int,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime,
	streetStatus varchar(250) NOT NULL default 'ACTIVE',
	streetDescription varchar(250)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addStreet') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addStreet
GO
CREATE procedure sp_addStreet
(
	@id uniqueidentifier,
	@wardId uniqueidentifier,
	@streetName varchar(250),
	@numberOfHouse int,
	@createdBy varchar(100),
	@streetDescription varchar(250)
)
as
begin
	declare @msg varchar(100);
	declare @success bit;

	if exists(select * from tbl_street where wardId = @wardId and streetName = @streetName)
	begin
		set @msg = 'Street already exist';
		set @success = 0;
	end
	else
	begin
		insert into tbl_street(id,wardId,streetName,numberOfHouse,createdBy,streetDescription,streetStatus) 
		values(NEWID(),@wardId,@streetName,@numberOfHouse,@createdBy,@streetDescription,'ACTIVE')
		if @@ROWCOUNT > 0
		begin
			set @msg = @streetName+' has been added successfully';
			set @success = 1;
		end
	end

	select newid() as id,@msg as msg,@success as success
end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateStreet') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateStreet
GO

create procedure sp_updateStreet
(
	@id uniqueidentifier,
	@wardid uniqueidentifier,
	@streetName varchar(250),
	@numberOfHouse int,
	@lastmodifiedby varchar(100),
	@streetDescription varchar(250)
)
as
begin
	declare @msg varchar(100);
	declare @success bit;
	
	if not exists(select * from tbl_street where id=@id)
	begin
		set @msg = @streetName +' does not exist';
		set @success = 0;
	end
	else
	begin
		update tbl_street set wardId = @wardid, streetName = @streetName, numberOfHouse = @numberOfHouse ,
		 lastmodifiedby = @lastmodifiedby ,lastModifiedDate = GETDATE(),streetDescription=@streetDescription where id = @id;

		 if @@ROWCOUNT > 0
		 begin
			set @msg = 'Update was successful';
			set @success = 1;
		 end
		 else
		 begin
			set @msg = 'An error occur. Please try again or contact administrator';
			set @success = 0;
		 end

	end
		select newid() as id,@msg as msg,@success as success
end
GO
	
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_StreetchangeStatus') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_StreetchangeStatus
GO
create procedure sp_StreetchangeStatus(
	@id uniqueidentifier,
	@streetStatus varchar(50)
)
as
begin
	declare @msg varchar(100);
	declare @success bit;
	update tbl_street set streetStatus = @streetStatus where id=@id

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Status has been updated successfully';
		set @success = 1;
	end
	else
	begin
		set @msg = 'An error occur.Please try again or contact your administrator';
		set @success = 0;
	end
		select newid() as id,@msg as msg,@success as success
end
go
