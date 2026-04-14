namespace Fenio1.Core.Entities;

/// <summary>
/// Poredak vozača u sezoni (kumulativni bodovi)
/// </summary>
public class DriverStanding
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public int Points { get; set; } = 0;
    public int Wins { get; set; } = 0;
    public int Podiums { get; set; } = 0;
    public int Position { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Poredak konstruktora u sezoni (kumulativni bodovi)
/// </summary>
public class ConstructorStanding
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int Points { get; set; } = 0;
    public int Wins { get; set; } = 0;
    public int Position { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Leaderboard prognoza korisnika - bodovi za prognoze po sezoni
/// </summary>
public class PredictionLeaderboard
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;

    public int TotalPoints { get; set; } = 0;
    public int RacesParticipated { get; set; } = 0;
    public int Position { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
