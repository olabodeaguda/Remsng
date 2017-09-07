
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_sector') AND type IN (N'U'))
CREATE TABLE sector
(
	id uniqueidentifier,
	sectorName uniqueidentifier not null unique,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO
