

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_ward') AND type IN (N'U'))
CREATE TABLE tbl_ward
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	wardName varchar(200) NOT NULL,
	lcdaId uniqueidentifier NOT NULL foreign key references tbl_lcda(id),
	wardStatus varchar(100) NOT NULL DEFAULT 'ACTIVE',
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_wardByLcda') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_wardByLcda
GO

create procedure sp_wardByLcda
(@lcdaId uniqueidentifier)
as
begin
select tbl_ward.*,tbl_lcda.lcdaName from tbl_ward
inner join tbl_lcda on tbl_lcda.id = tbl_ward.lcdaId
where tbl_ward.lcdaId = @lcdaId
order by tbl_lcda.lcdaName asc
end

go

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_ward') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_ward
GO

create procedure sp_ward
(@lcdaId uniqueidentifier)
as
begin
select tbl_ward.*,tbl_lcda.lcdaName from tbl_ward
inner join tbl_lcda on tbl_lcda.id = tbl_ward.lcdaId
order by tbl_lcda.lcdaName asc
end
