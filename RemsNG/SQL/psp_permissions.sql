
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_permission') AND type IN (N'U'))
CREATE TABLE tbl_permission
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	permissionName varchar(100) not null unique
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


