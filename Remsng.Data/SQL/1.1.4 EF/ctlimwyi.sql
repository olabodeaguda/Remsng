IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [tbl_bank] (
    [Id] uniqueidentifier NOT NULL,
    [BankName] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    CONSTRAINT [PK_tbl_bank] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_batchDownloadRequest] (
    [Id] uniqueidentifier NOT NULL,
    [BatchNo] nvarchar(max) NULL,
    [RequestStatus] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NULL,
    [BatchFileName] nvarchar(max) NULL,
    [Createdby] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_batchDownloadRequest] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_cloudData] (
    [Id] uniqueidentifier NOT NULL,
    [DomainId] uniqueidentifier NOT NULL,
    [DataTitle] nvarchar(max) NULL,
    [SyncStatus] nvarchar(max) NULL,
    [JsonContent] nvarchar(max) NULL,
    [BillingNumber] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    CONSTRAINT [PK_tbl_cloudData] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_contactDetail] (
    [Id] uniqueidentifier NOT NULL,
    [OwnerId] uniqueidentifier NOT NULL,
    [ContactValue] nvarchar(max) NULL,
    [ContactType] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_contactDetail] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_contactPerson] (
    [Id] uniqueidentifier NOT NULL,
    [Surname] nvarchar(max) NULL,
    [Firstname] nvarchar(max) NULL,
    [Lastname] nvarchar(max) NULL,
    [TaxPayerId] uniqueidentifier NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_contactPerson] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_country] (
    [Id] uniqueidentifier NOT NULL,
    [CountryName] nvarchar(max) NULL,
    [CountryCode] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_country] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_DemandNoticeDownloadHistory] (
    [Id] uniqueidentifier NOT NULL,
    [BillingNumber] nvarchar(max) NULL,
    [GrandTotal] decimal(18, 2) NOT NULL,
    [Charges] decimal(18, 2) NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_DemandNoticeDownloadHistory] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_error] (
    [Id] uniqueidentifier NOT NULL,
    [ErrorType] nvarchar(max) NULL,
    [Errorvalue] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [OwnerId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_error] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_images] (
    [Id] uniqueidentifier NOT NULL,
    [ImgFilename] nvarchar(max) NULL,
    [OwnerId] uniqueidentifier NOT NULL,
    [ImgType] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_images] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_LcdaProperty] (
    [Id] uniqueidentifier NOT NULL,
    [PropertyKey] nvarchar(max) NULL,
    [PropertyValue] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [PropertyStatus] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_LcdaProperty] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_permission] (
    [Id] uniqueidentifier NOT NULL,
    [PermissionName] nvarchar(max) NULL,
    [PermissionDescription] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_permission] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_prepayment] (
    [id] bigint NOT NULL IDENTITY,
    [taxpayerId] uniqueidentifier NOT NULL,
    [amount] decimal(18, 2) NOT NULL,
    [datecreated] datetime2 NOT NULL,
    [prepaymentStatus] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_prepayment] PRIMARY KEY ([id])
);

GO

