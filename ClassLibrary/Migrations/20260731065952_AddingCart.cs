using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOperation",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3daa517e-a204-4eb3-a96d-0941550b4bac", null, "admin", "ADMIN" },
                    { "3e16e1c9-274a-478b-a375-b120645b9434", null, "seller", "SELLER" },
                    { "b0ab0639-174b-4281-bd09-333195da1ccc", null, "client", "CLIENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");

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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DateOfOperation",
                table: "CartItems");

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
    }
}
