using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "603f0e91-58de-4395-8ee2-937319ffb9a1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8b30d80b-17ee-4bbf-8ee7-4872afc522d3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4c75fd3c-2163-4d0c-a753-52dafb027b01", "1", "Admin", "Admin" },
                    { "5472eb52-7931-4a41-85f0-24c03c7f0b88", "2", "Client", "Client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c75fd3c-2163-4d0c-a753-52dafb027b01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5472eb52-7931-4a41-85f0-24c03c7f0b88");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "603f0e91-58de-4395-8ee2-937319ffb9a1", "2", "Client", "Client" },
                    { "8b30d80b-17ee-4bbf-8ee7-4872afc522d3", "1", "Admin", "Admin" }
                });
        }
    }
}
