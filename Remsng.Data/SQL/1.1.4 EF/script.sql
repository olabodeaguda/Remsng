update tbl_demandNoticeItem set DemandNoticeId = dn_taxpayersDetailsId;
go

  alter table tbl_DemandNoticeTaxpayers add Period int not null DEFAULT 0;

  go

  
  update r  set r.dn_taxpayersDetailsId = dnt.Id
  from tbl_demandNoticeItem as r
  inner join tbl_demandNoticeTaxpayers as dnt on r.billingNo = dnt.BillingNumber
