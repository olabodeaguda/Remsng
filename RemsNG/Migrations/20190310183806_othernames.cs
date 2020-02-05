using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class othernames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "OtherNames",
            //    table: "tbl_demandNoticePaymentHistory",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "OtherNames",
            //    table: "tbl_demandNoticePaymentHistory");
        }
    }
}
