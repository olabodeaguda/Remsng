
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_domain') AND type IN (N'U'))
CREATE TABLE tbl_domain
(
	id uniqueidentifier NOT NULL PRIMARY KEY ,
	domainName varchar(250) NOT NULL,
	domainCode varchar(20) NOT NULL UNIQUE,
	datecreated datetime default getdate(),
	addressId uniqueidentifier NOT NULL
)
GO

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
	username varchar(100) unique
)
GO

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


