using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class afterwar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "56ac1dce-f9d8-4257-b2ca-66700e918253");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d6b24f-53c8-438d-9053-a2c28607242f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd69039d-47a3-44d6-8545-6c4fcb2d1b11");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "415870e9-7202-4d45-b22c-df178f375de9", null, "admin", "ADMIN" },
                    { "4290ff77-8c3f-44a6-a4aa-169c811d8d1b", null, "seller", "SELLER" },
                    { "7bb485e3-d9c2-480e-aa9c-8a191c8f024d", null, "client", "CLIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "415870e9-7202-4d45-b22c-df178f375de9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4290ff77-8c3f-44a6-a4aa-169c811d8d1b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bb485e3-d9c2-480e-aa9c-8a191c8f024d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "56ac1dce-f9d8-4257-b2ca-66700e918253", null, "seller", "SELLER" },
                    { "a5d6b24f-53c8-438d-9053-a2c28607242f", null, "client", "CLIENT" },
                    { "cd69039d-47a3-44d6-8545-6c4fcb2d1b11", null, "admin", "ADMIN" }
                });
        }
    }
}
