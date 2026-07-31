using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class cartloginProb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3daa517e-a204-4eb3-a96d-0941550b4bac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e16e1c9-274a-478b-a375-b120645b9434");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b0ab0639-174b-4281-bd09-333195da1ccc");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3069eac0-ec5a-4b2d-889b-1212d919c7a9", null, "client", "CLIENT" },
                    { "32fa08f0-bf64-407f-9290-7df2dd68dad3", null, "seller", "SELLER" },
                    { "83be7cbb-dfd0-410d-b997-eac45ab4840c", null, "admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3069eac0-ec5a-4b2d-889b-1212d919c7a9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32fa08f0-bf64-407f-9290-7df2dd68dad3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83be7cbb-dfd0-410d-b997-eac45ab4840c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3daa517e-a204-4eb3-a96d-0941550b4bac", null, "admin", "ADMIN" },
                    { "3e16e1c9-274a-478b-a375-b120645b9434", null, "seller", "SELLER" },
                    { "b0ab0639-174b-4281-bd09-333195da1ccc", null, "client", "CLIENT" }
                });
        }
    }
}
