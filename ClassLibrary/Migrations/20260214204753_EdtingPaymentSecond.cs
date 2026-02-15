using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class EdtingPaymentSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "507011b0-6042-472c-a4bc-fcad6d7ef96b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bad22417-e648-43eb-9f44-53784cfe1ca6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2d00c3a-6d91-4194-bafb-1b74d2b18648");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2907db3a-3e20-4845-a68c-9d7bdc9739d2", null, "seller", "SELLER" },
                    { "32e801d9-353b-4718-818a-33719e10850f", null, "client", "CLIENT" },
                    { "d7e579ff-b72b-43a5-ae7e-dc954e4aa856", null, "admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2907db3a-3e20-4845-a68c-9d7bdc9739d2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32e801d9-353b-4718-818a-33719e10850f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7e579ff-b72b-43a5-ae7e-dc954e4aa856");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "507011b0-6042-472c-a4bc-fcad6d7ef96b", null, "seller", "SELLER" },
                    { "bad22417-e648-43eb-9f44-53784cfe1ca6", null, "admin", "ADMIN" },
                    { "f2d00c3a-6d91-4194-bafb-1b74d2b18648", null, "client", "CLIENT" }
                });
        }
    }
}
