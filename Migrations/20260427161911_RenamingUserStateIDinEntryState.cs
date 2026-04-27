using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class RenamingUserStateIDinEntryState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserStateId",
                table: "EntryStates",
                newName: "UserStatesEmbId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStates_JournalEntryPartId",
                table: "EntryStates",
                column: "JournalEntryPartId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryStates_UserStatesEmbId",
                table: "EntryStates",
                column: "UserStatesEmbId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryStates_EntryParts_JournalEntryPartId",
                table: "EntryStates",
                column: "JournalEntryPartId",
                principalTable: "EntryParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryStates_UserStates_UserStatesEmbId",
                table: "EntryStates",
                column: "UserStatesEmbId",
                principalTable: "UserStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryStates_EntryParts_JournalEntryPartId",
                table: "EntryStates");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryStates_UserStates_UserStatesEmbId",
                table: "EntryStates");

            migrationBuilder.DropIndex(
                name: "IX_EntryStates_JournalEntryPartId",
                table: "EntryStates");

            migrationBuilder.DropIndex(
                name: "IX_EntryStates_UserStatesEmbId",
                table: "EntryStates");

            migrationBuilder.RenameColumn(
                name: "UserStatesEmbId",
                table: "EntryStates",
                newName: "UserStateId");
        }
    }
}
