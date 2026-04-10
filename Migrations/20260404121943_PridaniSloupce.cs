using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class PridaniSloupce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "UserStates",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "UserStates");

            migrationBuilder.DropColumn(
                name: "UserStates",
                table: "Entries");
        }
    }
}
