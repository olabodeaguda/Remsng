

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_domain') AND type IN (N'U'))
CREATE TABLE tbl_domain
(
	id uniqueidentifier NOT NULL PRIMARY KEY ,
	domainName varchar(250) NOT NULL,
	domainCode varchar(20) NOT NULL UNIQUE,
	datecreated datetime NOT NULL default getdate(),
	addressId uniqueidentifier,
	domainStatus varchar(100) NOT NULL,
	domainType varchar(100) NOT NULL default 'others'
)
GO

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_userdomain') AND type IN (N'U'))
CREATE TABLE tbl_userdomain
(
	userid uniqueidentifier NOT NULL foreign key references tbl_users(id) ,
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
  
  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserDomainByUsername') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserDomainByUsername
  GO
  CREATE PROCEDURE sp_getUserDomainByUsername(
  @username varchar(100)
  )
  AS
  BEGIN
		select tbl_domain.* from tbl_domain
		inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id
		inner join tbl_userlcda on tbl_userlcda.lgdaid = tbl_lcda.id
		inner join tbl_users on tbl_users.id = tbl_userlcda.userid
		where tbl_users.username =  @username
  END
  GO
   IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserDomainByUserId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserDomainByUserId
  GO
  CREATE PROCEDURE sp_getUserDomainByUserId(
  @userid uniqueidentifier
  )
  AS
  BEGIN
		select tbl_domain.* from tbl_domain
		inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id
		inner join tbl_userlcda on tbl_userlcda.lgdaid = tbl_lcda.id
		inner join tbl_users on tbl_users.id = tbl_userlcda.userid
		where tbl_users.username =  @userid and tbl_domain.domainStatus = 'Active'
  END
  GO
  