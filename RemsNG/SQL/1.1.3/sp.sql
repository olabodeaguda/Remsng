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

go

ALTER procedure sp_paymenthistoryByLcda(
@lcdaId uniqueidentifier,
	@pageSize int,
	@pageNum int
)
as
begin
declare @totalSize int;
	
	select @totalSize =  COUNT(*) from tbl_demandNoticePaymentHistory as dnph
	where dnph.ownerId in (select tbl_taxPayer.id from tbl_taxPayer
		inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
		inner join tbl_ward on tbl_ward.id = tbl_street.wardId
		where tbl_ward.lcdaId = @lcdaId);

IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

	select dnph.*,@totalSize as totalSize,dnp.billingYr as billingYear,
	dnp.taxpayersName from tbl_demandNoticePaymentHistory as dnph
	inner join tbl_demandNoticeTaxpayers as dnp on dnp.billingNumber = dnph.billingNumber
	where dnph.ownerId in (select tbl_taxPayer.id from tbl_taxPayer	
		inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
		inner join tbl_ward on tbl_ward.id = tbl_street.wardId
		where tbl_ward.lcdaId = @lcdaId)
			ORDER BY dnph.dateCreated desc
                 OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
end