CREATE TABLE [tbl_taxpayerCategory] (
    [Id] uniqueidentifier NOT NULL,
    [TaxpayerCategoryName] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_taxpayerCategory] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_users] (
    [Id] uniqueidentifier NOT NULL,
    [Email] nvarchar(max) NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [LockedOutEndDateUtc] datetime2 NULL,
    [Lockedoutenabled] bit NULL,
    [Username] nvarchar(max) NULL,
    [UserStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [Lastname] nvarchar(max) NULL,
    [Surname] nvarchar(max) NULL,
    [Firstname] nvarchar(max) NULL,
    [Gender] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_users] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [tbl_demandNoticePaymentHistory] (
    [Id] uniqueidentifier NOT NULL,
    [OwnerId] uniqueidentifier NOT NULL,
    [BillingNumber] nvarchar(max) NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [Charges] decimal(18, 2) NOT NULL,
    [PaymentMode] nvarchar(max) NULL,
    [ReferenceNumber] nvarchar(max) NULL,
    [BankId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [PaymentStatus] nvarchar(max) NULL,
    [SyncStatus] bit NOT NULL,
    [IsWaiver] bit NOT NULL,
    CONSTRAINT [PK_tbl_demandNoticePaymentHistory] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandNoticePaymentHistory_tbl_bank_BankId] FOREIGN KEY ([BankId]) REFERENCES [tbl_bank] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_state] (
    [Id] uniqueidentifier NOT NULL,
    [CountryId] uniqueidentifier NOT NULL,
    [StateCode] nvarchar(max) NULL,
    [StateName] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_state] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_state_tbl_country_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [tbl_country] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_company] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyName] nvarchar(max) NULL,
    [StreetId] uniqueidentifier NULL,
    [SectorId] uniqueidentifier NULL,
    [AddressId] uniqueidentifier NULL,
    [CategoryId] uniqueidentifier NULL,
    [CompanyStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_company] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_company_tbl_taxpayerCategory_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [tbl_taxpayerCategory] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [tbl_domain] (
    [Id] uniqueidentifier NOT NULL,
    [DomainName] nvarchar(max) NULL,
    [DomainCode] nvarchar(max) NULL,
    [Datecreated] datetime2 NOT NULL,
    [AddressId] uniqueidentifier NULL,
    [DomainStatus] nvarchar(max) NULL,
    [DomainType] nvarchar(max) NULL,
    [StateId] uniqueidentifier NULL,
    CONSTRAINT [PK_tbl_domain] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_domain_tbl_state_StateId] FOREIGN KEY ([StateId]) REFERENCES [tbl_state] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [tbl_lcda] (
    [Id] uniqueidentifier NOT NULL,
    [DomainId] uniqueidentifier NOT NULL,
    [LcdaName] nvarchar(max) NULL,
    [LcdaCode] nvarchar(max) NULL,
    [AddressId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [LcdaStatus] nvarchar(max) NULL,
    [Charges] decimal(18, 2) NOT NULL,
    CONSTRAINT [PK_tbl_lcda] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_lcda_tbl_domain_DomainId] FOREIGN KEY ([DomainId]) REFERENCES [tbl_domain] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_userdomain] (
    [UserId] uniqueidentifier NOT NULL,
    [DomainId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_userdomain] PRIMARY KEY ([UserId], [DomainId]),
    CONSTRAINT [FK_tbl_userdomain_tbl_domain_DomainId] FOREIGN KEY ([DomainId]) REFERENCES [tbl_domain] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_userdomain_tbl_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [tbl_users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_bank_lcda] (
    [Id] uniqueidentifier NOT NULL,
    [BankId] uniqueidentifier NOT NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [BankAccount] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_bank_lcda] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_bank_lcda_tbl_bank_BankId] FOREIGN KEY ([BankId]) REFERENCES [tbl_bank] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_bank_lcda_tbl_lcda_LcdaId] FOREIGN KEY ([LcdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_item] (
    [Id] uniqueidentifier NOT NULL,
    [ItemDescription] nvarchar(max) NULL,
    [ItemStatus] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [ItemCode] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_item] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_item_tbl_lcda_LcdaId] FOREIGN KEY ([LcdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_role] (
    [Id] uniqueidentifier NOT NULL,
    [RoleName] nvarchar(max) NULL,
    [DomainId] uniqueidentifier NOT NULL,
    [RoleStatus] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_role] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_role_tbl_lcda_DomainId] FOREIGN KEY ([DomainId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_sector] (
    [Id] uniqueidentifier NOT NULL,
    [SectorName] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [Prefix] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_sector] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_sector_tbl_lcda_LcdaId] FOREIGN KEY ([LcdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_userlcda] (
    [LgdaId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_userlcda] PRIMARY KEY ([UserId], [LgdaId]),
    CONSTRAINT [FK_tbl_userlcda_tbl_lcda_LgdaId] FOREIGN KEY ([LgdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_userlcda_tbl_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [tbl_users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_ward] (
    [Id] uniqueidentifier NOT NULL,
    [WardName] nvarchar(max) NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [WardStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_ward] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_ward_tbl_lcda_LcdaId] FOREIGN KEY ([LcdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_itempenalty] (
    [Id] uniqueidentifier NOT NULL,
    [ItemId] uniqueidentifier NOT NULL,
    [IsPercentage] bit NOT NULL,
    [PenaltyStatus] nvarchar(max) NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [Duration] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_itempenalty] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_itempenalty_tbl_item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [tbl_item] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_rolePermission] (
    [RoleId] uniqueidentifier NOT NULL,
    [PermissionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_rolePermission] PRIMARY KEY ([RoleId], [PermissionId]),
    CONSTRAINT [FK_tbl_rolePermission_tbl_permission_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [tbl_permission] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_rolePermission_tbl_role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [tbl_role] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_userRole] (
    [RoleId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_userRole] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_tbl_userRole_tbl_role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [tbl_role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_userRole_tbl_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [tbl_users] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_street] (
    [Id] uniqueidentifier NOT NULL,
    [WardId] uniqueidentifier NOT NULL,
    [StreetName] nvarchar(max) NULL,
    [NumberOfHouse] int NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [StreetStatus] nvarchar(max) NULL,
    [StreetDescription] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_street] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_street_tbl_ward_WardId] FOREIGN KEY ([WardId]) REFERENCES [tbl_ward] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_demandnotice] (
    [Id] uniqueidentifier NOT NULL,
    [Query] nvarchar(max) NULL,
    [PlainTextQuery] nvarchar(max) NULL,
    [BatchNo] nvarchar(max) NULL,
    [DemandNoticeStatus] nvarchar(max) NULL,
    [BillingYear] int NOT NULL,
    [LcdaId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [WardId] uniqueidentifier NULL,
    [StreetId] uniqueidentifier NULL,
    [IsUnbilled] bit NOT NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_tbl_demandnotice] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandnotice_tbl_street_StreetId] FOREIGN KEY ([StreetId]) REFERENCES [tbl_street] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_tbl_demandnotice_tbl_ward_WardId] FOREIGN KEY ([WardId]) REFERENCES [tbl_ward] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [tbl_demandNoticeTaxpayers] (
    [Id] uniqueidentifier NOT NULL,
    [DnId] uniqueidentifier NOT NULL,
    [TaxpayerId] uniqueidentifier NOT NULL,
    [TaxpayersName] nvarchar(max) NULL,
    [BillingNumber] nvarchar(max) NULL,
    [AddressName] nvarchar(max) NULL,
    [WardName] nvarchar(max) NULL,
    [LcdaName] nvarchar(max) NULL,
    [BillingYr] int NOT NULL,
    [DomainName] nvarchar(max) NULL,
    [LcdaAddress] nvarchar(max) NULL,
    [LcdaState] nvarchar(max) NULL,
    [LcdaLogoFileName] nvarchar(max) NULL,
    [CouncilTreasurerSigFilen] nvarchar(max) NULL,
    [RevCoodinatorSigFilen] nvarchar(max) NULL,
    [CouncilTreasurerMobile] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [DemandNoticeStatus] nvarchar(max) NULL,
    [IsUnbilled] bit NOT NULL,
    [Period] int NOT NULL,
    [IsRunArrears] bit NOT NULL,
    [IsRunPenalty] bit NOT NULL,
    CONSTRAINT [PK_tbl_demandNoticeTaxpayers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandNoticeTaxpayers_tbl_demandnotice_DnId] FOREIGN KEY ([DnId]) REFERENCES [tbl_demandnotice] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_taxPayer] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [StreetId] uniqueidentifier NULL,
    [AddressId] uniqueidentifier NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [TaxpayerStatus] nvarchar(max) NULL,
    [Surname] nvarchar(max) NULL,
    [Firstname] nvarchar(max) NULL,
    [Lastname] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_taxPayer] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_taxPayer_tbl_company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [tbl_company] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_taxPayer_tbl_street_StreetId] FOREIGN KEY ([StreetId]) REFERENCES [tbl_street] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [tbl_address] (
    [Id] uniqueidentifier NOT NULL,
    [Addressnumber] nvarchar(max) NULL,
    [StreetId] uniqueidentifier NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [OwnerId] uniqueidentifier NOT NULL,
    [Lcdaid] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_tbl_address] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_address_tbl_taxPayer_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_address_tbl_street_StreetId] FOREIGN KEY ([StreetId]) REFERENCES [tbl_street] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_companyItem] (
    [Id] uniqueidentifier NOT NULL,
    [TaxpayerId] uniqueidentifier NOT NULL,
    [ItemId] uniqueidentifier NOT NULL,
    [Amount] decimal(18, 2) NOT NULL,
    [BillingYear] int NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NOT NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [CompanyStatus] nvarchar(max) NULL,
    CONSTRAINT [PK_tbl_companyItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_companyItem_tbl_item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [tbl_item] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_companyItem_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_demandNoticeArrears] (
    [Id] uniqueidentifier NOT NULL,
    [BillingNo] nvarchar(max) NULL,
    [TaxpayerId] uniqueidentifier NOT NULL,
    [TotalAmount] decimal(18, 2) NOT NULL,
    [CurrentAmount] decimal(18, 2) NOT NULL,
    [AmountPaid] decimal(18, 2) NOT NULL,
    [OriginatedYear] int NOT NULL,
    [BillingYear] int NOT NULL,
    [ArrearsStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_demandNoticeArrears] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandNoticeArrears_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_demandNoticeItem] (
    [Id] uniqueidentifier NOT NULL,
    [BillingNo] nvarchar(max) NULL,
    [dn_taxpayersDetailsId] uniqueidentifier NOT NULL,
    [DemandNoticeId] uniqueidentifier NOT NULL,
    [TaxpayerId] uniqueidentifier NOT NULL,
    [ItemId] uniqueidentifier NOT NULL,
    [ItemName] nvarchar(max) NULL,
    [ItemAmount] decimal(18, 2) NOT NULL,
    [AmountPaid] decimal(18, 2) NOT NULL,
    [ItemStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_tbl_demandNoticeItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandNoticeItem_tbl_item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [tbl_item] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_demandNoticeItem_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_tbl_demandNoticeItem_tbl_demandNoticeTaxpayers_dn_taxpayersDetailsId] FOREIGN KEY ([dn_taxpayersDetailsId]) REFERENCES [tbl_demandNoticeTaxpayers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [tbl_demandNoticePenalty] (
    [Id] uniqueidentifier NOT NULL,
    [BillingNo] nvarchar(max) NULL,
    [TaxpayerId] uniqueidentifier NOT NULL,
    [TotalAmount] decimal(18, 2) NOT NULL,
    [AmountPaid] decimal(18, 2) NOT NULL,
    [OriginatedYear] int NOT NULL,
    [BillingYear] int NOT NULL,
    [ItemPenaltyStatus] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [DateCreated] datetime2 NULL,
    [Lastmodifiedby] nvarchar(max) NULL,
    [LastModifiedDate] datetime2 NULL,
    [CurrentAmount] decimal(18, 2) NOT NULL,
    CONSTRAINT [PK_tbl_demandNoticePenalty] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_tbl_demandNoticePenalty_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_tbl_address_OwnerId] ON [tbl_address] ([OwnerId]);

GO

CREATE INDEX [IX_tbl_address_StreetId] ON [tbl_address] ([StreetId]);

GO

CREATE INDEX [IX_tbl_bank_lcda_BankId] ON [tbl_bank_lcda] ([BankId]);

GO

CREATE INDEX [IX_tbl_bank_lcda_LcdaId] ON [tbl_bank_lcda] ([LcdaId]);

GO

CREATE INDEX [IX_tbl_company_CategoryId] ON [tbl_company] ([CategoryId]);

GO

CREATE INDEX [IX_tbl_companyItem_ItemId] ON [tbl_companyItem] ([ItemId]);

GO

CREATE INDEX [IX_tbl_companyItem_TaxpayerId] ON [tbl_companyItem] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandnotice_StreetId] ON [tbl_demandnotice] ([StreetId]);

GO

CREATE INDEX [IX_tbl_demandnotice_WardId] ON [tbl_demandnotice] ([WardId]);

GO

CREATE INDEX [IX_tbl_demandNoticeArrears_TaxpayerId] ON [tbl_demandNoticeArrears] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandNoticeItem_ItemId] ON [tbl_demandNoticeItem] ([ItemId]);

GO

CREATE INDEX [IX_tbl_demandNoticeItem_TaxpayerId] ON [tbl_demandNoticeItem] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandNoticeItem_dn_taxpayersDetailsId] ON [tbl_demandNoticeItem] ([dn_taxpayersDetailsId]);

