using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bilingo.Migrations
{
    public partial class userwords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWord_Users_UserId",
                table: "UserWord");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWord_Words_WordId",
                table: "UserWord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWord",
                table: "UserWord");

            migrationBuilder.RenameTable(
                name: "UserWord",
                newName: "UserWords");

            migrationBuilder.RenameIndex(
                name: "IX_UserWord_UserId",
                table: "UserWords",
                newName: "IX_UserWords_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWords",
                table: "UserWords",
                columns: new[] { "WordId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserWords_Users_UserId",
                table: "UserWords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWords_Words_WordId",
                table: "UserWords",
                column: "WordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWords_Users_UserId",
                table: "UserWords");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWords_Words_WordId",
                table: "UserWords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWords",
                table: "UserWords");

            migrationBuilder.RenameTable(
                name: "UserWords",
                newName: "UserWord");

            migrationBuilder.RenameIndex(
                name: "IX_UserWords_UserId",
                table: "UserWord",
                newName: "IX_UserWord_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWord",
                table: "UserWord",
                columns: new[] { "WordId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserWord_Users_UserId",
                table: "UserWord",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWord_Words_WordId",
                table: "UserWord",
                column: "WordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
