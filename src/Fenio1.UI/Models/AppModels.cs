namespace Fenio1.UI.Models;

// ========================
//  AUTH
// ========================

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

// ========================
//  SEASON / RACE
// ========================

public class SeasonDto
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class RaceSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public DateTime RaceDate { get; set; }
    public bool HasSprint { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CircuitName { get; set; } = string.Empty;
    public string CircuitCountry { get; set; } = string.Empty;
}

public class RaceDto
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public int CircuitId { get; set; }
    public string CircuitName { get; set; } = string.Empty;
    public string CircuitLocation { get; set; } = string.Empty;
    public string CircuitCountry { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public DateTime RaceDate { get; set; }
    public DateTime? QualifyingDate { get; set; }
    public DateTime? SprintDate { get; set; }
    public bool HasSprint { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ExternalApiId { get; set; }
}

// ========================
//  DRIVER / TEAM
// ========================

public class DriverSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DriverNumber { get; set; }
    public string? TeamName { get; set; }
}

public class DriverDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DriverNumber { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
}

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string? PrimaryColor { get; set; }
    public bool IsActive { get; set; }
}

public class CircuitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int LapCount { get; set; }
    public decimal LengthKm { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ========================
//  RESULTS
// ========================

public class RaceResultsDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public List<RacePositionDetailDto> RacePositions { get; set; } = new();
    public List<QualifyingPositionDetailDto> QualifyingPositions { get; set; } = new();
    public List<SprintPositionDetailDto>? SprintPositions { get; set; }
    public RaceExtraDetailDto? Extras { get; set; }
}

public class RacePositionDetailDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverCode { get; set; } = string.Empty;
    public bool DidNotFinish { get; set; }
}

public class QualifyingPositionDetailDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverCode { get; set; } = string.Empty;
}

public class SprintPositionDetailDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverCode { get; set; } = string.Empty;
}

public class RaceExtraDetailDto
{
    public List<DriverSummaryDto> DnfDrivers { get; set; } = new(); // lista
    public DriverSummaryDto? FastestLapDriver { get; set; }
    //public DriverSummaryDto? ScoredPointsDriver { get; set; }
    public DriverSummaryDto? FastestPitStopDriver { get; set; }
    public DriverSummaryDto? DriverOfTheDay { get; set; }
    public int SafetyCarCount { get; set; }
}

// ========================
//  PREDICTION
// ========================

public class PredictionPositionDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
}

public class SubmitPredictionRequest
{
    public List<PredictionPositionDto> RacePositions { get; set; } = new();
    public List<PredictionPositionDto> QualifyingPositions { get; set; } = new();
    public List<PredictionPositionDto>? SprintPositions { get; set; }
    public int? PredictedDnfDriverId { get; set; }
    public int? PredictedFastestLapDriverId { get; set; }
    public int? PredictedScoredPointsDriverId { get; set; }
    public int? PredictedFastestPitStopDriverId { get; set; }
    public int? PredictedDriverOfTheDayId { get; set; }
    public int? PredictedSafetyCarCount { get; set; }
}

public class PredictionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsLocked { get; set; }
    public int? TotalPointsForRace { get; set; }
    public List<PredictionPositionDetailDto> RacePositions { get; set; } = new();
    public List<PredictionPositionDetailDto> QualifyingPositions { get; set; } = new();
    public List<PredictionPositionDetailDto>? SprintPositions { get; set; }
    public PredictionExtrasDetailDto Extras { get; set; } = new();
}

public class PredictionPositionDetailDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverCode { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class PredictionExtrasDetailDto
{
    public DriverSummaryDto? PredictedDnfDriver { get; set; }
    public DriverSummaryDto? PredictedFastestLapDriver { get; set; }
    public DriverSummaryDto? PredictedScoredPointsDriver { get; set; }
    public DriverSummaryDto? PredictedFastestPitStopDriver { get; set; }
    public DriverSummaryDto? PredictedDriverOfTheDay { get; set; }
    public int? PredictedSafetyCarCount { get; set; }
}

// ========================
//  ADMIN - SUBMIT RESULTS
// ========================

public class RaceResultEntryDto
{
    public int Position { get; set; }
    public int DriverId { get; set; }
}

// NOVO - zasebni modeli za admin i prediction
public class AdminRaceExtrasDto  // za admin unos rezultata
{
    public List<int> DnfDriverIds { get; set; } = new();
    public int? FastestLapDriverId { get; set; }
    // ScoredPoints nema
    public int? FastestPitStopDriverId { get; set; }
    public int? DriverOfTheDayId { get; set; }
    public int SafetyCarCount { get; set; }
}

public class SubmitRaceResultsRequest
{
    public List<RaceResultEntryDto> RacePositions { get; set; } = new();
    public List<RaceResultEntryDto> QualifyingPositions { get; set; } = new();
    public List<RaceResultEntryDto>? SprintPositions { get; set; }
    public AdminRaceExtrasDto Extras { get; set; } = new(); // promijenjeni tip
}

// ========================
//  LEADERBOARD
// ========================

public class LeaderboardEntryDto
{
    public int Position { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int RacesParticipated { get; set; }
}

public class RaceLeaderboardDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public List<RaceScoreEntryDto> Scores { get; set; } = new();
}

public class RaceScoreEntryDto
{
    public int Position { get; set; }
    public string Username { get; set; } = string.Empty;
    public int PointsForRace { get; set; }
    public int CumulativePoints { get; set; }
}

// ========================
//  ADMIN - CREATE/UPDATE
// ========================

public class CreateDriverRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DriverNumber { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = new DateTime(1995, 1, 1);
    public int? TeamId { get; set; }
}

public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string? PrimaryColor { get; set; }
}

public class CreateRaceRequest
{
    public int SeasonId { get; set; }
    public int CircuitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public DateTime RaceDate { get; set; } = DateTime.Today;
    public DateTime? QualifyingDate { get; set; }
    public DateTime? SprintDate { get; set; }
    public bool HasSprint { get; set; }
    public string? ExternalApiId { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}

public class CreateCircuitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int LapCount { get; set; }
    public decimal LengthKm { get; set; }
}
