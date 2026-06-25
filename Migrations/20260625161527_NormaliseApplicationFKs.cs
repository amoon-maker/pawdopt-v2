using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawdoptApp.Migrations
{
    /// <inheritdoc />
    public partial class NormaliseApplicationFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "HardcodedPetId",
                table: "AdoptionApplications");

            migrationBuilder.DropColumn(
                name: "PetName",
                table: "AdoptionApplications");

            migrationBuilder.AlterColumn<int>(
                name: "PetListingId",
                table: "AdoptionApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PetListingId",
                table: "AdoptionApplications",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HardcodedPetId",
                table: "AdoptionApplications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetName",
                table: "AdoptionApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
