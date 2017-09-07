

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_street') AND type IN (N'U'))
CREATE TABLE tbl_street
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	wardId uniqueidentifier not null foreign key references tbl_ward(id),
	streetName varchar(200) NOT NULL,
	numberOfHouse int,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO

