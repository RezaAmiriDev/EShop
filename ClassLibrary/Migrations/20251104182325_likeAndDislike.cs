using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class likeAndDislike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbb5672a-ab4e-43e8-b5ae-ee698067387f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c85f9fb9-ac0f-49fe-b12a-0e477dda0329");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e737f0d4-46d0-4b89-a007-e6efbbc72e93");

            migrationBuilder.AddColumn<int>(
                name: "DislikesCount",
                table: "Shops",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfproducts",
                table: "Shops",
                type: "int",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "DislikesCount",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "NumberOfproducts",
                table: "Shops");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "bbb5672a-ab4e-43e8-b5ae-ee698067387f", null, "admin", "ADMIN" },
                    { "c85f9fb9-ac0f-49fe-b12a-0e477dda0329", null, "client", "CLIENT" },
                    { "e737f0d4-46d0-4b89-a007-e6efbbc72e93", null, "seller", "SELLER" }
                });
        }
    }
}
