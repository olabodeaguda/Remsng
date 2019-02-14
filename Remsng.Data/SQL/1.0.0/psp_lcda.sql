
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_lcda') AND type IN (N'U'))
CREATE TABLE tbl_lcda
(
	id uniqueidentifier not null primary key,
	domainId uniqueidentifier not null,
	lcdaName varchar(250) not null,
	lcdaCode varchar(100) not null,
	addressId uniqueidentifier not null ,
	stateId uniqueidentifier not null,
	country uniqueidentifier not null,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime,
	lcdaStatus varchar(50) not null default 'NOT_ACTIVE'
)
GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserDomainRolesByDomainId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserDomainRolesByDomainId
  GO
  create procedure sp_getUserDomainRolesByDomainId(
    @userId uniqueidentifier, @domainId uniqueidentifier
  )
  as
  begin
		select tbl_role.*,tbl_lcda.lcdaName as domainName from tbl_role
		inner join tbl_userRole on tbl_userRole.roleid = tbl_role.id
		inner join tbl_lcda on tbl_lcda.id = tbl_role.domainId
		where tbl_userRole.userid = @userId and tbl_role.domainId = @domainId
  end
  GO
  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_removeUserFromLCDA') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_removeUserFromLCDA
  GO
  create procedure sp_removeUserFromLCDA(
    @userId uniqueidentifier, @domainId uniqueidentifier
  )
  as
  begin
		declare @msg varchar(100);
		declare @success bit;
		delete from tbl_userlcda where tbl_userlcda.lgdaid = @domainId and tbl_userlcda.userid = @userId;
		if @@ROWCOUNT > 0
		begin
			set @msg = 'User has been successfully removed';
			set @success = 1;
		end
		else
		begin
			set @msg = 'User has not been remove from LCDA.Please try again or contact administrator';
			set @success = 1;
		end 
		select NEWID() as id, @msg as msg, @success as success
  end
  GO
