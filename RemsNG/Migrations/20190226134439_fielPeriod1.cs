using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class fielPeriod1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_tbl_demandnotice_tbl_street_StreetId",
            //    table: "tbl_demandnotice");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.AlterColumn<Guid>(
                name: "WardId",
                table: "tbl_demandnotice",
                nullable: true,
                oldClrType: typeof(Guid));

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "StreetId",
            //    table: "tbl_demandnotice",
            //    nullable: true,
            //    oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_street_StreetId",
                table: "tbl_demandnotice",
                column: "StreetId",
                principalTable: "tbl_street",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice",
                column: "WardId",
                principalTable: "tbl_ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_street_StreetId",
                table: "tbl_demandnotice");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.AlterColumn<Guid>(
                name: "WardId",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StreetId",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_street_StreetId",
                table: "tbl_demandnotice",
                column: "StreetId",
                principalTable: "tbl_street",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice",
                column: "WardId",
                principalTable: "tbl_ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
