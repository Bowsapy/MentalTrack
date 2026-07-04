using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserStatesEmbToSentiment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentiments_EntryParts_JournalEntryPartId",
                table: "Sentiments");

            migrationBuilder.AlterColumn<int>(
                name: "JournalEntryPartId",
                table: "Sentiments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserStatesEmbId",
                table: "Sentiments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments",
                column: "UserStatesEmbId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentiments_EntryParts_JournalEntryPartId",
                table: "Sentiments",
                column: "JournalEntryPartId",
                principalTable: "EntryParts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentiments_UserStates_UserStatesEmbId",
                table: "Sentiments",
                column: "UserStatesEmbId",
                principalTable: "UserStates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentiments_EntryParts_JournalEntryPartId",
                table: "Sentiments");

            migrationBuilder.DropForeignKey(
                name: "FK_Sentiments_UserStates_UserStatesEmbId",
                table: "Sentiments");

            migrationBuilder.DropIndex(
                name: "IX_Sentiments_UserStatesEmbId",
                table: "Sentiments");

            migrationBuilder.DropColumn(
                name: "UserStatesEmbId",
                table: "Sentiments");

            migrationBuilder.AlterColumn<int>(
                name: "JournalEntryPartId",
                table: "Sentiments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sentiments_EntryParts_JournalEntryPartId",
                table: "Sentiments",
                column: "JournalEntryPartId",
                principalTable: "EntryParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
