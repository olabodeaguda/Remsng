IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

ALTER TABLE [tbl_demandNoticeItem] DROP CONSTRAINT [FK_tbl_demandNoticeItem_tbl_demandnotice_DnTaxpayersDetailsId];

GO

DROP INDEX [IX_tbl_demandNoticeItem_DnTaxpayersDetailsId] ON [tbl_demandNoticeItem];

GO

ALTER TABLE [tbl_demandNoticeItem] ADD [DemandNoticeId] uniqueidentifier NULL;

GO

ALTER TABLE [tbl_demandnotice] ADD [RowVersion] rowversion NULL;

GO

ALTER TABLE [tbl_company] ADD [TaxPayerCatgeoryId] uniqueidentifier NULL;

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'tbl_bank_lcda') AND [c].[name] = N'LcdaId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [tbl_bank_lcda] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [tbl_bank_lcda] ALTER COLUMN [LcdaId] uniqueidentifier NOT NULL;

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'tbl_bank_lcda') AND [c].[name] = N'BankId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [tbl_bank_lcda] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [tbl_bank_lcda] ALTER COLUMN [BankId] uniqueidentifier NOT NULL;

GO

CREATE UNIQUE INDEX [IX_tbl_taxPayer_AddressId] ON [tbl_taxPayer] ([AddressId]) WHERE [AddressId] IS NOT NULL;

GO

CREATE INDEX [IX_tbl_demandNoticeTaxpayers_DnId] ON [tbl_demandNoticeTaxpayers] ([DnId]);

GO

CREATE INDEX [IX_tbl_demandNoticePenalty_ItemId] ON [tbl_demandNoticePenalty] ([ItemId]);

GO

CREATE INDEX [IX_tbl_demandNoticePenalty_TaxpayerId] ON [tbl_demandNoticePenalty] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandNoticePaymentHistory_BankId] ON [tbl_demandNoticePaymentHistory] ([BankId]);

GO

CREATE INDEX [IX_tbl_demandNoticeItem_DemandNoticeId] ON [tbl_demandNoticeItem] ([DemandNoticeId]);

GO

CREATE INDEX [IX_tbl_demandNoticeItem_TaxpayerId] ON [tbl_demandNoticeItem] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandNoticeArrears_ItemId] ON [tbl_demandNoticeArrears] ([ItemId]);

GO

CREATE INDEX [IX_tbl_demandNoticeArrears_TaxpayerId] ON [tbl_demandNoticeArrears] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_demandnotice_WardId] ON [tbl_demandnotice] ([WardId]);

GO

CREATE INDEX [IX_tbl_companyItem_TaxpayerId] ON [tbl_companyItem] ([TaxpayerId]);

GO

CREATE INDEX [IX_tbl_company_TaxPayerCatgeoryId] ON [tbl_company] ([TaxPayerCatgeoryId]);

GO

CREATE INDEX [IX_tbl_bank_lcda_BankId] ON [tbl_bank_lcda] ([BankId]);

GO

CREATE INDEX [IX_tbl_bank_lcda_LcdaId] ON [tbl_bank_lcda] ([LcdaId]);

GO

CREATE INDEX [IX_tbl_address_OwnerId] ON [tbl_address] ([OwnerId]);

GO

ALTER TABLE [tbl_address] ADD CONSTRAINT [FK_tbl_address_tbl_taxPayer_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_bank_lcda] ADD CONSTRAINT [FK_tbl_bank_lcda_tbl_bank_BankId] FOREIGN KEY ([BankId]) REFERENCES [tbl_bank] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_bank_lcda] ADD CONSTRAINT [FK_tbl_bank_lcda_tbl_lcda_LcdaId] FOREIGN KEY ([LcdaId]) REFERENCES [tbl_lcda] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_company] ADD CONSTRAINT [FK_tbl_company_tbl_taxpayerCategory_TaxPayerCatgeoryId] FOREIGN KEY ([TaxPayerCatgeoryId]) REFERENCES [tbl_taxpayerCategory] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [tbl_companyItem] ADD CONSTRAINT [FK_tbl_companyItem_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandnotice] ADD CONSTRAINT [FK_tbl_demandnotice_tbl_ward_WardId] FOREIGN KEY ([WardId]) REFERENCES [tbl_ward] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [tbl_demandNoticeArrears] ADD CONSTRAINT [FK_tbl_demandNoticeArrears_tbl_item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [tbl_item] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticeArrears] ADD CONSTRAINT [FK_tbl_demandNoticeArrears_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticeItem] ADD CONSTRAINT [FK_tbl_demandNoticeItem_tbl_demandnotice_DemandNoticeId] FOREIGN KEY ([DemandNoticeId]) REFERENCES [tbl_demandnotice] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [tbl_demandNoticeItem] ADD CONSTRAINT [FK_tbl_demandNoticeItem_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticePaymentHistory] ADD CONSTRAINT [FK_tbl_demandNoticePaymentHistory_tbl_bank_BankId] FOREIGN KEY ([BankId]) REFERENCES [tbl_bank] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticePenalty] ADD CONSTRAINT [FK_tbl_demandNoticePenalty_tbl_item_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [tbl_item] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticePenalty] ADD CONSTRAINT [FK_tbl_demandNoticePenalty_tbl_taxPayer_TaxpayerId] FOREIGN KEY ([TaxpayerId]) REFERENCES [tbl_taxPayer] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_demandNoticeTaxpayers] ADD CONSTRAINT [FK_tbl_demandNoticeTaxpayers_tbl_demandnotice_DnId] FOREIGN KEY ([DnId]) REFERENCES [tbl_demandnotice] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [tbl_taxPayer] ADD CONSTRAINT [FK_tbl_taxPayer_tbl_address_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [tbl_address] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190224080331_allupdate', N'2.1.0-rtm-30799');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190224083249_init', N'2.1.0-rtm-30799');

GO