GO

CREATE INDEX [IX_tbl_demandNoticePaymentHistory_BankId] ON [tbl_demandNoticePaymentHistory] ([BankId]);

GO

CREATE INDEX [IX_tbl_demandNoticePenalty_TaxpayerId] ON [tbl_demandNoticePenalty] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandNoticeTaxpayers_DnId] ON [tbl_demandNoticeTaxpayers] ([DnId]);

GO

CREATE INDEX [IX_tbl_domain_StateId] ON [tbl_domain] ([StateId]);

GO

CREATE INDEX [IX_tbl_item_LcdaId] ON [tbl_item] ([LcdaId]);

GO

CREATE INDEX [IX_tbl_itempenalty_ItemId] ON [tbl_itempenalty] ([ItemId]);

GO

CREATE INDEX [IX_tbl_lcda_DomainId] ON [tbl_lcda] ([DomainId]);

GO

CREATE INDEX [IX_tbl_role_DomainId] ON [tbl_role] ([DomainId]);

GO

CREATE INDEX [IX_tbl_rolePermission_PermissionId] ON [tbl_rolePermission] ([PermissionId]);

GO

CREATE INDEX [IX_tbl_sector_LcdaId] ON [tbl_sector] ([LcdaId]);

GO

CREATE INDEX [IX_tbl_state_CountryId] ON [tbl_state] ([CountryId]);

