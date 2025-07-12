using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStatusFromResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "695ff1d8-f64c-4640-9535-c0cdd6684e36");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed4b3ab7-d297-495c-a48e-169c700530e9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f1c04c0-6421-437e-8896-28965225dea5", "2", "Client", "Client" },
                    { "c2e71ad6-3935-473b-b262-9283aab9ef9c", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "695ff1d8-f64c-4640-9535-c0cdd6684e36", "1", "Admin", "Admin" },
                    { "ed4b3ab7-d297-495c-a48e-169c700530e9", "2", "Client", "Client" }
                });
        }
    }
}
