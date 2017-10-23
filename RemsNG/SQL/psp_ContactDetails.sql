
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_contactDetail') AND type IN (N'U'))
			CREATE TABLE tbl_contactDetail
			(
				id uniqueidentifier primary key,
				ownerId uniqueidentifier not null,
				contactValue varchar(100),
				contactType varchar(100),
				createdBy varchar(100) not null,
				dateCreated datetime default getDate(),
				lastmodifiedby varchar(100),
				lastModifiedDate datetime
			)
			GO