GO

CREATE INDEX [IX_tbl_street_WardId] ON [tbl_street] ([WardId]);

GO

CREATE UNIQUE INDEX [IX_tbl_taxPayer_AddressId] ON [tbl_taxPayer] ([AddressId]) WHERE [AddressId] IS NOT NULL;

GO

CREATE INDEX [IX_tbl_taxPayer_CompanyId] ON [tbl_taxPayer] ([CompanyId]);

GO

CREATE INDEX [IX_tbl_taxPayer_StreetId] ON [tbl_taxPayer] ([StreetId]);

GO

CREATE INDEX [IX_tbl_userdomain_DomainId] ON [tbl_userdomain] ([DomainId]);

GO

CREATE INDEX [IX_tbl_userlcda_LgdaId] ON [tbl_userlcda] ([LgdaId]);

GO

CREATE INDEX [IX_tbl_userRole_RoleId] ON [tbl_userRole] ([RoleId]);

GO

CREATE INDEX [IX_tbl_ward_LcdaId] ON [tbl_ward] ([LcdaId]);

GO

ALTER TABLE [tbl_taxPayer] ADD CONSTRAINT [FK_tbl_taxPayer_tbl_address_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [tbl_address] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190310131557_initialize', N'2.1.0-rtm-30799');

GO

