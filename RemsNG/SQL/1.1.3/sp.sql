CREATE procedure sp_getByCategoryDate
(
	@startDate Datetime,
	@endDate Datetime
)
as
begin
	select dni.*,tc.taxpayerCategoryName as category, wrd.wardName as wardName from tbl_demandNoticeItem as dni
inner join tbl_taxPayer as tp on tp.id = dni.taxpayerId
inner join tbl_company as cyp on cyp.id = tp.companyId
inner join tbl_street as street on street.id = tp.streetId
inner join tbl_ward as wrd on wrd.id = street.wardId
inner join tbl_taxpayerCategory as tc on tc.id = cyp.categoryId 
where dni.dateCreated >= @startDate and dni.dateCreated <= @endDate and itemStatus in('PENDING','PART_PAYMENT','PAID')
end

go

CREATE procedure sp_getArrearsByCategoryDate
(
	@startDate Datetime,
	@endDate Datetime
)
as
begin
	select dni.*,tc.taxpayerCategoryName as category, wrd.wardName as wardName from tbl_demandNoticeArrears as dni
inner join tbl_taxPayer as tp on tp.id = dni.taxpayerId
inner join tbl_company as cyp on cyp.id = tp.companyId
inner join tbl_street as street on street.id = tp.streetId
inner join tbl_ward as wrd on wrd.id = street.wardId
inner join tbl_taxpayerCategory as tc on tc.id = cyp.categoryId 
where dni.dateCreated >= @startDate and dni.dateCreated <= @endDate and arrearsStatus in ('PENDING','PART_PAYMENT','PAID')

end

go

CREATE procedure sp_getPenaltyByCategoryDate
(
	@startDate Datetime,
	@endDate Datetime
)
as
begin
	select dni.*,tc.taxpayerCategoryName as category, wrd.wardName as wardName from tbl_demandNoticePenalty as dni
inner join tbl_taxPayer as tp on tp.id = dni.taxpayerId
inner join tbl_company as cyp on cyp.id = tp.companyId
inner join tbl_street as street on street.id = tp.streetId
inner join tbl_ward as wrd on wrd.id = street.wardId
inner join tbl_taxpayerCategory as tc on tc.id = cyp.categoryId 
where dni.dateCreated >= @startDate and dni.dateCreated <= @endDate AND itemPenaltyStatus in ('PENDING','PART_PAYMENT','PAID')

end