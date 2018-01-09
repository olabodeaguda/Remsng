 
 ALTER procedure [dbo].[sp_addDemandNotice]
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

	--if exists(select * from tbl_demandnotice where query = @query and lcdaId = @lcdaId and billingyear = @billingYear and demandNoticeStatus <> 'CANCEL')
	--begin
	--	set @msg = 'Demand notice already exist';	
	--	set @success = 0;
	--end
	--else
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

