
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_images') AND type IN (N'U'))
CREATE TABLE tbl_images
(
	id uniqueidentifier not null primary key,
	imgFilename varchar(200) not null,
	ownerId uniqueidentifier not null,
	imgType varchar(50) not null,
	createdBy varchar(100) not null,
	dateCreated datetime default getDate(),
	lastmodifiedby varchar(100),
	lastModifiedDate datetime
)
GO
