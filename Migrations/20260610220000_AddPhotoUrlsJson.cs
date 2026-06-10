using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawdoptApp.Migrations
{
    public partial class AddPhotoUrlsJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrlsJson",
                table: "PetListings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrlsJson",
                table: "PetListings");
        }
    }
}
