namespace Fenio1.API.DTOs;

using Fenio1.Core.Entities;

// ========================
//  SEASON
// ========================

public record SeasonDto(int Id, int Year, string Name, bool IsActive);

public record CreateSeasonRequest(int Year, string Name);
public record UpdateSeasonRequest(string? Name, bool? IsActive);

// ========================
//  RACE
// ========================

public record RaceDto(
    int Id,
    int SeasonId,
    string SeasonName,
    int CircuitId,
    string CircuitName,
    string CircuitLocation,
    string CircuitCountry,
    string Name,
    int RoundNumber,
    DateTime RaceDate,
    DateTime? QualifyingDate,
    DateTime? SprintDate,
    bool HasSprint,
    string Status,
    string? ExternalApiId
);

public record RaceSummaryDto(
    int Id,
    string Name,
    int RoundNumber,
    DateTime RaceDate,
    bool HasSprint,
    string Status,
    string CircuitName,
    string CircuitCountry
);

public record CreateRaceRequest(
    int SeasonId,
    int CircuitId,
    string Name,
    int RoundNumber,
    DateTime RaceDate,
    DateTime? QualifyingDate,
    DateTime? SprintDate,
    bool HasSprint,
    string? ExternalApiId
);

public record UpdateRaceRequest(
    int? CircuitId,
    string? Name,
    int? RoundNumber,
    DateTime? RaceDate,
    DateTime? QualifyingDate,
    DateTime? SprintDate,
    bool? HasSprint,
    RaceStatus? Status,
    string? ExternalApiId
);

// ========================
//  RACE RESULTS (Admin unos)
// ========================

public record RaceResultEntryDto(int Position, int DriverId);
public record QualifyingResultEntryDto(int Position, int DriverId);
public record SprintResultEntryDto(int Position, int DriverId);

public record RaceExtrasDto(
    int? DnfDriverId,
    int? FastestLapDriverId,
    int? ScoredPointsDriverId,
    int? FastestPitStopDriverId,
    int? DriverOfTheDayId,
    int SafetyCarCount
);

/// <summary>
/// Kompletni unos rezultata utrke od strane admina.
/// Sadrži: poredak utrke (10 vozača), kvalifikacije (3), sprint (3 ako postoji), i posebne kategorije.
/// </summary>
public record SubmitRaceResultsRequest(
    List<RaceResultEntryDto> RacePositions,        // 10 vozača
    List<QualifyingResultEntryDto> QualifyingPositions,  // 3 vozača
    List<SprintResultEntryDto>? SprintPositions,   // 3 vozača, nullable ako nema sprinta
    RaceExtrasDto Extras
);

public record RaceResultsDto(
    int RaceId,
    string RaceName,
    List<RacePositionDetailDto> RacePositions,
    List<QualifyingPositionDetailDto> QualifyingPositions,
    List<SprintPositionDetailDto>? SprintPositions,
    RaceExtraDetailDto? Extras
);

public record RacePositionDetailDto(int Position, int DriverId, string DriverName, string DriverCode, bool DidNotFinish);
public record QualifyingPositionDetailDto(int Position, int DriverId, string DriverName, string DriverCode);
public record SprintPositionDetailDto(int Position, int DriverId, string DriverName, string DriverCode);

public record RaceExtraDetailDto(
    DriverSummaryDto? DnfDriver,
    DriverSummaryDto? FastestLapDriver,
    DriverSummaryDto? ScoredPointsDriver,
    DriverSummaryDto? FastestPitStopDriver,
    DriverSummaryDto? DriverOfTheDay,
    int SafetyCarCount
);

// ========================
//  PREDICTION (Unos prognoze korisnika)
// ========================

public record PredictionPositionDto(int Position, int DriverId);

/// <summary>
/// Kompletni unos prognoze od strane korisnika - ista struktura kao unos rezultata.
/// </summary>
public record SubmitPredictionRequest(
    List<PredictionPositionDto> RacePositions,       // Top 10 vozača
    List<PredictionPositionDto> QualifyingPositions, // Top 3 vozača
    List<PredictionPositionDto>? SprintPositions,    // Top 3 (ako ima sprint)
    int? PredictedDnfDriverId,
    int? PredictedFastestLapDriverId,
    int? PredictedScoredPointsDriverId,
    int? PredictedFastestPitStopDriverId,
    int? PredictedDriverOfTheDayId,
    int? PredictedSafetyCarCount
);

public record PredictionDto(
    int Id,
    int UserId,
    string Username,
    int RaceId,
    string RaceName,
    DateTime SubmittedAt,
    DateTime? UpdatedAt,
    bool IsLocked,
    int? TotalPointsForRace,
    List<PredictionPositionDetailDto> RacePositions,
    List<PredictionPositionDetailDto> QualifyingPositions,
    List<PredictionPositionDetailDto>? SprintPositions,
    PredictionExtrasDetailDto Extras
);

public record PredictionPositionDetailDto(int Position, int DriverId, string DriverName, string DriverCode, bool IsCorrect);

public record PredictionExtrasDetailDto(
    DriverSummaryDto? PredictedDnfDriver,
    DriverSummaryDto? PredictedFastestLapDriver,
    DriverSummaryDto? PredictedScoredPointsDriver,
    DriverSummaryDto? PredictedFastestPitStopDriver,
    DriverSummaryDto? PredictedDriverOfTheDay,
    int? PredictedSafetyCarCount
);

// ========================
//  SCORING / LEADERBOARD
// ========================

public record UserRaceScoreDto(
    int UserId,
    string Username,
    int RaceId,
    string RaceName,
    int PointsForRace,
    int CumulativePoints  // Zbroj za sve prethodne utrke + ova
);

public record LeaderboardEntryDto(
    int Position,
    int UserId,
    string Username,
    int TotalPoints,
    int RacesParticipated
);

public record RaceLeaderboardDto(
    int RaceId,
    string RaceName,
    List<RaceScoreEntryDto> Scores
);

public record RaceScoreEntryDto(
    int Position,
    string Username,
    int PointsForRace,
    int CumulativePoints
);
