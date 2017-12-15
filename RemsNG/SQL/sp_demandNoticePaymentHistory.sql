IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticePaymentHistory') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandNoticePaymentHistory
			  (
				  id uniqueidentifier not null primary key,
				  ownerId uniqueidentifier not null,
				  billingNumber varchar(100) not null,
				  amount decimal(10,2) not null default 0.0,
				  charges decimal(10,2) not null default 0.0,
				  paymentMode varchar(150) not null,
				  referenceNumber varchar(250) not null,
				  bankId uniqueidentifier not null,
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addDemandNoticePaymentHistory') AND type IN (N'P', N'PC'))
		  drop procedure sp_addDemandNoticePaymentHistory
GO



