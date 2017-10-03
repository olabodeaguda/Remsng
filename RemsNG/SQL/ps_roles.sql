
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_role') AND type IN (N'U'))
CREATE TABLE tbl_role
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	roleName varchar(100) not null,
	domainId uniqueidentifier not null foreign key references tbl_lcda(id),
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
		set @msg = 'Role already exists';
		set @success = 0;
	end
	else 
	begin
		insert into tbl_role(id,roleName,domainId) values(@id,@roleName,@domainId);
		IF @@ROWCOUNT > 0
		BEGIN 
			set @msg = 'Role has been added successfully';
			set @success = 1;
		END
	end
	select NEWID() as id, @msg as msg,@success as success;
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
          WHERE object_id = OBJECT_ID(N'sp_getUserDomainRoleByUsername') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserDomainRoleByUsername
  GO
  CREATE PROCEDURE sp_getUserDomainRoleByUsername(
  @username varchar(100), @domainId uniqueidentifier
  )
  AS
  BEGIN
		select tbl_role.*,tbl_lcda.lcdaName as domainName from tbl_role
		inner join tbl_userRole on tbl_userRole.roleid = tbl_role.id
		inner join tbl_users on tbl_users.id = tbl_userRole.userid
		inner join tbl_lcda on tbl_lcda.id = tbl_role.domainId
		where tbl_users.username = @username and tbl_role.domainId = @domainId
  END
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getRoles') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getRoles
  GO
  CREATE PROCEDURE sp_getRoles(
  @pageSize int,
  @pageNum int
  )
  AS
  BEGIN
		IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;
		select tbl_role.*,tbl_lcda.lcdaName as domainName from tbl_role		
		inner join tbl_lcda on tbl_lcda.id = tbl_role.domainId 
		ORDER BY tbl_role.domainId desc
                 OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
  END
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
		select tbl_role.*, tbl_users.username,tbl_lcda.lcdaName from tbl_lcda
		inner join tbl_role on tbl_role.domainId = tbl_lcda.id
		inner join tbl_userlcda on tbl_userlcda.lgdaid = tbl_lcda.id
		inner join tbl_users on tbl_users.id = tbl_userlcda.userid
		where tbl_users.username = @username
  END
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getDomainRolesByUsername') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getDomainRolesByUsername
  GO
  CREATE PROCEDURE sp_getDomainRolesByUsername(
  @username varchar(100)
  )
  AS
  BEGIN
		select tbl_role.*,tbl_lcda.lcdaName from tbl_lcda
		inner join tbl_role on tbl_role.domainId = tbl_lcda.id
		Where tbl_role.domainId  
		in (select  tbl_userlcda.lgdaid from tbl_userlcda 
		inner join tbl_users on tbl_users.id = tbl_userlcda.userid where tbl_users.username = @username)
  END
GO


IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserDomainRolesByDomainId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserDomainRolesByDomainId
  GO
  CREATE PROCEDURE sp_getUserDomainRolesByDomainId(
  @id uniqueidentifier, @domainId uniqueidentifier
  )
  AS
  BEGIN
		select tbl_role.*,tbl_lcda.lcdaName as domainName from tbl_role
		inner join tbl_userRole on tbl_userRole.roleid = tbl_role.id
		inner join tbl_users on tbl_users.id = tbl_userRole.userid
		inner join tbl_lcda on tbl_lcda.id = tbl_role.domainId
		where tbl_users.id = @id and tbl_role.domainId = @domainId
  END
GO


