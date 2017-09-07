
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_address') AND type IN (N'U'))
			  CREATE TABLE	tbl_address
			  (
				  id uniqueidentifier not null primary key,
				  addressnumber varchar(100) not null,
				  streetId uniqueidentifier not null,
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )