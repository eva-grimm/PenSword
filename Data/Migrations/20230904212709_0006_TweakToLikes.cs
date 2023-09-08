using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PenSword.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0006_TweakToLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_BlogUserId",
                schema: "blog",
                table: "Likes");

            migrationBuilder.AlterColumn<string>(
                name: "BlogUserId",
                schema: "blog",
                table: "Likes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_BlogUserId",
                schema: "blog",
                table: "Likes",
                column: "BlogUserId",
                principalSchema: "blog",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_BlogUserId",
                schema: "blog",
                table: "Likes");

            migrationBuilder.AlterColumn<string>(
                name: "BlogUserId",
                schema: "blog",
                table: "Likes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_BlogUserId",
                schema: "blog",
                table: "Likes",
                column: "BlogUserId",
                principalSchema: "blog",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
