ALTER procedure [dbo].[sp_currentMaxBilling]
as 
begin
	declare @billingNo bigint;
	declare @success bit;
	set @success = 1;
	if (select count(*) from tbl_demandNoticeTaxpayers) < 1
		begin
			set @billingNo = 0;
		end
	else
		begin
			select @billingNo = Max(CAST(billingNumber AS bigint))  from tbl_demandNoticeTaxpayers;
		end
	select NEWID() as id,cast(@billingNo as varchar(20)) as msg, @success as success
end
go
