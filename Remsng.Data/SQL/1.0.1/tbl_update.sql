alter table tbl_demandNoticeTaxpayers add demandNoticeStatus varchar(100) NOT NULL default 'PENDING'
go
insert into tbl_permission(id,permissionName,permissionDescription) values(NEWID(),'APPROVE_PAYMENT','Reciept aapproval');
go

