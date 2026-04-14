namespace Fenio1.Core.Entities;

/// <summary>
/// Prognoza korisnika za određenu utrku
/// </summary>
public class Prediction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsLocked { get; set; } = false; // Admin zaključava nakon utrke

    // Izračunati bodovi (popunjava se nakon završetka utrke)
    public int? TotalPointsForRace { get; set; }

    // Prognoze poretka u utrci (top 10)
    public ICollection<PredictionRacePosition> RacePositions { get; set; } = new List<PredictionRacePosition>();

    // Prognoze kvalifikacija (top 3)
    public ICollection<PredictionQualifyingPosition> QualifyingPositions { get; set; } = new List<PredictionQualifyingPosition>();

    // Prognoze sprint utrke (top 3, ako postoji)
    public ICollection<PredictionSprintPosition> SprintPositions { get; set; } = new List<PredictionSprintPosition>();

    // Posebne prognoze
    public int? PredictedDnfDriverId { get; set; }
    public Driver? PredictedDnfDriver { get; set; }

    public int? PredictedFastestLapDriverId { get; set; }
    public Driver? PredictedFastestLapDriver { get; set; }

    public int? PredictedScoredPointsDriverId { get; set; }
    public Driver? PredictedScoredPointsDriver { get; set; }

    public int? PredictedFastestPitStopDriverId { get; set; }
    public Driver? PredictedFastestPitStopDriver { get; set; }

    public int? PredictedDriverOfTheDayId { get; set; }
    public Driver? PredictedDriverOfTheDay { get; set; }

    public int? PredictedSafetyCarCount { get; set; }
}

/// <summary>
/// Prognozirana pozicija u utrci (1-10)
/// </summary>
public class PredictionRacePosition
{
    public int Id { get; set; }
    public int PredictionId { get; set; }
    public Prediction Prediction { get; set; } = null!;

    public int Position { get; set; }    // 1-10
    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
}

/// <summary>
/// Prognozirana pozicija u kvalifikacijama (1-3)
/// </summary>
public class PredictionQualifyingPosition
{
    public int Id { get; set; }
    public int PredictionId { get; set; }
    public Prediction Prediction { get; set; } = null!;

    public int Position { get; set; }    // 1-3
    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
}

/// <summary>
/// Prognozirana pozicija u sprint utrci (1-3)
/// </summary>
public class PredictionSprintPosition
{
    public int Id { get; set; }
    public int PredictionId { get; set; }
    public Prediction Prediction { get; set; } = null!;

    public int Position { get; set; }    // 1-3
    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
}
