using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class billingNoPrepayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillingNo",
                table: "tbl_prepayment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingNo",
                table: "tbl_prepayment");
        }
    }
}
