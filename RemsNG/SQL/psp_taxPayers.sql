
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_taxPayer') AND type IN (N'U'))
			  CREATE TABLE tbl_taxPayer
			  (
				id UNIQUEIDENTIFIER not null primary key,
				companyName varchar(200) not null unique,
				lcdaId uniqueidentifier foreign key references tbl_lcda(id),
				sectorId uniqueidentifier not null,
				addressId uniqueidentifier not null,
				categoryId uniqueidentifier not null,
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime
			  )
			  GO
