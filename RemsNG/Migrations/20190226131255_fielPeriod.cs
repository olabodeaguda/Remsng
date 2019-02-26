using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class fielPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnbilled",
                table: "tbl_demandNoticeTaxpayers",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "tbl_demandNoticeTaxpayers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "WardId",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true,
                defaultValue: default(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "StreetId",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true,
                defaultValue: default(Guid));

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnbilled",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                defaultValue: default(bool));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "tbl_demandnotice",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                defaultValue: default(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "PlainTextQuery",
                table: "tbl_demandnotice",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandnotice_StreetId",
                table: "tbl_demandnotice",
                column: "StreetId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_street_StreetId",
                table: "tbl_demandnotice");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice");

            migrationBuilder.DropIndex(
                name: "IX_tbl_demandnotice_StreetId",
                table: "tbl_demandnotice");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropColumn(
                name: "PlainTextQuery",
                table: "tbl_demandnotice");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnbilled",
                table: "tbl_demandNoticeTaxpayers",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<Guid>(
                name: "WardId",
                table: "tbl_demandnotice",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "StreetId",
                table: "tbl_demandnotice",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnbilled",
                table: "tbl_demandnotice",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "tbl_demandnotice",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_demandnotice_tbl_ward_WardId",
                table: "tbl_demandnotice",
                column: "WardId",
                principalTable: "tbl_ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
