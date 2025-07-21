using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class dodataRola : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c75fd3c-2163-4d0c-a753-52dafb027b01",
                column: "ConcurrencyStamp",
                value: "770334ea-c116-4018-a346-954c0e6053fd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5472eb52-7931-4a41-85f0-24c03c7f0b88",
                column: "ConcurrencyStamp",
                value: "f796cb13-ce7d-444b-95cf-cbbfb77471bb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c75fd3c-2163-4d0c-a753-52dafb027b01",
                column: "ConcurrencyStamp",
                value: "49dcc1d6-9c00-4813-af36-32c443ffc74f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5472eb52-7931-4a41-85f0-24c03c7f0b88",
                column: "ConcurrencyStamp",
                value: "7678efad-0f1d-4a35-ab27-de85284969e1");
        }
    }
}
