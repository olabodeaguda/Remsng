ALTER procedure sp_addDemandNoticeTaxpayer
(
	@dnId uniqueidentifier,
	@billingYr int,
	@createdBy varchar(100),
	@taxpayersId uniqueidentifier,
	@domainName varchar(250),
	@lcdaAddress varchar(250),
	@lcdaState varchar(100),
	@lcdaLogoFileName varchar(100),
	@councilTreasurerSigFilen varchar(100),
	@revCoodinatorSigFilen varchar(100),
	@councilTreasurerMobile varchar(100),
	@lcdaName varchar(250),
	@billingnumber varchar(20)
)
as
begin
	declare @msg varchar(250);
	declare @success bit;
	-- declare @billingnumber varchar(100);
	-- set @billingnumber = dbo.fn_generateBillingNumber();

	if exists(select * from tbl_demandNoticeTaxpayers where taxpayerId = @taxpayersId and billingYr = @billingYr and demandNoticeStatus not in ('CLOSED','CANCEL'))
	begin
		set @msg = 'Taxpayer already exist';
		set @success = 0;
	end
	else
	begin
		insert into tbl_demandNoticeTaxpayers(id,dnId,taxpayerId,taxpayersName,billingNumber,addressName,wardName
		,lcdaName,billingYr,createdBy,dateCreated,domainName,lcdaAddress,lcdaLogoFileName,
		councilTreasurerSigFilen,revCoodinatorSigFilen,councilTreasurerMobile,lcdaState)
			select newid(),@dnId,@taxpayersId,
			CONCAT(tbl_taxPayer.surname,' ',tbl_taxPayer.firstname,' ',tbl_taxPayer.lastname),
			@billingnumber,
			concat(tbl_address.addressnumber,', ',tbl_street.streetName),
			tbl_ward.wardName,
			tbl_lcda.lcdaName,@billingYr,@createdBy,GETDATE(),@domainName,@lcdaAddress,@lcdaLogoFileName,
			@councilTreasurerSigFilen,@revCoodinatorSigFilen,@councilTreasurerMobile,@lcdaState
			 from tbl_taxPayer(nolock) 
			inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
			inner join tbl_address on tbl_address.id = tbl_taxpayer.addressId
			inner join tbl_ward on tbl_ward.id = tbl_street.wardId
			inner join tbl_lcda on tbl_lcda.id = tbl_ward.lcdaId
			where tbl_taxPayer.id = @taxpayersId

			if @@Rowcount > 0
			begin
				set @msg = @billingnumber;
				set @success = 1;
			end
			else
			begin
				set @msg = 'Zero record was affected';
				set @success = 0;
			end
	end
		select newId() as id,@msg as msg,@success as success
end
GO
