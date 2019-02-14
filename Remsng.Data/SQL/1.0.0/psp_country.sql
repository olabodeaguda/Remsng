IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_country') AND type IN (N'U'))

			  CREATE TABLE	tbl_country
			  (
			  id uniqueidentifier not null primary key,
			  countryName varchar(100) not null unique,
			  countryCode varchar(50) not null unique
			  )