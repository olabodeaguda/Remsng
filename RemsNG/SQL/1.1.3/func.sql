alter FUNCTION dbo.GetItemAmount (@billNumber varchar(250))
returns decimal(18,2)
begin
	DECLARE @price decimal(18,2); 
	select @price = sum(itemAmount) from tbl_demandNoticeItem where billingNo = @billNumber and itemStatus='Paid';
	return @price;
end