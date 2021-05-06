using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_Blog.Data.Migrations
{
    public partial class _006AddPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Posts",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Posts");
        }
    }
}
