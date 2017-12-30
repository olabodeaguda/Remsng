IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_demandnotice') AND type IN (N'U'))
			  CREATE TABLE	tbl_demandnotice
			  (
				  id uniqueidentifier not null primary key,
				  query varchar(250) not null,
				  batchNo varchar(20) not null,
				  demandNoticeStatus varchar(50) not null,
				  billingYear int not null,
				  lcdaId uniqueidentifier not null,
				  createdBy varchar(100) not null,
				  dateCreated datetime default getDate(),
				  lastmodifiedby varchar(100),
				  lastModifiedDate datetime
			  )
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_addDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_addDemandNotice
  GO
  create procedure sp_addDemandNotice
  (
	@id uniqueidentifier,
	@query varchar(MAX),
	@batchno varchar(20),
	@demandNoticeStatus varchar(50),
	@billingYear int,
	@lcdaId uniqueidentifier,
	@createdBy varchar(100)
  )
  as
  begin
  declare @msg varchar(100);
  declare @success bit;

	if exists(select * from tbl_demandnotice where query = @query and lcdaId = @lcdaId and billingyear = @billingYear and demandNoticeStatus <> 'CANCEL')
	begin
		set @msg = 'Demand notice already exist';	
		set @success = 0;
	end
	else
	begin
		insert into tbl_demandnotice(id,query,batchNo,demandNoticeStatus,billingYear,lcdaId,createdBy,dateCreated) 
		values(@id,@query,@batchno,@demandNoticeStatus,@billingYear,@lcdaId,@createdBy,GETDATE())
		
		if @@ROWCOUNT > 0
		begin
			set @msg = 'Demand notice has been submited for onward processing';
			set @success = 1
		end
		else
		begin
			set @msg = 'Database Error: An error occur while trying to submit demand notice request';
			set @success = 0;
		end
	end

	select NEWID() as id, @msg  as msg,@success as success
  end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateQueryDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateQueryDemandNotice
  GO
  create procedure sp_updateQueryDemandNotice
  (
	@id uniqueidentifier,
	@query varchar(250)
  )
  as
  begin
	declare @msg varchar(100);
	declare @success bit;
	Update tbl_demandnotice set query=@query where id=@id;

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Query has been updated successfully';
		set @success = 0;
	end
	else
	begin
		set @msg = 'Database Error: An error occur while trying to update query';
		set @success = 1;
	end

	select NEWID() as id,@msg as msg, @success as success;
  end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateBillingYrDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateBillingYrDemandNotice
  GO
  create procedure sp_updateBillingYrDemandNotice
  (
	@id uniqueidentifier,
	@billingYr int
  )
  as
  begin
	declare @msg varchar(100);
	declare @success bit;
	Update tbl_demandnotice set billingYear=@billingYr where id=@id;

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Billing year has been updated successfully';
		set @success = 0;
	end
	else
	begin
		set @msg = 'Database Error: An error occur while trying to update query';
		set @success = 1;
	end

	select NEWID() as id,@msg as msg, @success as success;
  end
GO

 IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_updateStatusDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_updateStatusDemandNotice
  GO
  create procedure sp_updateStatusDemandNotice
  (
	@id uniqueidentifier,
	@demandNoticeStatus int
  )
  as
  begin
	declare @msg varchar(100);
	declare @success bit;
	Update tbl_demandnotice set demandNoticeStatus=@demandNoticeStatus where id=@id;

	if @@ROWCOUNT > 0
	begin
		set @msg = 'Status has been updated successfully';
		set @success = 0;
	end
	else
	begin
		set @msg = 'Database Error: An error occur while trying to update query';
		set @success = 1;
	end

	select NEWID() as id,@msg as msg, @success as success;
  end
  GO
  IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_demandNoticeByLcda') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_demandNoticeByLcda
  GO

  create procedure sp_demandNoticeByLcda
  (
	@lcdaId uniqueidentifier,
	@pageNum int,
	@pageSize int
  )
  as
  begin
		IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

		declare @count int;
		select @count = count(*) from tbl_demandnotice
		where lcdaId = @lcdaId;

		select tbl_demandnotice.*,@count as totalSize from tbl_demandnotice
		where lcdaId = @lcdaId
		order by dateCreated desc
		  OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
  end
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_getDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_getDemandNotice
  GO

  create procedure sp_getDemandNotice
  (
	@id uniqueidentifier
  )
  as
  begin
		select * from tbl_demandnotice where id = @id;
  end
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_dequeueDemandNotice') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_dequeueDemandNotice
  GO

  create procedure sp_dequeueDemandNotice
  as
  begin
		update top(1) tbl_demandnotice set demandNoticeStatus = 'PROCESSING'
output inserted.*,-1 as totalSize
where id in(select top 1 id from tbl_demandnotice where demandNoticeStatus = 'SUBMITTED')
  end
GO
 IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_demandNoticePaginated') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_demandNoticePaginated
  GO

  create procedure sp_demandNoticePaginated
  (
	@pageNum int,
	@pageSize int
  )
  as
  begin
		IF @pageSize = 0 or @pageSize>100
            SET @pageSize = 100;
        IF @pageNum = 0
            SET @pageNum = 1;

		declare @count int;
		select @count = count(*) from tbl_demandnotice;

		select tbl_demandnotice.*,@count as totalSize from tbl_demandnotice
		order by dateCreated desc
		  OFFSET @PageSize * (@PageNum - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;
  end
GO

