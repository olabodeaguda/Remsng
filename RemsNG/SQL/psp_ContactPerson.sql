
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_taxPayers') AND type IN (N'U'))
CREATE TABLE tbl_contactPerson
(
	id uniqueidentifier not null primary key,
	surname varchar(100) not null,
	firstname varchar(100) not null,
	lastname varchar(100),
	taxPayerId uniqueidentifier foreign key references tbl_TaxPayer(id),
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO
