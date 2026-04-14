namespace Fenio1.Core.Entities;

/// <summary>
/// Sezona Formule 1 (npr. 2024, 2025)
/// </summary>
public class Season
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Name { get; set; } = string.Empty; // npr. "Formula 1 2025 Season"
    public bool IsActive { get; set; } = false;

    public ICollection<Race> Races { get; set; } = new List<Race>();
    public ICollection<DriverStanding> DriverStandings { get; set; } = new List<DriverStanding>();
    public ICollection<ConstructorStanding> ConstructorStandings { get; set; } = new List<ConstructorStanding>();
}
