IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_error') AND type IN (N'U'))
			  CREATE TABLE	tbl_error
			  (
				  id uniqueidentifier not null primary key,
				  ownerId uniqueidentifier not null,
				  errorType varchar(100) not null,
				  errorvalue varchar(250) not null,
				  dateCreated datetime default getDate()
			  )
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addError') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addError
GO
create procedure sp_addError
(
	@ownerId uniqueidentifier,
	@errortype varchar(100),
	@errorvalue varchar(250)
)
as
begin
declare @msg varchar(250);
declare @success bit;
	insert into tbl_error(id,errorType,errorvalue,dateCreated,ownerId) values(newid(),@errorType,@errorvalue,getdate(),@ownerId);
	if @@Rowcount > 0
	begin
		set @msg = 'Request was successful';
		set @success = 1;
	end
	else
	begin
		set @msg = 'Request failed';
		set @success = 0;
	end

	select newid() as id, @msg as msg, @success as success
end
GO

