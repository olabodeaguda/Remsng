﻿
IF NOT EXISTS(SELECT *
              FROM sys.objects
              WHERE object_id = OBJECT_ID(N'tbl_company') AND type IN (N'U'))
CREATE TABLE tbl_company
(
	id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	companyName varchar(100) not null,
	streetId uniqueidentifier foreign key references tbl_street(id),
	sectorId uniqueidentifier foreign key references tbl_sector(id),
	addressId uniqueidentifier foreign key references tbl_address(id),
	categoryId uniqueidentifier foreign key references tbl_lcda(id),
	companyStatus varchar(100) not null default 'ACTIVE',
	createdBy varchar(100) NOT NULL,
	dateCreated datetime NOT NULL DEFAULT GETDATE(),
	lastmodifiedby varchar(100) NULL,
	lastModifiedDate [datetime] NULL
)
GO
IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'sp_companyBystreetId') AND type IN (N'P', N'PC'))
  DROP PROCEDURE sp_companyBystreetId
GO
create procedure sp_companyBystreetId
(
	@streetId uniqueidentifier
)
as
begin
	
	select distinct tbl_company.* from tbl_company
inner join tbl_address on tbl_address.ownerId = tbl_company.id
inner join tbl_street on tbl_street.id = tbl_address.streetId
	where tbl_street.id = @streetId

end

GO

