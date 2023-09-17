using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PenSword.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0008_AddedAuthorPropsAndSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Byline",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHub",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "blog",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Byline",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Company",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Facebook",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GitHub",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Instagram",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LinkedIn",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Twitter",
                schema: "blog",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "blog",
                table: "AspNetUsers");
        }
    }
}
