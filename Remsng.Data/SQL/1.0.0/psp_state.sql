IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_state') AND type IN (N'U'))
			  CREATE TABLE	tbl_state
			  (
			  id uniqueidentifier not null primary key,
			  countryId uniqueidentifier not null foreign key references tbl_country(id),
			  stateCode varchar(50) not null unique,
			  stateName varchar(100) not null unique
			  )