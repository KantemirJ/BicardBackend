using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BicardBackend.Migrations
{
    public partial class MedService_SubMedService_OneToMany_Relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedServiceId",
                table: "Subs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subs_MedServiceId",
                table: "Subs",
                column: "MedServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subs_Meds_MedServiceId",
                table: "Subs",
                column: "MedServiceId",
                principalTable: "Meds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subs_Meds_MedServiceId",
                table: "Subs");

            migrationBuilder.DropIndex(
                name: "IX_Subs_MedServiceId",
                table: "Subs");

            migrationBuilder.DropColumn(
                name: "MedServiceId",
                table: "Subs");
        }
    }
}
