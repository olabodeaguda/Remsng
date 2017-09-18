
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


