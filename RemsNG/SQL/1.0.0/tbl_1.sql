

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_role') AND type IN (N'U'))
CREATE TABLE tbl_role
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	roleName varchar(100) not null,
	domainId uniqueidentifier not null foreign key references tbl_domain(id)
)
GO

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_userRole') AND type IN (N'U'))
CREATE TABLE tbl_userRole
(
	userid uniqueidentifier not null foreign key references tbl_users(id),
	roleid uniqueidentifier not null foreign key references tbl_userRole(id)
)
GO

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_permission') AND type IN (N'U'))
CREATE TABLE tbl_permission
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	permissionName varchar(100) not null unique
)
GO

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_rolePermission') AND type IN (N'U'))
CREATE TABLE tbl_rolePermission
(
	roleId uniqueidentifier not null foreign key references tbl_role(id),
	permissionId uniqueidentifier not null foreign key references tbl_permission(id)
)
GO


