using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class NameAndSurname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c04c0-6421-437e-8896-28965225dea5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2e71ad6-3935-473b-b262-9283aab9ef9c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4c75fd3c-2163-4d0c-a753-52dafb027b01", "49dcc1d6-9c00-4813-af36-32c443ffc74f", "Admin", "ADMIN" },
                    { "5472eb52-7931-4a41-85f0-24c03c7f0b88", "7678efad-0f1d-4a35-ab27-de85284969e1", "Client", "CLIENT" }
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
                    { "5f1c04c0-6421-437e-8896-28965225dea5", "2", "Client", "Client" },
                    { "c2e71ad6-3935-473b-b262-9283aab9ef9c", "1", "Admin", "Admin" }
                });
        }
    }
}
