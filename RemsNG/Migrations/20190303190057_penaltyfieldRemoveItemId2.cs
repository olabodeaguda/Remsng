using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class penaltyfieldRemoveItemId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_item_ItemId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandNoticePenalty_ItemId",
                table: "tbl_demandNoticePenalty");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "tbl_demandNoticePenalty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "tbl_demandNoticePenalty",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePenalty_ItemId",
                table: "tbl_demandNoticePenalty",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandNoticePenalty_tbl_item_ItemId",
                table: "tbl_demandNoticePenalty",
                column: "ItemId",
                principalTable: "tbl_item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
