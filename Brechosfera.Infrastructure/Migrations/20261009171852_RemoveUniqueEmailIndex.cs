using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brechosfera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCredentials_Email",
                table: "UserCredentials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_Email",
                table: "UserCredentials",
                column: "Email",
                unique: true);
        }
    }
}
