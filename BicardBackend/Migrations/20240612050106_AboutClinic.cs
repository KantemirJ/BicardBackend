﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BicardBackend.Migrations
{
    public partial class AboutClinic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clinicstats");

            migrationBuilder.CreateTable(
                name: "AboutClinic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Intro = table.Column<string>(type: "text", nullable: false),
                    NumberOfBeds = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPatients = table.Column<int>(type: "integer", nullable: false),
                    NumberOfEmployees = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutClinic", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutClinic");

            migrationBuilder.CreateTable(
                name: "Clinicstats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumberOfBeds = table.Column<int>(type: "integer", nullable: false),
                    NumberOfEmployees = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPatients = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicstats", x => x.Id);
                });
        }
    }
}
