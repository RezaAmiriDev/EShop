using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class MIGIn2025_05_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "469af424-38ba-4ec1-9032-a0016a0d85aa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6fab7ec-1fa1-40fd-9d4d-588290163c00");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fb5611be-2018-4ad7-9a02-a63137186a41");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2f1b87db-7218-4a67-a386-0a898aeb7957", null, "seller", "SELLER" },
                    { "8846b093-f7b6-4f51-97e1-e7a90543d11b", null, "admin", "ADMIN" },
                    { "8f3dbeeb-e192-46cd-a404-e8e6e0a6aa94", null, "client", "CLIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f1b87db-7218-4a67-a386-0a898aeb7957");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8846b093-f7b6-4f51-97e1-e7a90543d11b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f3dbeeb-e192-46cd-a404-e8e6e0a6aa94");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "469af424-38ba-4ec1-9032-a0016a0d85aa", null, "client", "CLIENT" },
                    { "f6fab7ec-1fa1-40fd-9d4d-588290163c00", null, "seller", "SELLER" },
                    { "fb5611be-2018-4ad7-9a02-a63137186a41", null, "admin", "ADMIN" }
                });
        }
    }
}
