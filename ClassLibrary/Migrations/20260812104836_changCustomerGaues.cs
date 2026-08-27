using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class changCustomerGaues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "307310a6-ab15-4d56-b686-e5c77eaa8836");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c800201-560e-482d-abee-e72cb7fdc346");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd81424b-9357-4e86-a868-1f478a8a9498");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "307310a6-ab15-4d56-b686-e5c77eaa8836", null, "client", "CLIENT" },
                    { "8c800201-560e-482d-abee-e72cb7fdc346", null, "admin", "ADMIN" },
                    { "bd81424b-9357-4e86-a868-1f478a8a9498", null, "seller", "SELLER" }
                });
        }
    }
}
