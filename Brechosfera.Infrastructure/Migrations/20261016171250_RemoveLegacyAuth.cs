using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brechosfera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserCredentials_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_UserCredentials_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_UserCredentials_OwnerUserId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Storereviews_AspNetUsers_UserId",
                table: "Storereviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_OwnerUserId",
                table: "Stores",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_AspNetUsers_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Storereviews_AspNetUsers_UserId",
                table: "Storereviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_OwnerUserId",
                table: "Stores");

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Roles = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Username = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserCredentials_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_UserCredentials_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_UserCredentials_OwnerUserId",
                table: "Stores",
                column: "OwnerUserId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
