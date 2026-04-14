namespace Fenio1.Core.Entities;

/// <summary>
/// Rezultat kvalifikacija - top 3 pozicije
/// </summary>
public class QualifyingResult
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public int Position { get; set; }       // 1, 2 ili 3 (pole, 2nd, 3rd)
    public string? Q1Time { get; set; }
    public string? Q2Time { get; set; }
    public string? Q3Time { get; set; }
}

/// <summary>
/// Rezultat sprint utrke - top 3 pozicije
/// </summary>
public class SprintResult
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public int Position { get; set; }       // 1, 2 ili 3
    public bool DidNotFinish { get; set; } = false;
    public int? PointsScored { get; set; }
}

/// <summary>
/// Posebni rezultati utrke: DNF, Driver of the Day, Safety Car itd.
/// </summary>
public class RaceExtra
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public Race Race { get; set; } = null!;

    // Vozač koji nije završio (DNF)
    public int? DnfDriverId { get; set; }
    public Driver? DnfDriver { get; set; }

    // Vozač koji je postigao najbrži krug
    public int? FastestLapDriverId { get; set; }
    public Driver? FastestLapDriver { get; set; }

    // Vozač koji je ubilježio body (Scored Points - za prognoziranje tko će imati bodove)
    public int? ScoredPointsDriverId { get; set; }
    public Driver? ScoredPointsDriver { get; set; }

    // Vozač koji je imao najbrži pit stop
    public int? FastestPitStopDriverId { get; set; }
    public Driver? FastestPitStopDriver { get; set; }

    // Vozač dana
    public int? DriverOfTheDayId { get; set; }
    public Driver? DriverOfTheDay { get; set; }

    // Broj Safety Car izlazaka
    public int SafetyCarCount { get; set; } = 0;
}
