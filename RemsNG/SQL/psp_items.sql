IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_item') AND type IN (N'U'))			  
			  CREATE table tbl_item
			  (
				id uniqueidentifier NOT NULL PRIMARY KEY,
				itemDescription varchar(200) not null,
				itemStatus varchar(20) not null default 'ACTIVE',
				lcdaId uniqueidentifier not null foreign key references tbl_lcda(id),
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime
			  )
GO

