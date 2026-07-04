using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class addNavigationFromPartToSentiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sentiments_JournalEntryPartId",
                table: "Sentiments");

            migrationBuilder.DropIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_JournalEntryPartId",
                table: "Sentiments",
                column: "JournalEntryPartId",
                unique: true,
                filter: "[JournalEntryPartId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments",
                column: "UserStatesEmbId",
                unique: true,
                filter: "[UserStatesEmbId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sentiments_JournalEntryPartId",
                table: "Sentiments");

            migrationBuilder.DropIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_JournalEntryPartId",
                table: "Sentiments",
                column: "JournalEntryPartId");

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments",
                column: "UserStatesEmbId");
        }
    }
}
