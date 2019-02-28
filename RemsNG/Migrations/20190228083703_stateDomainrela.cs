using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class stateDomainrela : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_lcda_DomainId",
                table: "tbl_lcda",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_domain_StateId",
                table: "tbl_domain",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_address_StreetId",
                table: "tbl_address",
                column: "StreetId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_address_tbl_street_StreetId",
                table: "tbl_address",
                column: "StreetId",
                principalTable: "tbl_street",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_domain_tbl_state_StateId",
                table: "tbl_domain",
                column: "StateId",
                principalTable: "tbl_state",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_lcda_tbl_domain_DomainId",
                table: "tbl_lcda",
                column: "DomainId",
                principalTable: "tbl_domain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_address_tbl_street_StreetId",
                table: "tbl_address");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_domain_tbl_state_StateId",
                table: "tbl_domain");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_lcda_tbl_domain_DomainId",
                table: "tbl_lcda");

            migrationBuilder.DropIndex(
                name: "IX_tbl_lcda_DomainId",
                table: "tbl_lcda");

            migrationBuilder.DropIndex(
                name: "IX_tbl_domain_StateId",
                table: "tbl_domain");

            migrationBuilder.DropIndex(
                name: "IX_tbl_address_StreetId",
                table: "tbl_address");
        }
    }
}
