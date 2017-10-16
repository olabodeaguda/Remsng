
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_permission') AND type IN (N'U'))
CREATE TABLE tbl_permission
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	permissionName varchar(100) not null unique,
	permissionDescription varchar(Max)
)
GO

IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CREATE_USER')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CREATE_USER');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'APROVE_USER')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'APROVE_USER');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_USER')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_USER');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CHANGE_STATUS')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CHANGE_STATUS');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CREATE_DOMAIN')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CREATE_DOMAIN');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ASSIGN_DOMAIN')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ASSIGN_DOMAIN');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_PROFILE')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_PROFILE');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ADD_PROFILE')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ADD_PROFILE');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_PROFILE')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_PROFILE');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CHANGE_USER_PWD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CHANGE_USER_PWD');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_WARD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_WARD');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_WARD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_WARD');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_ROLES')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_ROLES');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ADD_ROLES')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ADD_ROLES');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_ROLES')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_ROLES');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CHANGE_STATUS')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CHANGE_STATUS');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ASSIGN_ROLES')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ASSIGN_ROLES');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'MANAGE_PERMISSION')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'MANAGE_PERMISSION');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_ROLES')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_ROLES');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_REMOVE')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_REMOVE');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_PERMISSIONS')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_PERMISSIONS');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_LCDA')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_LCDA');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ADD_LCDA')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ADD_LCDA');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_LCDA')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_LCDA');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CHANGE_LCDA_STATUS')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CHANGE_LCDA_STATUS');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_DOMAIN')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_DOMAIN');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CREATE_DOMAIN')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CREATE_DOMAIN');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_DOMAIN')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_DOMAIN');
GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'CHANGE_STATUS')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'CHANGE_STATUS');

GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'GET_WARD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'GET_WARD');

GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'UPDATE_WARD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'UPDATE_WARD');

GO
IF NOT EXISTS(SELECT * FROM tbl_permission where permissionName = 'ADD_WARD')
INSERT INTO tbl_permission(id,permissionName) values(newid(),'ADD_WARD');

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getRolePermission') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getRolePermission
GO
CREATE PROCEDURE sp_getRolePermission
(
	@roleId uniqueidentifier
)
AS
BEGIN
	select tbl_permission.* from tbl_permission 
	inner join tbl_rolePermission  on tbl_rolePermission.permissionId = tbl_permission.id
	where tbl_rolePermission.roleId = @roleId

END
GO


IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getRolePermissionPaginated') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getRolePermissionPaginated
GO
CREATE PROCEDURE sp_getRolePermissionPaginated
(
	@roleId uniqueidentifier,
	@pageNum int,
	@pageSize int
)
AS
BEGIN
IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;
	select tbl_permission.* from tbl_permission 
	inner join tbl_rolePermission  on tbl_rolePermission.permissionId = tbl_permission.id
	where tbl_rolePermission.roleId = @roleId
	ORDER BY tbl_permission.permissionName desc
                 OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;

END
GO


IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getPermissionNotInRole') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getPermissionNotInRole
GO
CREATE PROCEDURE sp_getPermissionNotInRole
(
	@roleId uniqueidentifier
)
AS
BEGIN
	select tbl_permission.* from tbl_permission 
	where id not in(select permissionId from tbl_rolePermission where roleId = @roleId) order by tbl_permission.permissionName;
END
GO

