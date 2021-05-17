using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_Blog.Data.Migrations
{
    public partial class _009AddCommentUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Comments",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Comments");
        }
    }
}
