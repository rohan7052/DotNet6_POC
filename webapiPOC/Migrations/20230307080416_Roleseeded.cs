using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace webapiPOC.Migrations
{
    /// <inheritdoc />
    public partial class Roleseeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1c349875-cadf-40fe-aad9-6547401e37af", "2", "User", "User" },
                    { "4d871733-01a1-4f0e-8a77-f60e4c74d890", "3", "HR", "HR" },
                    { "cff1f37d-2560-4f8a-98c9-9450d23bb1bd", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c349875-cadf-40fe-aad9-6547401e37af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d871733-01a1-4f0e-8a77-f60e4c74d890");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cff1f37d-2560-4f8a-98c9-9450d23bb1bd");
        }
    }
}
