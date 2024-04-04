using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CharacterService.Migrations
{
    /// <inheritdoc />
    public partial class Init_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Warrior in life", "Warrior" },
                    { 2, "Rogue in life", "Rogue" },
                    { 3, "Mage in life", "Mage" },
                    { 4, "Priest in life", "Priest" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Class",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Class",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Class",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Class",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
