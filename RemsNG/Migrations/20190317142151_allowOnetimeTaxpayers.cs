using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class allowOnetimeTaxpayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsOneTime",
            //    table: "tbl_taxPayer",
            //    nullable: false,
            //    defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "IsOneTime",
            //    table: "tbl_taxPayer");

        }
    }
}
