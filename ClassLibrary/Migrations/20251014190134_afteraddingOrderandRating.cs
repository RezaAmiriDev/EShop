using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ModelLayer.Migrations
{
    /// <inheritdoc />
    public partial class afteraddingOrderandRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerLike_Shops_ShopId",
                table: "SellerLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerLike",
                table: "SellerLike");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "35e59390-dce9-4618-892d-281fb776cb13");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a36cad19-e7f9-4781-b7c1-1a4c97329687");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2a7039c-b0f0-4d91-8fc2-e781f3b7c748");

            migrationBuilder.RenameTable(
                name: "SellerLike",
                newName: "SellerLikes");

            migrationBuilder.RenameIndex(
                name: "IX_SellerLike_ShopId",
                table: "SellerLikes",
                newName: "IX_SellerLikes_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerLikes",
                table: "SellerLikes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfOperation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "bbb5672a-ab4e-43e8-b5ae-ee698067387f", null, "admin", "ADMIN" },
                    { "c85f9fb9-ac0f-49fe-b12a-0e477dda0329", null, "client", "CLIENT" },
                    { "e737f0d4-46d0-4b89-a007-e6efbbc72e93", null, "seller", "SELLER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SellerLikes_Shops_ShopId",
                table: "SellerLikes",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerLikes_Shops_ShopId",
                table: "SellerLikes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerLikes",
                table: "SellerLikes");

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

            migrationBuilder.RenameTable(
                name: "SellerLikes",
                newName: "SellerLike");

            migrationBuilder.RenameIndex(
                name: "IX_SellerLikes_ShopId",
                table: "SellerLike",
                newName: "IX_SellerLike_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerLike",
                table: "SellerLike",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "35e59390-dce9-4618-892d-281fb776cb13", null, "admin", "ADMIN" },
                    { "a36cad19-e7f9-4781-b7c1-1a4c97329687", null, "client", "CLIENT" },
                    { "d2a7039c-b0f0-4d91-8fc2-e781f3b7c748", null, "seller", "SELLER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SellerLike_Shops_ShopId",
                table: "SellerLike",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
