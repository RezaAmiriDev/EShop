using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class cartloginProbLOMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3574a726-0fb2-43bc-897b-cbd2cd093be3", null, "client", "CLIENT" },
                    { "b9acce5e-f85a-460a-a4b4-97378ab0db65", null, "admin", "ADMIN" },
                    { "bf95cf00-d533-4880-b07f-414f648f4b51", null, "seller", "SELLER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3574a726-0fb2-43bc-897b-cbd2cd093be3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9acce5e-f85a-460a-a4b4-97378ab0db65");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf95cf00-d533-4880-b07f-414f648f4b51");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
    }
}
