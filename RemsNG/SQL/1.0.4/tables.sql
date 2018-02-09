
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_cloudData') AND type IN (N'U'))
CREATE TABLE tbl_cloudData
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	domainId UNIQUEIDENTIFIER NOT NULL,
	dataTitle varchar(50) not null,
	syncStatus varchar(50) default 'NEW',
	jsonContent varchar(max) null,
	billingNumber varchar(50) not null,
	dateCreated datetime default getdate()
)
GO
CREATE UNIQUE INDEX uq_dataTitle_billingNumber
  ON tbl_cloudData(dataTitle, billingNumber);
GO
