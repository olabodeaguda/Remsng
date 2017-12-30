IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_itempenalty') AND type IN (N'U'))			  
			  CREATE table tbl_itempenalty
			  (
				id uniqueidentifier NOT NULL PRIMARY KEY,
				itemId uniqueidentifier not null foreign key references tbl_item(id),
				isPercentage bit not null default 0,
				penaltyStatus varchar(20) not null default 'ACTIVE',
				amount decimal(12,2) not null default 0.0,
				duration varchar(20) NOT NULL,
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime
			  )
GO
