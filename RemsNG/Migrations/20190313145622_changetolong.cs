using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class changetolong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "BillingNumber",
                table: "tbl_demandNoticeTaxpayers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingNo",
                table: "tbl_demandNoticePenalty",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingNumber",
                table: "tbl_demandNoticePaymentHistory",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingNo",
                table: "tbl_demandNoticeItem",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingNumber",
                table: "tbl_DemandNoticeDownloadHistory",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BillingNo",
                table: "tbl_demandNoticeArrears",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BillingNumber",
                table: "tbl_demandNoticeTaxpayers",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "BillingNo",
                table: "tbl_demandNoticePenalty",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "BillingNumber",
                table: "tbl_demandNoticePaymentHistory",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "BillingNo",
                table: "tbl_demandNoticeItem",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "BillingNumber",
                table: "tbl_DemandNoticeDownloadHistory",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "BillingNo",
                table: "tbl_demandNoticeArrears",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
