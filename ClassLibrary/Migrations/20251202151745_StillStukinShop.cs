using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class StillStukinShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54d8cfbd-42fc-4e62-87ad-3639bbbbf36e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b22a2204-7dc5-4946-9ec4-e48b39ae5c3d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dfd892ed-1dce-41c2-a6dc-081a9bb3a136");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "54d8cfbd-42fc-4e62-87ad-3639bbbbf36e", null, "seller", "SELLER" },
                    { "b22a2204-7dc5-4946-9ec4-e48b39ae5c3d", null, "admin", "ADMIN" },
                    { "dfd892ed-1dce-41c2-a6dc-081a9bb3a136", null, "client", "CLIENT" }
                });
        }
    }
}
