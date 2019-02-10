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
go


  ALTER procedure [dbo].[sp_addTaxpayerDemandNoticeItem](
	@demandNoticeId uniqueidentifier,
	@taxpayerId uniqueidentifier,
	@billinYr int,
	@createdBy varchar(100)
  )
  as
  begin
	  declare @msg varchar(100);
	  declare @success bit;
		insert into tbl_demandNoticeItem(id,dn_taxpayersDetailsId,taxpayerId,itemId,itemName,itemAmount,
		itemStatus,billingNo,amountPaid,createdBy)
		select NEWID(),@demandNoticeId,tbl_demandNoticeTaxpayers.taxpayerId,tbl_companyItem.itemId,tbl_item.itemDescription
		,tbl_companyItem.amount,'PENDING',tbl_demandNoticeTaxpayers.billingNumber,0.0,@createdBy 
		from tbl_demandNoticeTaxpayers(nolock)
		inner join tbl_companyItem on tbl_companyItem.taxpayerId = tbl_demandNoticeTaxpayers.taxpayerId
		inner join tbl_item on tbl_item.id = tbl_companyItem.itemId
		 where tbl_demandNoticeTaxpayers.billingYr = @billinYr and tbl_companyItem.companyStatus = 'ACTIVE' and 
		 tbl_demandNoticeTaxpayers.dnId = @demandNoticeId and tbl_demandNoticeTaxpayers.taxpayerId = @taxpayerId and tbl_item.itemStatus= 'ACTIVE';

		 if @@rowcount > 0
		 begin
			set @msg = 'Request was successfully';
			set @success = 1;
		 end
		 else
		 begin
			set @msg = 'Zero rows affected';
			set @success = 0;
		 end

		 select newid() as id,@msg as msg,@success as success;
  end
  go
  ALTER procedure sp_addPayment
(
	@taxpayerId uniqueidentifier,
	@billingNumber varchar(30),
	@amount decimal(18,2),
	@charges decimal(18,2),
	@paymentMode varchar(30),
	@referenceNumber varchar(30),
	@bankId uniqueidentifier,
	@createdBy varchar(100),
	@dateCreated datetime,
	@isWaiver bit
)
as
begin
	declare @msg varchar(200);
	declare @success bit;

	insert into tbl_demandNoticePaymentHistory(id,ownerId,billingNumber,amount,charges,paymentMode,
	referenceNumber,bankId,createdBy,dateCreated,paymentStatus,IsWaiver)
	values(NEWID(),@taxpayerId,@billingNumber,@amount,@charges,@paymentMode,@referenceNumber,@bankId,
	@createdBy,@dateCreated,'COMPLETED',@isWaiver);

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Payment has been added';
		set @success = 1;
	end
	else
	begin
		set @msg = 'An error occured, Please try again, Payment has been added';
		set @success = 0;
	end
	select NEWID() as id, @msg as msg,@success as success;
end

