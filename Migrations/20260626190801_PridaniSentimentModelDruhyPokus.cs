using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalTrack.Migrations
{
    /// <inheritdoc />
    public partial class PridaniSentimentModelDruhyPokus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoodSummaries");

            migrationBuilder.CreateTable(
                name: "Sentiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryPartId = table.Column<int>(type: "int", nullable: false),
                    MainPolarity = table.Column<int>(type: "int", nullable: false),
                    Positive = table.Column<double>(type: "float", nullable: false),
                    Neutral = table.Column<double>(type: "float", nullable: false),
                    Negative = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentiments_EntryParts_JournalEntryPartId",
                        column: x => x.JournalEntryPartId,
                        principalTable: "EntryParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sentiments_JournalEntryPartId",
                table: "Sentiments",
                column: "JournalEntryPartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sentiments");

            migrationBuilder.CreateTable(
                name: "MoodSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DayPhases = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserStates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeekDays = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoodSummaries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoodSummaries_UserId",
                table: "MoodSummaries",
                column: "UserId");
        }
    }
}
