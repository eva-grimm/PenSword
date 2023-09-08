using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PenSword.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0005_FixedLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostBlogUser",
                schema: "blog");

            migrationBuilder.CreateTable(
                name: "Likes",
                schema: "blog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlogPostId = table.Column<int>(type: "integer", nullable: false),
                    BlogUserId = table.Column<string>(type: "text", nullable: false),
                    IsLiked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_BlogUserId",
                        column: x => x.BlogUserId,
                        principalSchema: "blog",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalSchema: "blog",
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_BlogPostId",
                schema: "blog",
                table: "Likes",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_BlogUserId",
                schema: "blog",
                table: "Likes",
                column: "BlogUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes",
                schema: "blog");

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
    }
}
