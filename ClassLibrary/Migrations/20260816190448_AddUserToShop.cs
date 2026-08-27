using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ac08ca4-8391-45ab-8792-a9481e136a13");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d1e68e2-141c-4a20-b324-994c7a61afbe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a6031b5-354c-4d2d-ba50-15e5cd7c00aa");

            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "Shops",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "admin", "ADMIN" },
                    { "2", null, "client", "CLIENT" },
                    { "3", null, "seller", "SELLER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shops_SellerId",
                table: "Shops",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_AspNetUsers_SellerId",
                table: "Shops",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_AspNetUsers_SellerId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_SellerId",
                table: "Shops");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Shops");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ac08ca4-8391-45ab-8792-a9481e136a13", null, "admin", "ADMIN" },
                    { "2d1e68e2-141c-4a20-b324-994c7a61afbe", null, "client", "CLIENT" },
                    { "6a6031b5-354c-4d2d-ba50-15e5cd7c00aa", null, "seller", "SELLER" }
                });
        }
    }
}
