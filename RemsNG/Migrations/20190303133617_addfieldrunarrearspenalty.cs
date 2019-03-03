using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class addfieldrunarrearspenalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRunArrears",
                table: "tbl_demandNoticeTaxpayers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRunPenalty",
                table: "tbl_demandNoticeTaxpayers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRunArrears",
                table: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropColumn(
                name: "IsRunPenalty",
                table: "tbl_demandNoticeTaxpayers");
        }
    }
}
