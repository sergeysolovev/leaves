using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ABC.Leaves.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    GmailLogin = table.Column<string>(nullable: false),
                    GoogleCalendarId = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.GmailLogin);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaves",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    GoogleAuthAccessToken = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeaves", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeLeaves");
        }
    }
}
