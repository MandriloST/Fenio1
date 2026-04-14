namespace Fenio1.Core.Entities;

/// <summary>
/// Korisnik koji može unositi prognoze ili upravljati podacima (admin)
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}

public enum UserRole
{
    User = 0,
    Admin = 1
}
