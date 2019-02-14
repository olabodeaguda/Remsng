
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_taxpayerCategory') AND type IN (N'U'))
CREATE TABLE tbl_taxpayerCategory
(
	id uniqueidentifier,
	taxpayerCategoryName varchar(100) not null PRIMARY KEY,
	lcdaId uniqueidentifier not null unique,
	createdBy varchar(100) not null,
	dateCreated datetime not null default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO


