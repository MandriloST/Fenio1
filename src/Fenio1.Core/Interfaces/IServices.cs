namespace Fenio1.Core.Interfaces;

using Fenio1.Core.Entities;

public interface IScoringService
{
    /// <summary>
    /// Izračunava bodove za prognozu jedne utrke.
    /// Svaki pogođeni podatak = 2 boda.
    /// </summary>
    Task<int> CalculatePredictionPointsAsync(int predictionId);

    /// <summary>
    /// Izračunava ukupne bodove za korisnika u sezoni.
    /// </summary>
    Task<int> GetTotalPointsForUserInSeasonAsync(int userId, int seasonId);

    /// <summary>
    /// Ažurira bodove za sve prognoze jedne utrke nakon unosa rezultata.
    /// </summary>
    Task RecalculateRacePointsAsync(int raceId);

    /// <summary>
    /// Ažurira leaderboard za sezonu.
    /// </summary>
    Task UpdateLeaderboardAsync(int seasonId);
}

public interface IExternalApiService
{
    /// <summary>
    /// Dohvaća rezultate utrke iz vanjskog API-a (npr. Ergast, OpenF1).
    /// Implementaciju logike mapiranja/prijevoda ćete napraviti sami.
    /// </summary>
    Task<ExternalRaceData?> FetchRaceResultsAsync(string externalRaceId);
}

/// <summary>
/// Privremeni model za podatke iz vanjskog API-a - popuniti prema odabranom API-u
/// </summary>
public class ExternalRaceData
{
    public string ExternalId { get; set; } = string.Empty;
    public List<ExternalDriverResult> RaceResults { get; set; } = new();
    public List<ExternalDriverResult> QualifyingResults { get; set; } = new();
    public List<ExternalDriverResult> SprintResults { get; set; } = new();
    public string? FastestLapDriverCode { get; set; }
    public string? DriverOfTheDayCode { get; set; }
    public string? FastestPitStopDriverCode { get; set; }
    public int SafetyCarCount { get; set; }
}

public class ExternalDriverResult
{
    public string DriverCode { get; set; } = string.Empty;  // npr. "VER"
    public int Position { get; set; }
    public bool DidNotFinish { get; set; }
    public string? DnfReason { get; set; }
}
