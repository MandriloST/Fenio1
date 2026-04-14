namespace Fenio1.Core.Entities;

/// <summary>
/// Staza / Circuit
/// </summary>
public class Circuit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;        // npr. "Circuit de Monaco"
    public string Location { get; set; } = string.Empty;    // npr. "Monte Carlo"
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty; // ISO npr. "MC"
    public int LapCount { get; set; }
    public decimal LengthKm { get; set; }

    public ICollection<Race> Races { get; set; } = new List<Race>();
}
