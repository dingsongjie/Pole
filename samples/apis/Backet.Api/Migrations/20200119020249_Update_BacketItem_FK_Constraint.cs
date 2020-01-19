using Microsoft.EntityFrameworkCore.Migrations;

namespace Backet.Api.Migrations
{
    public partial class Update_BacketItem_FK_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem");

            migrationBuilder.AddForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem",
                column: "BacketId",
                principalTable: "Backet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem");

            migrationBuilder.AddForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem",
                column: "BacketId",
                principalTable: "Backet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
