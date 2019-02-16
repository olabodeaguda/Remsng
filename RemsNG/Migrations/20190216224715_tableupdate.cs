using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RemsNG.Migrations
{
    public partial class tableupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_batchDownloadRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BatchFileName = table.Column<string>(nullable: true),
                    BatchNo = table.Column<string>(nullable: true),
                    Createdby = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: true),
                    RequestStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_batchDownloadRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_cloudData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNumber = table.Column<string>(nullable: true),
                    DataTitle = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DomainId = table.Column<Guid>(nullable: false),
                    JsonContent = table.Column<string>(nullable: true),
                    SyncStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_cloudData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_contactPerson",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    TaxPayerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_contactPerson", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticePenalty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    BillingNo = table.Column<string>(nullable: true),
                    BillingYear = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemPenaltyStatus = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    OriginatedYear = table.Column<int>(nullable: false),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticePenalty", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_batchDownloadRequest");

            migrationBuilder.DropTable(
                name: "tbl_cloudData");

            migrationBuilder.DropTable(
                name: "tbl_contactPerson");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticePenalty");
        }
    }
}
