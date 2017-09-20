﻿

IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_ward') AND type IN (N'U'))
CREATE TABLE tbl_ward
(
	id uniqueidentifier NOT NULL PRIMARY KEY,
	wardName varchar(200) NOT NULL,
	lcdaId uniqueidentifier NOT NULL foreign key references tbl_lcda(id),
	wardStatus varchar(100) NOT NULL DEFAULT 'ACTIVE',
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO
