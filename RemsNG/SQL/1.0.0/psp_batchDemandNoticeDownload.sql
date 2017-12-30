IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_batchDownloadRequest') AND type IN (N'U'))
CREATE TABLE tbl_batchDownloadRequest
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	batchNo varchar(100),
	requestStatus varchar(100),
	lcdaId uniqueidentifier not null,
	batchFileName varchar(200),
	createdby varchar(100),
	dateCreated datetime NOT NULL DEFAULT GETDATE(),
	lastmodifiedby varchar(100) NULL,
	lastModifiedDate [datetime] NULL
)
GO

