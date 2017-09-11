﻿

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_domain') AND type IN (N'U'))
CREATE TABLE tbl_domain
(
	id uniqueidentifier NOT NULL PRIMARY KEY ,
	domainName varchar(250) NOT NULL,
	domainCode varchar(20) NOT NULL UNIQUE,
	datecreated datetime NOT NULL default getdate(),
	addressId uniqueidentifier
)
GO


IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_userdomain') AND type IN (N'U'))
CREATE TABLE tbl_userdomain
(
	userid uniqueidentifier NOT NULL foreign key references tbl_user(id) ,
	domainid uniqueidentifier NOT NULL foreign key references tbl_domain(id)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addUserDomain') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addUserDomain
  GO

  CREATE PROCEDURE sp_addUserDomain
  (
	@userId uniqueidentifier,@domainId uniqueidentifier
  )
  as
  BEGIN
	declare @msg varchar(100);
	declare @success bit = 0;

	if exists(select * from tbl_userDomain where userId=@userId and domainId=@domainId)
	BEGIN
		SET @msg = 'user have already been tie to te selected domain';
		set @success = 0;
	END
	ELSE
	BEGIN
		INSERT INTO tbl_userDomain(userId,domainId) values(@userId,@domainId);
		if @@RowCount > 0
		begin
			set @msg = 'User have been added to the selected domain';
			set @success = 1
		end
	END
	select newID() as id, @msg as msg, @success as success;
  END
  GO
