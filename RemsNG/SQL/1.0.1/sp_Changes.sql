ALTER procedure [dbo].[sp_getBillingNumberTotalDue]
(
	@billingno varchar(50)
)
as
begin
	 select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus,tm.itemDescription,'ARREARS' as category
	  from tbl_demandNoticeArrears as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	 union
	  select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus,tm.itemDescription,'PENALTY' as category
	  from tbl_demandNoticePenalty as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
	  union
	  select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus,tm.itemDescription,'ITEMS' as category
	  from tbl_demandNoticeItem as dn
	 inner join tbl_item as tm on tm.id = dn.itemId
	  where billingNo = @billingno
end
go

ALTER procedure [dbo].[sp_CompanyBylcdaId]
(
	@lcdaId uniqueidentifier
)
as
begin
	select tbl_company.*,tbl_sector.sectorName as sectorName,tbl_taxpayerCategory.taxpayerCategoryName as categoryName,
	-1 as totalSize from tbl_company 
	left join tbl_sector on tbl_sector.id = tbl_company.sectorId
	left join tbl_taxpayerCategory on tbl_company .categoryId= tbl_taxpayerCategory.id
	where tbl_company.lcdaId = @lcdaId
end
