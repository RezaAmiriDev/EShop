using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class AfterAddingPaginatio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOperation",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4428be42-848e-497e-bf5c-fcc406e2c549", null, "seller", "SELLER" },
                    { "88a050b4-5096-4b20-82e4-f810b8a74389", null, "client", "CLIENT" },
                    { "b5fb6592-d9d3-4b36-b6f1-9a711afd7f40", null, "admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4428be42-848e-497e-bf5c-fcc406e2c549");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88a050b4-5096-4b20-82e4-f810b8a74389");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5fb6592-d9d3-4b36-b6f1-9a711afd7f40");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateOfOperation",
                table: "Payments");

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
    }
}
