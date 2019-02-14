IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_bank') AND type IN (N'U'))
			  CREATE TABLE	tbl_bank
			  (
				  id uniqueidentifier not null primary key,
				  bankName varchar(100) not null,
				  dateCreated datetime default getDate()
			  )
GO
if not exists(select * from tbl_bank where bankName='Skye Bank')
	insert into tbl_bank(id,bankName,dateCreated) values(newid(),'Skye Bank',getdate());
if not exists(select * from tbl_bank where bankName='Eco Bank')
	insert into tbl_bank(id,bankName,dateCreated) values(newid(),'Eco Bank',getdate());
if not exists(select * from tbl_bank where bankName='GT Bank')
	insert into tbl_bank(id,bankName,dateCreated) values(newid(),'GT Bank',getdate());



