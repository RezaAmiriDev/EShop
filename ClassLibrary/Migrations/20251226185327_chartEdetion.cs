using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class chartEdetion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "864a8741-6f33-4dff-8369-13d988f3fe1c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d62e479d-16b2-4d4e-83fd-7554b2238d98");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f43920b9-ad76-4e4b-a4ae-85c3ba859fc3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8562c20e-086f-49ca-b429-2233aa5a08f6", null, "seller", "SELLER" },
                    { "a8666923-8aba-4ca4-95c6-0a0ab317b319", null, "client", "CLIENT" },
                    { "f9255c3c-e761-4cf4-91aa-e20465a86a46", null, "admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8562c20e-086f-49ca-b429-2233aa5a08f6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8666923-8aba-4ca4-95c6-0a0ab317b319");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9255c3c-e761-4cf4-91aa-e20465a86a46");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "864a8741-6f33-4dff-8369-13d988f3fe1c", null, "client", "CLIENT" },
                    { "d62e479d-16b2-4d4e-83fd-7554b2238d98", null, "seller", "SELLER" },
                    { "f43920b9-ad76-4e4b-a4ae-85c3ba859fc3", null, "admin", "ADMIN" }
                });
        }
    }
}
