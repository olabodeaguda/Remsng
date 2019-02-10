CREATE TABLE [dbo].[tbl_prepayment](
	[id] [bigint] NOT NULL PRIMARY KEY IDENTITY(1,1),
	[taxpayerId] [uniqueidentifier] NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[prepaymentStatus] [varchar(50)] NOT NULL default 'ACTIVE',
	[datecreated] [datetime] NOT NULL);

GO

alter table tbl_demandNoticePaymentHistory add IsWaiver bit not null DEFAULT 0;

GO

