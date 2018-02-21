create procedure sp_paymentReportSync
as
begin

 select top 100 CONCAT(tp.surname,' ',tp.firstname,' ',tp.lastname) as taxpayerName,dnPaymentHistory.*, bank.bankName
   from tbl_demandNoticePaymentHistory as dnPaymentHistory
  inner join tbl_taxPayer as tp on tp.id = dnPaymentHistory.ownerId
  inner join tbl_bank as bank on bank.id = dnPaymentHistory.bankId
  where syncStatus = 0

end

