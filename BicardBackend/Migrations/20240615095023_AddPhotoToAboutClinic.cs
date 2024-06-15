using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BicardBackend.Migrations
{
    public partial class AddPhotoToAboutClinic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PathToPhoto",
                table: "AboutClinic",
                newName: "PathToPhoto2");

            migrationBuilder.AddColumn<string>(
                name: "PathToPhoto1",
                table: "AboutClinic",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathToPhoto1",
                table: "AboutClinic");

            migrationBuilder.RenameColumn(
                name: "PathToPhoto2",
                table: "AboutClinic",
                newName: "PathToPhoto");
        }
    }
}
