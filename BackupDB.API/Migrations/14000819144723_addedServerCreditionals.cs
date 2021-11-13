using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackupDB.API.Migrations
{
    public partial class addedServerCreditionals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ServerPasswordHash",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ServerPasswordSalt",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerUsername",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerPasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServerPasswordSalt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServerUsername",
                table: "Users");
        }
    }
}
