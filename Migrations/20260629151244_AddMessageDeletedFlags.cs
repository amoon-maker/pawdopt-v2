using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawdoptApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageDeletedFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeletedForReceiver",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeletedForSender",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedForReceiver",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeletedForSender",
                table: "Messages");
        }
    }
}
