
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_role') AND type IN (N'U'))
CREATE TABLE tbl_role
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	roleName varchar(100) not null,
	domainId uniqueidentifier not null foreign key references tbl_domain(id),
	roleStatus varchar(100) not null default 'ACTIVE'
)
GO

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_userRole') AND type IN (N'U'))
CREATE TABLE tbl_userRole
(
	userid uniqueidentifier not null foreign key references tbl_users(id),
	roleid uniqueidentifier not null foreign key references tbl_Role(id)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_createRole') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_createRole
  GO
  create procedure sp_createRole
  (	
		@username varchar(100),
		@id uniqueidentifier,
		@roleName varchar(100),
		@domainId uniqueidentifier
  )
  as
  begin
	declare @msg varchar(250);
	declare @success bit = 0;

	if exists(select * from tbl_role where roleName=@rolename and domainId=@domainId)
	begin
		set @msg = 'Role arleady exist';
		set @success = 0;
	end
	else 
	begin
		insert into tbl_role(id,roleName,domainId) values(@id,@roleName,@domainId);
		IF @@ROWCOUNT > 0
		BEGIN 
			set @msg = 'Role have been added successfully';
			set @success = 1;
		END
	end
	select @msg as msg,@success as success;
  end

  GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateRole') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateRole
  GO
  create procedure sp_updateRole
  (	
		@id uniqueidentifier,
		@roleName varchar(100),
		@domainId uniqueidentifier
  )
  as
  begin
	declare @msg varchar(250);
	declare @success bit = 0;

	if exists(select * from tbl_role where roleName=@rolename and domainId=@domainId)
	begin
		set @msg = 'Role already exist';
		set @success = 0;
	end
	else 
	begin
		update  tbl_role set roleName=@roleName,domainId = @domainId where id=@id;

		IF @@ROWCOUNT > 0
		BEGIN 
			set @msg = 'Role has been updated successfully';
			set @success = 1;
		END
	end
	select @msg as msg,@success as success;
end
GO
  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_changeRolestatus') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_changeRolestatus
  GO
  create procedure sp_changeRolestatus
  (	
		@id uniqueidentifier,
		@roleStatus varchar(50)
  )
  as
  begin
	declare @msg varchar(250);
	declare @success bit = 0;

	if not exists(select * from tbl_role where id=@id)
	begin
		set @msg = 'Role does not exist';
		set @success = 0;
	end
	else 
	begin
		update  tbl_role set roleStatus=@roleStatus where id=@id;

		IF @@ROWCOUNT > 0
		BEGIN 
			set @msg = 'Role status has been updated successfully';
			set @success = 1;
		END
	end
	select @msg as msg,@success as success;
end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserRoleByUsername') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserRoleByUsername
  GO
  CREATE PROCEDURE sp_getUserRoleByUsername(
  @username varchar(100)
  )
  AS
  BEGIN
		select tbl_role.* from tbl_role
		inner join tbl_userRole on tbl_userRole.roleid = tbl_role.id
		inner join tbl_users on tbl_users.id = tbl_userRole.userid
		where tbl_users.username = @username
  END
GO




