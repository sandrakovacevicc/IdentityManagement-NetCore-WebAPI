using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenCanBeNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c75fd3c-2163-4d0c-a753-52dafb027b01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5472eb52-7931-4a41-85f0-24c03c7f0b88");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "695ff1d8-f64c-4640-9535-c0cdd6684e36", "1", "Admin", "Admin" },
                    { "ed4b3ab7-d297-495c-a48e-169c700530e9", "2", "Client", "Client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "695ff1d8-f64c-4640-9535-c0cdd6684e36");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed4b3ab7-d297-495c-a48e-169c700530e9");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4c75fd3c-2163-4d0c-a753-52dafb027b01", "1", "Admin", "Admin" },
                    { "5472eb52-7931-4a41-85f0-24c03c7f0b88", "2", "Client", "Client" }
                });
        }
    }
}
