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
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    UserId = table.Column<string>(maxLength: 32, nullable: false),
                    TotalPrice = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Backet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BacketItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    ProductId = table.Column<string>(maxLength: 32, nullable: true),
                    ProductName = table.Column<string>(maxLength: 256, nullable: false),
                    Price = table.Column<long>(nullable: false),
                    BacketId = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacketItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BacketItem_Backet_BacketId",
                        column: x => x.BacketId,
                        principalTable: "Backet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Backet_UserId",
                table: "Backet",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_BacketId",
                table: "BacketItem",
                column: "BacketId");

            migrationBuilder.CreateIndex(
                name: "IX_BacketItem_ProductId",
                table: "BacketItem",
                column: "ProductId");
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
