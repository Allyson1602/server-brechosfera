using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brechosfera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreActiveNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_Name",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "ActiveName",
                table: "Stores",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                computedColumnSql: "CASE WHEN `DeletedAt` IS NULL THEN `Name` END",
                stored: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ActiveName",
                table: "Stores",
                column: "ActiveName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_ActiveName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ActiveName",
                table: "Stores");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Name",
                table: "Stores",
                column: "Name",
                unique: true);
        }
    }
}
