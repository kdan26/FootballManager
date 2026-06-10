using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballManager.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RatedByUserId = table.Column<int>(type: "int", nullable: false),
                    RatingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: true),
                    TrainingStatsId = table.Column<int>(type: "int", nullable: true),
                    RatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttitudeRating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FitnessRating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TechnicalRating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TacticalRating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPublishedToPlayer = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_PlayerTrainingStats_TrainingStatsId",
                        column: x => x.TrainingStatsId,
                        principalTable: "PlayerTrainingStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PerformanceRatings_Users_RatedByUserId",
                        column: x => x.RatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamTactics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Formation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTactics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamTactics_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_MatchId",
                table: "PerformanceRatings",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_PlayerId_RatingDate",
                table: "PerformanceRatings",
                columns: new[] { "PlayerId", "RatingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_RatedByUserId",
                table: "PerformanceRatings",
                column: "RatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRatings_TrainingStatsId",
                table: "PerformanceRatings",
                column: "TrainingStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTactics_TeamId",
                table: "TeamTactics",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceRatings");

            migrationBuilder.DropTable(
                name: "TeamTactics");
        }
    }
}
