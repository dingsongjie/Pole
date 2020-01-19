using Microsoft.EntityFrameworkCore.Migrations;

namespace Backet.Api.Migrations
{
    public partial class Remove_BacketItem_FK_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BacketId",
                table: "BacketItem",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BacketId",
                table: "BacketItem",
                type: "character varying(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32);
        }
    }
}
