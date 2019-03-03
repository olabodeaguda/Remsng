using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class removeItemIdArrears : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_item_ItemId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticeArrears_ItemId",
                table: "tbl_demandNoticeArrears");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "tbl_demandNoticeArrears");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "tbl_demandNoticeArrears",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeArrears_ItemId",
                table: "tbl_demandNoticeArrears",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticeArrears_tbl_item_ItemId",
                table: "tbl_demandNoticeArrears",
                column: "ItemId",
                principalTable: "tbl_item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
