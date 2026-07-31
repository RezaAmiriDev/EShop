using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class FirstModelsToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f9aef65-45fa-4472-a761-8c002f08c49e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23a32d2d-337e-46e8-a920-35c9a0f6e1cd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7448fc24-6520-47a4-906b-00aa681c1b3c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3c4ae53c-6417-4d14-a915-ff258d236f6c", null, "client", "CLIENT" },
                    { "81bd9593-5e99-483b-8d41-5553f187fcdb", null, "admin", "ADMIN" },
                    { "b416a4ad-447b-4b3e-beaa-a10b14710141", null, "seller", "SELLER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c4ae53c-6417-4d14-a915-ff258d236f6c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "81bd9593-5e99-483b-8d41-5553f187fcdb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b416a4ad-447b-4b3e-beaa-a10b14710141");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f9aef65-45fa-4472-a761-8c002f08c49e", null, "admin", "ADMIN" },
                    { "23a32d2d-337e-46e8-a920-35c9a0f6e1cd", null, "client", "CLIENT" },
                    { "7448fc24-6520-47a4-906b-00aa681c1b3c", null, "seller", "SELLER" }
                });
        }
    }
}
