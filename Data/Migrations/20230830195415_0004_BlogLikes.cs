using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PenSword.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0004_BlogLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPostBlogUser",
                schema: "blog",
                columns: table => new
                {
                    LikedBlogPostsId = table.Column<int>(type: "integer", nullable: false),
                    UsersWhoLikeThisId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostBlogUser", x => new { x.LikedBlogPostsId, x.UsersWhoLikeThisId });
                    table.ForeignKey(
                        name: "FK_BlogPostBlogUser_AspNetUsers_UsersWhoLikeThisId",
                        column: x => x.UsersWhoLikeThisId,
                        principalSchema: "blog",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostBlogUser_BlogPosts_LikedBlogPostsId",
                        column: x => x.LikedBlogPostsId,
                        principalSchema: "blog",
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostBlogUser_UsersWhoLikeThisId",
                schema: "blog",
                table: "BlogPostBlogUser",
                column: "UsersWhoLikeThisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostBlogUser",
                schema: "blog");
        }
    }
}
