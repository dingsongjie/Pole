using Microsoft.EntityFrameworkCore.Migrations;

namespace Backet.Api.Migrations
{
    public partial class Modify_Backet_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacketItem_Backet_BacketUserId",
                table: "BacketItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BacketItem",
                table: "BacketItem");

            migrationBuilder.DropIndex(
                name: "IX_BacketItem_BacketUserId",
                table: "BacketItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Backet",
                table: "Backet");

            migrationBuilder.DropColumn(
                name: "BacketUserId",
                table: "BacketItem");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "BacketItem",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "BacketItem",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "BacketId",
                table: "BacketItem",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Backet",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BacketItem",
                table: "BacketItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Backet",
                table: "Backet",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_BacketId",
                table: "BacketItem",
                column: "BacketId");

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_ProductId",
                table: "BacketItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Backet_UserId",
                table: "Backet",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem",
                column: "BacketId",
                principalTable: "Backet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacketItem_Backet_BacketId",
                table: "BacketItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BacketItem",
                table: "BacketItem");

            migrationBuilder.DropIndex(
                name: "IX_BacketItem_BacketId",
                table: "BacketItem");

            migrationBuilder.DropIndex(
                name: "IX_BacketItem_ProductId",
                table: "BacketItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Backet",
                table: "Backet");

            migrationBuilder.DropIndex(
                name: "IX_Backet_UserId",
                table: "Backet");

            migrationBuilder.DropColumn(
                name: "BacketId",
                table: "BacketItem");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "BacketItem",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "BacketItem",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "BacketUserId",
                table: "BacketItem",
                type: "character varying(32)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Backet",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BacketItem",
                table: "BacketItem",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Backet",
                table: "Backet",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_BacketUserId",
                table: "BacketItem",
                column: "BacketUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BacketItem_Backet_BacketUserId",
                table: "BacketItem",
                column: "BacketUserId",
                principalTable: "Backet",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
