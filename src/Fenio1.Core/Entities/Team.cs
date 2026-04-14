namespace Fenio1.Core.Entities;

/// <summary>
/// Konstruktor / Tim Formule 1
/// </summary>
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;           // npr. "Red Bull Racing"
    public string ShortName { get; set; } = string.Empty;      // npr. "Red Bull"
    public string Nationality { get; set; } = string.Empty;
    public string? PrimaryColor { get; set; }                   // HEX boja za UI
    public bool IsActive { get; set; } = true;

    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<ConstructorStanding> ConstructorStandings { get; set; } = new List<ConstructorStanding>();
}
