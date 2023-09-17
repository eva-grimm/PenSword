using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PenSword.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0007_AddedAuthorToBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                schema: "blog",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_AuthorId",
                schema: "blog",
                table: "BlogPosts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_AuthorId",
                schema: "blog",
                table: "BlogPosts",
                column: "AuthorId",
                principalSchema: "blog",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_AuthorId",
                schema: "blog",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_AuthorId",
                schema: "blog",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                schema: "blog",
                table: "BlogPosts");
        }
    }
}
