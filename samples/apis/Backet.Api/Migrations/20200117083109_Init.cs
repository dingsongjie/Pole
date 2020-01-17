using Microsoft.EntityFrameworkCore.Migrations;

namespace Backet.Api.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Backet",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 32, nullable: false),
                    Id = table.Column<string>(maxLength: 32, nullable: true),
                    TotalPrice = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Backet", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "BacketItem",
                columns: table => new
                {
                    ProductId = table.Column<string>(maxLength: 32, nullable: false),
                    Id = table.Column<string>(maxLength: 32, nullable: true),
                    ProductName = table.Column<string>(maxLength: 256, nullable: false),
                    Price = table.Column<long>(nullable: false),
                    BacketUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacketItem", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_BacketItem_Backet_BacketUserId",
                        column: x => x.BacketUserId,
                        principalTable: "Backet",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_BacketUserId",
                table: "BacketItem",
                column: "BacketUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BacketItem");

            migrationBuilder.DropTable(
                name: "Backet");
        }
    }
}
