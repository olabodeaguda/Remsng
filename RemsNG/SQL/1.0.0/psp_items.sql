IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_item') AND type IN (N'U'))			  
			  CREATE table tbl_item
			  (
				id uniqueidentifier NOT NULL PRIMARY KEY,
				itemDescription varchar(200) not null,
				itemStatus varchar(20) not null default 'ACTIVE',
				lcdaId uniqueidentifier not null foreign key references tbl_lcda(id),
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime
			  )
GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_createitem') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_createitem
  GO
  CREATE procedure sp_createitem(
	@itemDescription varchar(250),
	@itemStatus varchar(100),
	@lcdaId uniqueidentifier
  )
  as
BEGIN
		declare @msg varchar(250);
		declare @success bit;
		if exists(select * from tbl_item where itemDescription = @itemDescription and lcdaId = @lcdaId)
		begin
			set @msg = 'item description already exist';
			set @success = 0
		end
		else
		begin
			insert into tbl_item(id,itemDescription,itemStatus,lcdaId) values(NEWID(),@itemDescription,@itemStatus,@lcdaId)

			if @@ROWCOUNT > 0
			begin
				set @msg = @itemDescription+' has been added successfully';
				set @success = 1;
			end
			else
			begin
				set @msg = 'An error occur while adding the record. Please contact administrator or try again';
				set @success = 0
			end
		end

		select NEWID() as id,@msg as msg,@success as success
END
GO
IF EXISTS(SELECT *
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'sp_updateItem') AND type IN (N'P', N'PC'))
DROP PROCEDURE sp_updateItem
GO
CREATE procedure sp_updateItem(
	@id uniqueidentifier,
	@itemDescription varchar(250)
  )
  as
  begin
		declare @msg varchar(250);
		declare @success bit;
		if not exists(select * from tbl_item where id=@id)
			begin
				set @msg = 'Item does not exist';
				set @success = 0
			end
		else
			begin
				update tbl_item set itemDescription = @itemDescription where id=@id;
				if @@ROWCOUNT > 0
				begin
					set @msg = @itemDescription+' has been updated successfully';
					set @success = 1;
				end
				else
				begin
					set @msg = 'An error occur while adding the record. Please contact administrator or try again';
					set @success = 0
				end
			end

		select NEWID() as id,@msg as msg,@success as success
  end
GO
IF EXISTS(SELECT *
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'sp_changeItemstatus') AND type IN (N'P', N'PC'))
DROP PROCEDURE sp_changeItemstatus
GO
CREATE procedure sp_changeItemstatus(
	@id uniqueidentifier,
	@changeStatus varchar(250)
  )
  as
  begin
		declare @msg varchar(250);
		declare @success bit;
		if not exists(select * from tbl_item where id=@id)
			begin
				set @msg = 'Item does not exist';
				set @success = 0
			end
		else
			begin
				update tbl_item set itemStatus = @changeStatus where id=@id;
				if @@ROWCOUNT > 0
				begin
					set @msg = 'item status has been updated successfully';
					set @success = 1;
				end
				else
				begin
					set @msg = 'An error occur while adding the record. Please contact administrator or try again';
					set @success = 0
				end
			end

		select NEWID() as id,@msg as msg,@success as success
  end
GO
IF EXISTS(SELECT *
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'sp_ActiveItemByLcdaId') AND type IN (N'P', N'PC'))
DROP PROCEDURE sp_ActiveItemByLcdaId
GO
	CREATE procedure sp_ActiveItemByLcdaId
	(
		@lcdaId uniqueidentifier
	)
	as
	begin
		select * from tbl_item where lcdaId = @lgdaId and itemStatus = 'ACTIVE'		
	end
GO
