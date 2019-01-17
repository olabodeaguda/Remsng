alter table tbl_demandnotice add isUnbilled bit not null DEFAULT 0;
alter table tbl_demandNoticeTaxpayers add isUnbilled bit not null DEFAULT 0;