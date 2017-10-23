
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_company') AND type IN (N'U'))
CREATE TABLE tbl_company
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	companyName varchar(100) not null,
	streetId uniqueidentifier foreign key references tbl_street(id),
	sectorId uniqueidentifier foreign key references tbl_sector(id),
	addressId uniqueidentifier foreign key references tbl_address(id),
	categoryId uniqueidentifier foreign key references tbl_lcda(id),
	companyStatus varchar(100) not null default 'ACTIVE',
	createdBy varchar(100) NOT NULL,
	dateCreated datetime NOT NULL DEFAULT GETDATE(),
	lastmodifiedby varchar(100) NULL,
	lastModifiedDate [datetime] NULL
)
GO



