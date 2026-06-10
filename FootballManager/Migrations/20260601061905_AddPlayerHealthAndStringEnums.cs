using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballManager.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerHealthAndStringEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedReturnDate",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthNote",
                table: "Players",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthStatus",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PlayerMatchStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    MinutesPlayed = table.Column<int>(type: "int", nullable: false),
                    PassCompletionPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PsxGDiff = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SavePct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LaunchAccuracyPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccurateCrosses = table.Column<int>(type: "int", nullable: true),
                    TacklesWonPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceCovered = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AerialDuelsPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Clearances = table.Column<int>(type: "int", nullable: true),
                    Interceptions = table.Column<int>(type: "int", nullable: true),
                    LongPassPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BallRecoveries = table.Column<int>(type: "int", nullable: true),
                    PressuredPassPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KeyPasses = table.Column<int>(type: "int", nullable: true),
                    ShotCreatingActions = table.Column<int>(type: "int", nullable: true),
                    SuccessfulDribblesPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TouchesInPenaltyArea = table.Column<int>(type: "int", nullable: true),
                    ShotsPer90 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShotsOnTargetPct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConversionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTrainingStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TrainingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrillName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SprintSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnduranceScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceRun = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TechniqueScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PassAccuracy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShotsOnTarget = table.Column<int>(type: "int", nullable: true),
                    TacticsScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CoachRating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CoachNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrainingStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerTrainingStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_MatchId",
                table: "PlayerMatchStats",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_PlayerId_MatchId",
                table: "PlayerMatchStats",
                columns: new[] { "PlayerId", "MatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrainingStats_PlayerId",
                table: "PlayerTrainingStats",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerMatchStats");

            migrationBuilder.DropTable(
                name: "PlayerTrainingStats");

            migrationBuilder.DropColumn(
                name: "ExpectedReturnDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HealthNote",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HealthStatus",
                table: "Players");

            migrationBuilder.AlterColumn<int>(
                name: "Position",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
