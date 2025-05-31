using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3aa54ef2-5116-439d-b6ae-e30f7a8e80ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73c57ea3-0f99-40ec-8d4d-2459d521cf7a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "18dd3041-c439-42e9-a6d8-53d56a59a6a5", "1", "Admin", "Admin" },
                    { "25c7df2b-2ef0-4868-a2b2-8542fed9205a", "2", "Client", "Client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18dd3041-c439-42e9-a6d8-53d56a59a6a5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25c7df2b-2ef0-4868-a2b2-8542fed9205a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3aa54ef2-5116-439d-b6ae-e30f7a8e80ef", "1", "Admin", "Admin" },
                    { "73c57ea3-0f99-40ec-8d4d-2459d521cf7a", "2", "Client", "Client" }
                });
        }
    }
}
