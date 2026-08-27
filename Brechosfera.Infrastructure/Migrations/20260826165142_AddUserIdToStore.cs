using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brechosfera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibleName",
                table: "Stores");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "Stores",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_OwnerUserId",
                table: "Stores",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_UserCredentials_OwnerUserId",
                table: "Stores",
                column: "OwnerUserId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_UserCredentials_OwnerUserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_OwnerUserId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleName",
                table: "Stores",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
