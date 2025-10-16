using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class WensdayMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOperation",
                table: "Sellers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfOperation",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "23c529d8-4999-4f49-9f1b-07efd14bfc2b", null, "seller", "SELLER" },
                    { "4964af88-7082-4c65-9a52-186a52614c5b", null, "admin", "ADMIN" },
                    { "9480c77f-9189-4bc3-821f-ffd3453549e5", null, "client", "CLIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23c529d8-4999-4f49-9f1b-07efd14bfc2b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4964af88-7082-4c65-9a52-186a52614c5b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9480c77f-9189-4bc3-821f-ffd3453549e5");

            migrationBuilder.DropColumn(
                name: "DateOfOperation",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "DateOfOperation",
                table: "Products");

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
    }
}
