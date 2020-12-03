using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SForum.Data.Migrations
{
    public partial class UpdateApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "IsActive",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                "MemberSicne",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ProfileImage",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                "Rating",
                "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsActive",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "MemberSicne",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "ProfileImage",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "Rating",
                "AspNetUsers");
        }
    }
}