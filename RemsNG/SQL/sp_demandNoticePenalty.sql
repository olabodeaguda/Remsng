IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticePenalty') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandNoticePenalty
			  (
				  id uniqueidentifier not null primary key,
				  billingNo varchar(50) not null,
				  taxpayerId uniqueidentifier not null,
				  totalAmount decimal(18,2) not null,
				  amountPaid decimal(18,2) not null,
				  itemId uniqueidentifier not null,
				  originatedYear int not null,
				  billingYear int not null,
				  arrearsStatus varchar(50) default 'PENDING',
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime				  
			  )
GO


