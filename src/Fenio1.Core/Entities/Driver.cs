namespace Fenio1.Core.Entities;

/// <summary>
/// Vozač Formule 1
/// </summary>
public class Driver
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;      // npr. "VER", "HAM"
    public int DriverNumber { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigacijska svojstva
    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    public ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
    public ICollection<QualifyingResult> QualifyingResults { get; set; } = new List<QualifyingResult>();
    public ICollection<SprintResult> SprintResults { get; set; } = new List<SprintResult>();
    public ICollection<DriverStanding> DriverStandings { get; set; } = new List<DriverStanding>();

    public string FullName => $"{FirstName} {LastName}";
}
