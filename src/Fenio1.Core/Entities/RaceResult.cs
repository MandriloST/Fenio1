namespace Fenio1.Core.Entities;

/// <summary>
/// Rezultat utrke - poredak vozača (pozicije 1-10+ za bodove)
/// </summary>
public class RaceResult
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public Race Race { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    public int Position { get; set; }           // Finalna pozicija (1-20)
    public int? GridPosition { get; set; }      // Startna pozicija
    public bool DidNotFinish { get; set; } = false;
    public string? DnfReason { get; set; }      // Razlog odustajanja
    public int? PointsScored { get; set; }      // Stvarni F1 bodovi
    public bool FastestLap { get; set; } = false;
    public bool FastestPitStop { get; set; } = false;
    public string? LapTime { get; set; }        // Najbrži krug ako postoji
    public string? PitStopTime { get; set; }
}
