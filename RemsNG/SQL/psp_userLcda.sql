
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_userlcda') AND type IN (N'U'))
CREATE TABLE tbl_userlcda
(
	userid uniqueidentifier NOT NULL foreign key references tbl_users(id) ,
	lcdaid uniqueidentifier NOT NULL foreign key references tbl_lcda(id)
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserLCDAByUsername') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserLCDAByUsername
  GO
  CREATE PROCEDURE sp_getUserLCDAByUsername(
  @username varchar(100)
  )
  AS
  BEGIN
		select tbl_lcda.* from tbl_lcda
		inner join tbl_userlcda on tbl_userlcda.lgdaid = tbl_lcda.id
		inner join tbl_users on tbl_users.id = tbl_userlcda.userid
		where tbl_users.username = @username
  END
  GO

  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getUserLCDAByuserId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getUserLCDAByuserId
  GO
  CREATE PROCEDURE sp_getUserLCDAByuserId(
	@id uniqueidentifier
  )
  AS
  BEGIN
		select tbl_lcda.* from tbl_lcda
		inner join tbl_userlcda on tbl_userlcda.lgdaid = tbl_lcda.id
		where tbl_userlcda.userid = @id
  END
  GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_unAssignUserDomainByuserId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_unAssignUserDomainByuserId
  GO
  create procedure sp_unAssignUserDomainByuserId
(
	@userId uniqueidentifier
)
as
begin
	select * from tbl_lcda where id not in(select lgdaid from tbl_userlcda where tbl_userlcda.userid = @userId)
end

