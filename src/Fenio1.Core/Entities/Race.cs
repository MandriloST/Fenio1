namespace Fenio1.Core.Entities;

/// <summary>
/// Utrka Formule 1 unutar sezone
/// </summary>
public class Race
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int CircuitId { get; set; }
    public Circuit Circuit { get; set; } = null!;

    public string Name { get; set; } = string.Empty;       // npr. "Monaco Grand Prix"
    public int RoundNumber { get; set; }
    public DateTime RaceDate { get; set; }
    public DateTime? QualifyingDate { get; set; }
    public DateTime? SprintDate { get; set; }
    public bool HasSprint { get; set; } = false;
    public RaceStatus Status { get; set; } = RaceStatus.Upcoming;

    // Metapodaci za vanjski API
    public string? ExternalApiId { get; set; }             // ID iz npr. Ergast/OpenF1 API-a

    // Navigacijska svojstva
    public ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
    public ICollection<QualifyingResult> QualifyingResults { get; set; } = new List<QualifyingResult>();
    public ICollection<SprintResult> SprintResults { get; set; } = new List<SprintResult>();
    public ICollection<RaceExtra> RaceExtras { get; set; } = new List<RaceExtra>();
    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}

public enum RaceStatus
{
    Upcoming = 0,
    Completed = 1,
    Cancelled = 2
}
