using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddingFKInEntryStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EntryParts_JournalEntryId",
                table: "EntryParts",
                column: "JournalEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryParts_Entries_JournalEntryId",
                table: "EntryParts",
                column: "JournalEntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryParts_Entries_JournalEntryId",
                table: "EntryParts");

            migrationBuilder.DropIndex(
                name: "IX_EntryParts_JournalEntryId",
                table: "EntryParts");
        }
    }
}
