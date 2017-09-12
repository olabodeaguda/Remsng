

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_users') AND type IN (N'U'))
CREATE TABLE tbl_users
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	email varchar(50) NOT NULL unique,
	passwordHash varchar(250),
	securityStamp varchar(250),
	lockedOutEndDateUTC date,
	lockedoutenabled bit default 0,
	username varchar(100) unique,
	userStatus varchar(50),
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime,
	lastname varchar(100),
	surname varchar(100),
	firstname varchar(100)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_createUser') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_createUser
GO
	CREATE PROCEDURE sp_createUser
	(
		@id uniqueidentifier,
		@email varchar(50),
		@passwordHash varchar(250),
		@securityStamp varchar(250),
		@username varchar(100),
		@userstatus varchar(50),
		@createdBy varchar(100),
	@lastname varchar(100),
	@surname varchar(100),
	@firstname varchar(100)
	)
	as
	BEGIN
		declare @msg varchar(100);
		declare @success bit = 0;
		if exists(select * from tbl_users where username = @username)
			BEGIN
				set @success = 0;
				set @msg = @username+' already exist'
			END
		ELSE
			BEGIN
				INSERT INTO tbl_users(id,email,passwordHash,securityStamp,username,userstatus,createdBy,lastname,surname,firstname) 
				values(@id,@email,@passwordHash,@securityStamp,@username,@userstatus,@createdBy,@lastname,@surname,@firstname);
				IF @@RowCount > 0
				BEGIN
					SET @success = 1;
					set @msg = @username+' has been created successfully';
				END
			END

		select @msg as msg,@success as success;
	END
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateUser') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateUser
GO
	CREATE PROCEDURE sp_updateUser
	(
		@id uniqueidentifier,
		@email varchar(50),
		@passwordHash varchar(250),
		@securityStamp varchar(250),
		@username varchar(100),
		@userstatus varchar(50),
		@createdBy varchar(100),
	@lastname varchar(100),
	@surname varchar(100),
	@firstname varchar(100)
	)
	as
	BEGIN
		declare @msg varchar(100);
		declare @success bit = 0;
		if not exists(select * from tbl_users where username = @username)
			BEGIN
				set @success = 0;
				set @msg = @username+' does not exist'
			END
		ELSE
			BEGIN
				update tbl_users set lastname=@lastname,surname=@surname,firstname=@firstname, email=@email, passwordHash=@passwordHash,
				securityStamp=@securityStamp, username=@username, userstatus=@userstatus, lastmodifiedby=@createdBy,
				lastModifiedDate = GETDATE() where id=@id;
				IF @@RowCount > 0
				BEGIN
					SET @success = 1;
					set @msg = @username+' has been updated successfully';
				END
			END

		select @msg as msg,@success as success;
	END
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_changePwd') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_changePwdByOldPwd
  GO
  CREATE PROCEDURE sp_changePwdByOldPwd
  (
	@username varchar(100),
	@oldPwd uniqueidentifier,
	@newPwd uniqueidentifier,
	@createdBy varchar(100)
  )
  AS
  BEGIN
	  declare @msg varchar(250);
	  declare @success bit = 0;

	  if not exists(select * from tbl_users where username = @username)
		begin
			set @msg = 'Username does not exist';
			set @success = 0;
		end
	 else
		begin
			if not exists(select * from tbl_users where username=@username and passwordHash=@oldPwd)
			begin
				set @msg = 'Password is incorrect';
				set @success = 0;
			end
			else
			begin
				update tbl_user set passwordHash = @oldPwd where username=@username;
				if @@Rowcount > 0
				begin
					set @msg = 'Password have been changed successfully';
					set @success = 1;
				end
			end
	 end
	  select @msg as msg, @success as success;
  END

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_changePwd') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_changePwd
  GO

  CREATE PROCEDURE sp_changePwd
  (
	@username varchar(100),
	@newPwd uniqueidentifier,
	@createdBy varchar(100)
  )
  as
  begin
	 declare @msg varchar(250);
	  declare @success bit = 0;

	  if not exists(select * from tbl_users where username = @username)
		  begin
				set @msg = 'Username does not exist';
				set @success = 0;
		  end
	 else
		begin
			update tbl_user set passwordHash = @newPwd where username=@username;
				if @@Rowcount > 0
				begin
					set @msg = 'Password have been changed successfully';
					set @success = 1;
				end
		end
	select @msg as msg,@success as success;
  end

  GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_confirmSecurityQuestions') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_confirmSecurityQuestions
  GO
  CREATE PROCEDURE sp_confirmSecurityQuestions
  (
	@username varchar(100),
	@securityStamp varchar(100)
  )
  as
  BEGIN
      declare @msg varchar(250);
	  declare @success bit = 0;
		if not exists(select * from tbl_users where username=@username)
		begin
			set @msg = 'username does not exist';
			set @success = 0;
		end
	  else 
	  begin
		  if exists(select * from tbl_users where username=@username and securityStamp = @securityStamp)
		  begin
				set @msg = 'Answer to the question is incorrect';
				set @success = 0;
		  end
		  else
		  begin
				set @msg = 'Security question is correct';
				set @success = 1;
		  end	
	  end
	  select @msg as msg,@success as success;
  END
  GO
   IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_changeUserStatus') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_changeUserStatus
  GO
  CREATE PROCEDURE sp_changeUserStatus
  (
	@id uniqueidentifier,
	@userStatus varchar(50)
  )
  as
  BEGIN
	  declare @msg varchar(250);
	  declare @success bit = 0;
	IF NOT EXISTS(select * from tbl_users where id=@id)
	begin
		set @msg = 'User does not exist';
		set @success = 0;
	end
	else
	begin
		update tbl_users set  userStatus =@userstatus where id=@id
		if @@rowcount > 0
		begin
			set @msg = 'User Status have been updated successsfully';
			set @success = 1;
		end
	end

	select @msg as msg,@success as success;
  END

  GO

