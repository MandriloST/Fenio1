using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fenio1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Circuits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    LapCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LengthKm = table.Column<decimal>(type: "decimal(6,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circuits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PrimaryColor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeasonId = table.Column<int>(type: "INTEGER", nullable: false),
                    CircuitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RoundNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    RaceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    QualifyingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SprintDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HasSprint = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ExternalApiId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Races_Circuits_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "Circuits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConstructorStandings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeasonId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructorStandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstructorStandings_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConstructorStandings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    DriverNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PredictionLeaderboards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    RacesParticipated = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionLeaderboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionLeaderboards_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PredictionLeaderboards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverStandings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeasonId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    Podiums = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverStandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverStandings_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverStandings_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Predictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    TotalPointsForRace = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedDnfDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedFastestLapDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedScoredPointsDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedFastestPitStopDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedDriverOfTheDayId = table.Column<int>(type: "INTEGER", nullable: true),
                    PredictedSafetyCarCount = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Predictions_Drivers_PredictedDnfDriverId",
                        column: x => x.PredictedDnfDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Predictions_Drivers_PredictedDriverOfTheDayId",
                        column: x => x.PredictedDriverOfTheDayId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Predictions_Drivers_PredictedFastestLapDriverId",
                        column: x => x.PredictedFastestLapDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Predictions_Drivers_PredictedFastestPitStopDriverId",
                        column: x => x.PredictedFastestPitStopDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Predictions_Drivers_PredictedScoredPointsDriverId",
                        column: x => x.PredictedScoredPointsDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Predictions_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Predictions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualifyingResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Q1Time = table.Column<string>(type: "TEXT", nullable: true),
                    Q2Time = table.Column<string>(type: "TEXT", nullable: true),
                    Q3Time = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualifyingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualifyingResults_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualifyingResults_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DnfDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    FastestLapDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScoredPointsDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    FastestPitStopDriverId = table.Column<int>(type: "INTEGER", nullable: true),
                    DriverOfTheDayId = table.Column<int>(type: "INTEGER", nullable: true),
                    SafetyCarCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Drivers_DnfDriverId",
                        column: x => x.DnfDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Drivers_DriverOfTheDayId",
                        column: x => x.DriverOfTheDayId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Drivers_FastestLapDriverId",
                        column: x => x.FastestLapDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Drivers_FastestPitStopDriverId",
                        column: x => x.FastestPitStopDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Drivers_ScoredPointsDriverId",
                        column: x => x.ScoredPointsDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RaceExtras_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    GridPosition = table.Column<int>(type: "INTEGER", nullable: true),
                    DidNotFinish = table.Column<bool>(type: "INTEGER", nullable: false),
                    DnfReason = table.Column<string>(type: "TEXT", nullable: true),
                    PointsScored = table.Column<int>(type: "INTEGER", nullable: true),
                    FastestLap = table.Column<bool>(type: "INTEGER", nullable: false),
                    FastestPitStop = table.Column<bool>(type: "INTEGER", nullable: false),
                    LapTime = table.Column<string>(type: "TEXT", nullable: true),
                    PitStopTime = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceResults_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaceResults_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    DidNotFinish = table.Column<bool>(type: "INTEGER", nullable: false),
                    PointsScored = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintResults_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SprintResults_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionQualifyingPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PredictionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionQualifyingPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionQualifyingPositions_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredictionQualifyingPositions_Predictions_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionRacePositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PredictionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionRacePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionRacePositions_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredictionRacePositions_Predictions_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionSprintPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PredictionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionSprintPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionSprintPositions_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredictionSprintPositions_Predictions_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Circuits",
                columns: new[] { "Id", "Country", "CountryCode", "LapCount", "LengthKm", "Location", "Name" },
                values: new object[,]
                {
                    { 1, "Bahrain", "BH", 57, 5.412m, "Sakhir", "Bahrain International Circuit" },
                    { 2, "Saudi Arabia", "SA", 50, 6.174m, "Jeddah", "Jeddah Corniche Circuit" },
                    { 3, "Australia", "AU", 58, 5.278m, "Melbourne", "Albert Park Circuit" },
                    { 4, "Japan", "JP", 53, 5.807m, "Suzuka", "Suzuka International Racing Course" },
                    { 5, "China", "CN", 56, 5.451m, "Shanghai", "Shanghai International Circuit" },
                    { 6, "USA", "US", 57, 5.412m, "Miami", "Miami International Autodrome" },
                    { 7, "Italy", "IT", 63, 4.909m, "Imola", "Autodromo Enzo e Dino Ferrari" },
                    { 8, "Monaco", "MC", 78, 3.337m, "Monte Carlo", "Circuit de Monaco" },
                    { 9, "Spain", "ES", 66, 4.657m, "Barcelona", "Circuit de Barcelona-Catalunya" },
                    { 10, "Canada", "CA", 70, 4.361m, "Montreal", "Circuit Gilles Villeneuve" }
                });

            migrationBuilder.InsertData(
                table: "Seasons",
                columns: new[] { "Id", "IsActive", "Name", "Year" },
                values: new object[] { 1, true, "Formula 1 2025 Season", 2025 });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "IsActive", "Name", "Nationality", "PrimaryColor", "ShortName" },
                values: new object[,]
                {
                    { 1, true, "Red Bull Racing", "Austrian", "#3671C6", "Red Bull" },
                    { 2, true, "Scuderia Ferrari", "Italian", "#E8002D", "Ferrari" },
                    { 3, true, "Mercedes-AMG Petronas", "German", "#27F4D2", "Mercedes" },
                    { 4, true, "McLaren F1 Team", "British", "#FF8000", "McLaren" },
                    { 5, true, "Aston Martin Aramco", "British", "#229971", "Aston Martin" },
                    { 6, true, "BWT Alpine F1 Team", "French", "#0093CC", "Alpine" },
                    { 7, true, "Williams Racing", "British", "#64C4FF", "Williams" },
                    { 8, true, "Visa Cash App RB", "Italian", "#6692FF", "RB" },
                    { 9, true, "Stake F1 Team Kick Sauber", "Swiss", "#52E252", "Sauber" },
                    { 10, true, "MoneyGram Haas F1 Team", "American", "#B6BABD", "Haas" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@fenio1.com", true, "$2a$11$JahuhaS3JzM37Jl0lAmTqe6mzZW.w7M8J9lBZVsvSHAUiqAmvYk9O", 1, "admin" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "user1@fenio1.com", true, "$2a$11$ECvhVBe0IePbMVg3UAaAkuZC2g3BIjw1QcYRW4P1AwLygItqr7sw.", 0, "user1" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Code", "DateOfBirth", "DriverNumber", "FirstName", "IsActive", "LastName", "Nationality", "TeamId" },
                values: new object[,]
                {
                    { 1, "VER", new DateTime(1997, 9, 30, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Max", true, "Verstappen", "Dutch", 1 },
                    { 2, "PER", new DateTime(1990, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), 11, "Sergio", true, "Perez", "Mexican", 1 },
                    { 3, "LEC", new DateTime(1997, 10, 16, 0, 0, 0, 0, DateTimeKind.Utc), 16, "Charles", true, "Leclerc", "Monégasque", 2 },
                    { 4, "HAM", new DateTime(1985, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), 44, "Lewis", true, "Hamilton", "British", 2 },
                    { 5, "RUS", new DateTime(1998, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 63, "George", true, "Russell", "British", 3 },
                    { 6, "ANT", new DateTime(2006, 8, 25, 0, 0, 0, 0, DateTimeKind.Utc), 12, "Kimi", true, "Antonelli", "Italian", 3 },
                    { 7, "NOR", new DateTime(1999, 11, 13, 0, 0, 0, 0, DateTimeKind.Utc), 4, "Lando", true, "Norris", "British", 4 },
                    { 8, "PIA", new DateTime(2001, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), 81, "Oscar", true, "Piastri", "Australian", 4 },
                    { 9, "ALO", new DateTime(1981, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 14, "Fernando", true, "Alonso", "Spanish", 5 },
                    { 10, "STR", new DateTime(1998, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), 18, "Lance", true, "Stroll", "Canadian", 5 },
                    { 11, "GAS", new DateTime(1996, 2, 7, 0, 0, 0, 0, DateTimeKind.Utc), 10, "Pierre", true, "Gasly", "French", 6 },
                    { 12, "DOO", new DateTime(2003, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), 7, "Jack", true, "Doohan", "Australian", 6 },
                    { 13, "ALB", new DateTime(1996, 3, 23, 0, 0, 0, 0, DateTimeKind.Utc), 23, "Alexander", true, "Albon", "Thai", 7 },
                    { 14, "SAI", new DateTime(1994, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), 55, "Carlos", true, "Sainz", "Spanish", 7 },
                    { 15, "TSU", new DateTime(2000, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), 22, "Yuki", true, "Tsunoda", "Japanese", 8 },
                    { 16, "LAW", new DateTime(2002, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), 30, "Liam", true, "Lawson", "New Zealander", 8 },
                    { 17, "HUL", new DateTime(1987, 8, 19, 0, 0, 0, 0, DateTimeKind.Utc), 27, "Nico", true, "Hülkenberg", "German", 9 },
                    { 18, "BOR", new DateTime(2004, 10, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Gabriel", true, "Bortoleto", "Brazilian", 9 },
                    { 19, "OCO", new DateTime(1996, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), 31, "Esteban", true, "Ocon", "French", 10 },
                    { 20, "BEA", new DateTime(2005, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), 87, "Oliver", true, "Bearman", "British", 10 }
                });

            migrationBuilder.InsertData(
                table: "Races",
                columns: new[] { "Id", "CircuitId", "ExternalApiId", "HasSprint", "Name", "QualifyingDate", "RaceDate", "RoundNumber", "SeasonId", "SprintDate", "Status" },
                values: new object[,]
                {
                    { 1, 1, null, false, "Bahrain Grand Prix", new DateTime(2025, 3, 1, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 2, 15, 0, 0, 0, DateTimeKind.Utc), 1, 1, null, 0 },
                    { 2, 2, null, false, "Saudi Arabian Grand Prix", new DateTime(2025, 3, 8, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 9, 20, 0, 0, 0, DateTimeKind.Utc), 2, 1, null, 0 },
                    { 3, 3, null, false, "Australian Grand Prix", new DateTime(2025, 3, 15, 6, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 16, 6, 0, 0, 0, DateTimeKind.Utc), 3, 1, null, 0 },
                    { 4, 5, null, true, "Chinese Grand Prix", new DateTime(2025, 3, 22, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 23, 7, 0, 0, 0, DateTimeKind.Utc), 4, 1, new DateTime(2025, 3, 22, 11, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 5, 6, null, true, "Miami Grand Prix", new DateTime(2025, 5, 3, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 4, 20, 0, 0, 0, DateTimeKind.Utc), 5, 1, new DateTime(2025, 5, 3, 15, 0, 0, 0, DateTimeKind.Utc), 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConstructorStandings_SeasonId_TeamId",
                table: "ConstructorStandings",
                columns: new[] { "SeasonId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConstructorStandings_TeamId",
                table: "ConstructorStandings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Code",
                table: "Drivers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TeamId",
                table: "Drivers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverStandings_DriverId",
                table: "DriverStandings",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverStandings_SeasonId_DriverId",
                table: "DriverStandings",
                columns: new[] { "SeasonId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionLeaderboards_SeasonId",
                table: "PredictionLeaderboards",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionLeaderboards_UserId_SeasonId",
                table: "PredictionLeaderboards",
                columns: new[] { "UserId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionQualifyingPositions_DriverId",
                table: "PredictionQualifyingPositions",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionQualifyingPositions_PredictionId_Position",
                table: "PredictionQualifyingPositions",
                columns: new[] { "PredictionId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionRacePositions_DriverId",
                table: "PredictionRacePositions",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionRacePositions_PredictionId_Position",
                table: "PredictionRacePositions",
                columns: new[] { "PredictionId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_PredictedDnfDriverId",
                table: "Predictions",
                column: "PredictedDnfDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_PredictedDriverOfTheDayId",
                table: "Predictions",
                column: "PredictedDriverOfTheDayId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_PredictedFastestLapDriverId",
                table: "Predictions",
                column: "PredictedFastestLapDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_PredictedFastestPitStopDriverId",
                table: "Predictions",
                column: "PredictedFastestPitStopDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_PredictedScoredPointsDriverId",
                table: "Predictions",
                column: "PredictedScoredPointsDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_RaceId",
                table: "Predictions",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_UserId_RaceId",
                table: "Predictions",
                columns: new[] { "UserId", "RaceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionSprintPositions_DriverId",
                table: "PredictionSprintPositions",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionSprintPositions_PredictionId_Position",
                table: "PredictionSprintPositions",
                columns: new[] { "PredictionId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualifyingResults_DriverId",
                table: "QualifyingResults",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_QualifyingResults_RaceId_DriverId",
                table: "QualifyingResults",
                columns: new[] { "RaceId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualifyingResults_RaceId_Position",
                table: "QualifyingResults",
                columns: new[] { "RaceId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_DnfDriverId",
                table: "RaceExtras",
                column: "DnfDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_DriverOfTheDayId",
                table: "RaceExtras",
                column: "DriverOfTheDayId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_FastestLapDriverId",
                table: "RaceExtras",
                column: "FastestLapDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_FastestPitStopDriverId",
                table: "RaceExtras",
                column: "FastestPitStopDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_RaceId",
                table: "RaceExtras",
                column: "RaceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaceExtras_ScoredPointsDriverId",
                table: "RaceExtras",
                column: "ScoredPointsDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceResults_DriverId",
                table: "RaceResults",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceResults_RaceId_DriverId",
                table: "RaceResults",
                columns: new[] { "RaceId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaceResults_RaceId_Position",
                table: "RaceResults",
                columns: new[] { "RaceId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Races_CircuitId",
                table: "Races",
                column: "CircuitId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_SeasonId_RoundNumber",
                table: "Races",
                columns: new[] { "SeasonId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_Year",
                table: "Seasons",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintResults_DriverId",
                table: "SprintResults",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintResults_RaceId_DriverId",
                table: "SprintResults",
                columns: new[] { "RaceId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintResults_RaceId_Position",
                table: "SprintResults",
                columns: new[] { "RaceId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructorStandings");

            migrationBuilder.DropTable(
                name: "DriverStandings");

            migrationBuilder.DropTable(
                name: "PredictionLeaderboards");

            migrationBuilder.DropTable(
                name: "PredictionQualifyingPositions");

            migrationBuilder.DropTable(
                name: "PredictionRacePositions");

            migrationBuilder.DropTable(
                name: "PredictionSprintPositions");

            migrationBuilder.DropTable(
                name: "QualifyingResults");

            migrationBuilder.DropTable(
                name: "RaceExtras");

            migrationBuilder.DropTable(
                name: "RaceResults");

            migrationBuilder.DropTable(
                name: "SprintResults");

            migrationBuilder.DropTable(
                name: "Predictions");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Circuits");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
