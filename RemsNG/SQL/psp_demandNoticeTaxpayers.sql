IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandNoticeTaxpayers') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandNoticeTaxpayers
			  (
				  id uniqueidentifier not null primary key,
				  dnId uniqueidentifier not null,
				  taxpayerId uniqueidentifier not null,
				  taxpayersName varchar(200) not null,
				  billingNumber varchar(100) not null,
				  addressName varchar(250) not null,
				  wardName varchar(150) not null,
				  lcdaName varchar(250) not null,
				  billingYr int not null,
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'fn_generateBillingNumber') AND type IN (N'P', N'PC'))
		   drop function fn_generateBillingNumber
		  go
		create function fn_generateBillingNumber()
		returns int as
		begin
			declare @continue bit;
			set @continue = 0;
			declare @uniquenumber int
			select @uniquenumber = uniqueNumber from UniqueNumberView;
	
			while (@continue <> 0)
			begin
				if exists(select * from tbl_demandNoticeTaxpayers where tbl_demandNoticeTaxpayers.billingNumber = @uniquenumber)
				begin
					select @uniquenumber = uniqueNumber from UniqueNumberView;
					set @continue = 0;
				end
				else
				begin
					set @continue = 1;
				end
			end
			return @uniquenumber;
		end

GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addDemandNoticeTaxpayer') AND type IN (N'P', N'PC'))
		  drop procedure sp_addDemandNoticeTaxpayer
		  go
create procedure sp_addDemandNoticeTaxpayer
(
	@dnId uniqueidentifier,
	@billingYr int,
	@createdBy varchar(100),
	@taxpayersId uniqueidentifier
)
as
begin
	declare @msg varchar(250);
	declare @success bit;

	if exists(select * from tbl_demandNoticeTaxpayers where taxpayerId = @taxpayerid and billingYr = @billingYr)
	begin
		set @msg = 'Taxpayer already exist';
		set @success = 0;
	end
	else
	begin
		insert into tbl_demandNoticeTaxpayers(id,dnId,taxpayerId,taxpayersName,billingNumber,addressName,wardName,lcdaName,billingYr,createdBy,dateCreated)
			select newid(),@dnId,@taxpayersId,
			CONCAT(tbl_taxPayer.surname,' ',tbl_taxPayer.firstname,' ',tbl_taxPayer.lastname),
			dbo.fn_generateBillingNumber(),
			concat(tbl_address.addressnumber,', ',tbl_street.streetName),
			tbl_ward.wardName,
			tbl_lcda.lcdaName,@billingYr,@createdBy,GETDATE()
			 from tbl_taxPayer 
			inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId
			inner join tbl_address on tbl_address.id = tbl_taxpayer.addressId
			inner join tbl_ward on tbl_ward.id = tbl_street.wardId
			inner join tbl_lcda on tbl_lcda.id = tbl_ward.lcdaId
			where tbl_taxPayer.id = @taxpayersId;

			if @@Rowcount > 0
			begin
				set @msg = 'Demand notice has been raised successfully';
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
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_dequeueDemandNotice') AND type IN (N'P', N'PC'))
		  drop procedure sp_dequeueDemandNotice
GO
create procedure sp_dequeueDemandNotice
as 
begin
	select top 1 * from tbl_demandnotice where demandNoticeStatus = 'SUBMITTED'
end
GO

