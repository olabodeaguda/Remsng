using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RemsNG.Migrations
{
    public partial class allupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_demandnotice_DnTaxpayersDetailsId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeItem_DnTaxpayersDetailsId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.AddColumn<Guid>(
                name: "DemandNoticeId",
                table: "tbl_demandNoticeItem",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "tbl_demandnotice",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxPayerCatgeoryId",
                table: "tbl_company",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LcdaId",
                table: "tbl_bank_lcda",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "tbl_bank_lcda",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_taxPayer_AddressId",
                table: "tbl_taxPayer",
                column: "AddressId",
                unique: true,
                filter: "[AddressId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeTaxpayers_DnId",
                table: "tbl_demandNoticeTaxpayers",
                column: "DnId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePenalty_ItemId",
                table: "tbl_demandNoticePenalty",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePenalty_TaxpayerId",
                table: "tbl_demandNoticePenalty",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePaymentHistory_BankId",
                table: "tbl_demandNoticePaymentHistory",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_DemandNoticeId",
                table: "tbl_demandNoticeItem",
                column: "DemandNoticeId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_TaxpayerId",
                table: "tbl_demandNoticeItem",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeArrears_ItemId",
                table: "tbl_demandNoticeArrears",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeArrears_TaxpayerId",
                table: "tbl_demandNoticeArrears",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandnotice_WardId",
                table: "tbl_demandnotice",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_companyItem_TaxpayerId",
                table: "tbl_companyItem",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_company_TaxPayerCatgeoryId",
                table: "tbl_company",
                column: "TaxPayerCatgeoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bank_lcda_BankId",
                table: "tbl_bank_lcda",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bank_lcda_LcdaId",
                table: "tbl_bank_lcda",
                column: "LcdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_address_OwnerId",
                table: "tbl_address",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_address_tbl_taxPayer_OwnerId",
                table: "tbl_address",
                column: "OwnerId",
                principalTable: "tbl_taxPayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_bank_lcda_tbl_bank_BankId",
                table: "tbl_bank_lcda",
                column: "BankId",
                principalTable: "tbl_bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_bank_lcda_tbl_lcda_LcdaId",
                table: "tbl_bank_lcda",
                column: "LcdaId",
                principalTable: "tbl_lcda",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_company_tbl_taxpayerCategory_TaxPayerCatgeoryId",
                table: "tbl_company",
                column: "TaxPayerCatgeoryId",
                principalTable: "tbl_taxpayerCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_companyItem_tbl_taxPayer_TaxpayerId",
                table: "tbl_companyItem",
                column: "TaxpayerId",
                principalTable: "tbl_taxPayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice",
                column: "WardId",
                principalTable: "tbl_ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_item_ItemId",
                table: "tbl_demandNoticeArrears",
                column: "ItemId",
                principalTable: "tbl_item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticeArrears",
                column: "TaxpayerId",
                principalTable: "tbl_taxPayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_demandnotice_DemandNoticeId",
                table: "tbl_demandNoticeItem",
                column: "DemandNoticeId",
                principalTable: "tbl_demandnotice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticeItem",
                column: "TaxpayerId",
                principalTable: "tbl_taxPayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticePaymentHistory_tbl_bank_BankId",
                table: "tbl_demandNoticePaymentHistory",
                column: "BankId",
                principalTable: "tbl_bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_item_ItemId",
                table: "tbl_demandNoticePenalty",
                column: "ItemId",
                principalTable: "tbl_item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticePenalty",
                column: "TaxpayerId",
                principalTable: "tbl_taxPayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeTaxpayers_tbl_demandnotice_DnId",
                table: "tbl_demandNoticeTaxpayers",
                column: "DnId",
                principalTable: "tbl_demandnotice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_taxPayer_tbl_address_AddressId",
                table: "tbl_taxPayer",
                column: "AddressId",
                principalTable: "tbl_address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_address_tbl_taxPayer_OwnerId",
                table: "tbl_address");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_bank_lcda_tbl_bank_BankId",
                table: "tbl_bank_lcda");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_bank_lcda_tbl_lcda_LcdaId",
                table: "tbl_bank_lcda");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_company_tbl_taxpayerCategory_TaxPayerCatgeoryId",
                table: "tbl_company");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_companyItem_tbl_taxPayer_TaxpayerId",
                table: "tbl_companyItem");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_item_ItemId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_demandnotice_DemandNoticeId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticePaymentHistory_tbl_bank_BankId",
                table: "tbl_demandNoticePaymentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_item_ItemId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_taxPayer_TaxpayerId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeTaxpayers_tbl_demandnotice_DnId",
                table: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_taxPayer_tbl_address_AddressId",
                table: "tbl_taxPayer");

            migrationBuilder.DropIndex(
                name: "IX_tbl_taxPayer_AddressId",
                table: "tbl_taxPayer");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeTaxpayers_DnId",
                table: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticePenalty_ItemId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticePenalty_TaxpayerId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticePaymentHistory_BankId",
                table: "tbl_demandNoticePaymentHistory");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeItem_DemandNoticeId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeItem_TaxpayerId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeArrears_ItemId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeArrears_TaxpayerId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandnotice_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.DropIndex(
                name: "IX_tbl_companyItem_TaxpayerId",
                table: "tbl_companyItem");

            migrationBuilder.DropIndex(
                name: "IX_tbl_company_TaxPayerCatgeoryId",
                table: "tbl_company");

            migrationBuilder.DropIndex(
                name: "IX_tbl_bank_lcda_BankId",
                table: "tbl_bank_lcda");

            migrationBuilder.DropIndex(
                name: "IX_tbl_bank_lcda_LcdaId",
                table: "tbl_bank_lcda");

            migrationBuilder.DropIndex(
                name: "IX_tbl_address_OwnerId",
                table: "tbl_address");

            migrationBuilder.DropColumn(
                name: "DemandNoticeId",
                table: "tbl_demandNoticeItem");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "tbl_demandnotice");

            migrationBuilder.DropColumn(
                name: "TaxPayerCatgeoryId",
                table: "tbl_company");

            migrationBuilder.AlterColumn<Guid>(
                name: "LcdaId",
                table: "tbl_bank_lcda",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "tbl_bank_lcda",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_DnTaxpayersDetailsId",
                table: "tbl_demandNoticeItem",
                column: "DnTaxpayersDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeItem_tbl_demandnotice_DnTaxpayersDetailsId",
                table: "tbl_demandNoticeItem",
                column: "DnTaxpayersDetailsId",
                principalTable: "tbl_demandnotice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
