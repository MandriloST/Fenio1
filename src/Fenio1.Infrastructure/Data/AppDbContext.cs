namespace Fenio1.Infrastructure.Data;

using Fenio1.Core.Entities;
using Fenio1.Core.Services;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // --- Tablice ---
    public DbSet<User> Users => Set<User>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Circuit> Circuits => Set<Circuit>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<RaceResult> RaceResults => Set<RaceResult>();
    public DbSet<QualifyingResult> QualifyingResults => Set<QualifyingResult>();
    public DbSet<SprintResult> SprintResults => Set<SprintResult>();
    public DbSet<RaceExtra> RaceExtras => Set<RaceExtra>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<PredictionRacePosition> PredictionRacePositions => Set<PredictionRacePosition>();
    public DbSet<PredictionQualifyingPosition> PredictionQualifyingPositions => Set<PredictionQualifyingPosition>();
    public DbSet<PredictionSprintPosition> PredictionSprintPositions => Set<PredictionSprintPosition>();
    public DbSet<DriverStanding> DriverStandings => Set<DriverStanding>();
    public DbSet<ConstructorStanding> ConstructorStandings => Set<ConstructorStanding>();
    public DbSet<PredictionLeaderboard> PredictionLeaderboards => Set<PredictionLeaderboard>();
    public DbSet<RaceDnfEntry> RaceDnfEntries => Set<RaceDnfEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- User ---
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<int>();
        });

        // --- Season ---
        modelBuilder.Entity<Season>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Year).IsUnique();
            e.Property(s => s.Name).HasMaxLength(100);
        });

        // --- Team ---
        modelBuilder.Entity<Team>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).HasMaxLength(100).IsRequired();
            e.Property(t => t.ShortName).HasMaxLength(50).IsRequired();
            e.Property(t => t.Nationality).HasMaxLength(50);
            e.Property(t => t.PrimaryColor).HasMaxLength(7); // HEX boja
        });

        // --- Driver ---
        modelBuilder.Entity<Driver>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Code).IsUnique();
            e.Property(d => d.FirstName).HasMaxLength(50).IsRequired();
            e.Property(d => d.LastName).HasMaxLength(50).IsRequired();
            e.Property(d => d.Code).HasMaxLength(3).IsRequired();
            e.Property(d => d.Nationality).HasMaxLength(50);
            e.Ignore(d => d.FullName); // Computed property, ne sprema se u bazu

            e.HasOne(d => d.Team)
                .WithMany(t => t.Drivers)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Circuit ---
        modelBuilder.Entity<Circuit>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.Property(c => c.Location).HasMaxLength(100);
            e.Property(c => c.Country).HasMaxLength(100);
            e.Property(c => c.CountryCode).HasMaxLength(2);
            e.Property(c => c.LengthKm).HasColumnType("decimal(6,3)");
        });

        // --- Race ---
        modelBuilder.Entity<Race>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => new { r.SeasonId, r.RoundNumber }).IsUnique();
            e.Property(r => r.Name).HasMaxLength(100).IsRequired();
            e.Property(r => r.Status).HasConversion<int>();
            e.Property(r => r.ExternalApiId).HasMaxLength(50);

            e.HasOne(r => r.Season)
                .WithMany(s => s.Races)
                .HasForeignKey(r => r.SeasonId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Circuit)
                .WithMany(c => c.Races)
                .HasForeignKey(r => r.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- RaceResult ---
        modelBuilder.Entity<RaceResult>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => new { r.RaceId, r.Position }).IsUnique();
            e.HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

            e.HasOne(r => r.Race)
                .WithMany(race => race.RaceResults)
                .HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Driver)
                .WithMany(d => d.RaceResults)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- QualifyingResult ---
        modelBuilder.Entity<QualifyingResult>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => new { r.RaceId, r.Position }).IsUnique();
            e.HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

            e.HasOne(r => r.Race)
                .WithMany(race => race.QualifyingResults)
                .HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Driver)
                .WithMany(d => d.QualifyingResults)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- SprintResult ---
        modelBuilder.Entity<SprintResult>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => new { r.RaceId, r.Position }).IsUnique();
            e.HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

            e.HasOne(r => r.Race)
                .WithMany(race => race.SprintResults)
                .HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.Driver)
                .WithMany(d => d.SprintResults)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- RaceExtra ---
        modelBuilder.Entity<RaceExtra>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => r.RaceId).IsUnique(); // Jedna RaceExtra per utrka

            e.HasOne(r => r.Race)
                .WithMany(race => race.RaceExtras)
                .HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nullable FK-ovi za vozače - bez kaskadnog brisanja
            //e.HasOne(r => r.DnfDriver).WithMany().HasForeignKey(r => r.DnfDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.FastestLapDriver).WithMany().HasForeignKey(r => r.FastestLapDriverId).OnDelete(DeleteBehavior.SetNull);
            //e.HasOne(r => r.ScoredPointsDriver).WithMany().HasForeignKey(r => r.ScoredPointsDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.FastestPitStopDriver).WithMany().HasForeignKey(r => r.FastestPitStopDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.DriverOfTheDay).WithMany().HasForeignKey(r => r.DriverOfTheDayId).OnDelete(DeleteBehavior.SetNull);
        });

        // --- Prediction ---
        modelBuilder.Entity<Prediction>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.UserId, p.RaceId }).IsUnique(); // Jedna prognoza per korisnik per utrka

            e.HasOne(p => p.User)
                .WithMany(u => u.Predictions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Race)
                .WithMany(r => r.Predictions)
                .HasForeignKey(p => p.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nullable FK-ovi za posebne prognoze
            e.HasOne(p => p.PredictedDnfDriver).WithMany().HasForeignKey(p => p.PredictedDnfDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.PredictedFastestLapDriver).WithMany().HasForeignKey(p => p.PredictedFastestLapDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.PredictedScoredPointsDriver).WithMany().HasForeignKey(p => p.PredictedScoredPointsDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.PredictedFastestPitStopDriver).WithMany().HasForeignKey(p => p.PredictedFastestPitStopDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.PredictedDriverOfTheDay).WithMany().HasForeignKey(p => p.PredictedDriverOfTheDayId).OnDelete(DeleteBehavior.SetNull);
        });

        // --- PredictionRacePosition ---
        modelBuilder.Entity<PredictionRacePosition>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.PredictionId, p.Position }).IsUnique();

            e.HasOne(p => p.Prediction)
                .WithMany(pred => pred.RacePositions)
                .HasForeignKey(p => p.PredictionId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Driver).WithMany().HasForeignKey(p => p.DriverId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- PredictionQualifyingPosition ---
        modelBuilder.Entity<PredictionQualifyingPosition>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.PredictionId, p.Position }).IsUnique();

            e.HasOne(p => p.Prediction)
                .WithMany(pred => pred.QualifyingPositions)
                .HasForeignKey(p => p.PredictionId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Driver).WithMany().HasForeignKey(p => p.DriverId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- PredictionSprintPosition ---
        modelBuilder.Entity<PredictionSprintPosition>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.PredictionId, p.Position }).IsUnique();

            e.HasOne(p => p.Prediction)
                .WithMany(pred => pred.SprintPositions)
                .HasForeignKey(p => p.PredictionId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Driver).WithMany().HasForeignKey(p => p.DriverId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- DriverStanding ---
        modelBuilder.Entity<DriverStanding>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => new { d.SeasonId, d.DriverId }).IsUnique();

            e.HasOne(d => d.Season).WithMany(s => s.DriverStandings).HasForeignKey(d => d.SeasonId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.Driver).WithMany(dr => dr.DriverStandings).HasForeignKey(d => d.DriverId).OnDelete(DeleteBehavior.Cascade);
        });

        // --- ConstructorStanding ---
        modelBuilder.Entity<ConstructorStanding>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => new { c.SeasonId, c.TeamId }).IsUnique();

            e.HasOne(c => c.Season).WithMany(s => s.ConstructorStandings).HasForeignKey(c => c.SeasonId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.Team).WithMany(t => t.ConstructorStandings).HasForeignKey(c => c.TeamId).OnDelete(DeleteBehavior.Cascade);
        });

        // --- PredictionLeaderboard ---
        modelBuilder.Entity<PredictionLeaderboard>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.UserId, p.SeasonId }).IsUnique();

            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Season).WithMany().HasForeignKey(p => p.SeasonId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RaceDnfEntry>(e =>
        {
            e.HasKey(d => d.Id);
            // Isti vozač može biti DNF samo jednom po utrci
            e.HasIndex(d => new { d.RaceId, d.DriverId }).IsUnique();

            e.HasOne(d => d.Race)
                .WithMany(r => r.DnfEntries)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.Driver)
                .WithMany()
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed - Admin korisnik (lozinka: Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@fenio1.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed - Demo korisnik (lozinka: User@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 2,
            Username = "user1",
            Email = "user1@fenio1.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed - Sezona 2025
        modelBuilder.Entity<Season>().HasData(new Season
        {
            Id = 1,
            Year = 2025,
            Name = "Formula 1 2025 Season",
            IsActive = true
        });

        // Seed - Timovi 2025
        modelBuilder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Red Bull Racing", ShortName = "Red Bull", Nationality = "Austrian", PrimaryColor = "#3671C6", IsActive = true },
            new Team { Id = 2, Name = "Scuderia Ferrari", ShortName = "Ferrari", Nationality = "Italian", PrimaryColor = "#E8002D", IsActive = true },
            new Team { Id = 3, Name = "Mercedes-AMG Petronas", ShortName = "Mercedes", Nationality = "German", PrimaryColor = "#27F4D2", IsActive = true },
            new Team { Id = 4, Name = "McLaren F1 Team", ShortName = "McLaren", Nationality = "British", PrimaryColor = "#FF8000", IsActive = true },
            new Team { Id = 5, Name = "Aston Martin Aramco", ShortName = "Aston Martin", Nationality = "British", PrimaryColor = "#229971", IsActive = true },
            new Team { Id = 6, Name = "BWT Alpine F1 Team", ShortName = "Alpine", Nationality = "French", PrimaryColor = "#0093CC", IsActive = true },
            new Team { Id = 7, Name = "Williams Racing", ShortName = "Williams", Nationality = "British", PrimaryColor = "#64C4FF", IsActive = true },
            new Team { Id = 8, Name = "Visa Cash App RB", ShortName = "RB", Nationality = "Italian", PrimaryColor = "#6692FF", IsActive = true },
            new Team { Id = 9, Name = "Stake F1 Team Kick Sauber", ShortName = "Sauber", Nationality = "Swiss", PrimaryColor = "#52E252", IsActive = true },
            new Team { Id = 10, Name = "MoneyGram Haas F1 Team", ShortName = "Haas", Nationality = "American", PrimaryColor = "#B6BABD", IsActive = true }
        );

        // Seed - Vozači 2025
        var dob = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Driver>().HasData(
            new Driver { Id = 1, FirstName = "Max", LastName = "Verstappen", Code = "VER", DriverNumber = 1, Nationality = "Dutch", TeamId = 1, IsActive = true, DateOfBirth = new DateTime(1997, 9, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 2, FirstName = "Sergio", LastName = "Perez", Code = "PER", DriverNumber = 11, Nationality = "Mexican", TeamId = 1, IsActive = true, DateOfBirth = new DateTime(1990, 1, 26, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 3, FirstName = "Charles", LastName = "Leclerc", Code = "LEC", DriverNumber = 16, Nationality = "Monégasque", TeamId = 2, IsActive = true, DateOfBirth = new DateTime(1997, 10, 16, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 4, FirstName = "Lewis", LastName = "Hamilton", Code = "HAM", DriverNumber = 44, Nationality = "British", TeamId = 2, IsActive = true, DateOfBirth = new DateTime(1985, 1, 7, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 5, FirstName = "George", LastName = "Russell", Code = "RUS", DriverNumber = 63, Nationality = "British", TeamId = 3, IsActive = true, DateOfBirth = new DateTime(1998, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 6, FirstName = "Kimi", LastName = "Antonelli", Code = "ANT", DriverNumber = 12, Nationality = "Italian", TeamId = 3, IsActive = true, DateOfBirth = new DateTime(2006, 8, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 7, FirstName = "Lando", LastName = "Norris", Code = "NOR", DriverNumber = 4, Nationality = "British", TeamId = 4, IsActive = true, DateOfBirth = new DateTime(1999, 11, 13, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 8, FirstName = "Oscar", LastName = "Piastri", Code = "PIA", DriverNumber = 81, Nationality = "Australian", TeamId = 4, IsActive = true, DateOfBirth = new DateTime(2001, 4, 6, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 9, FirstName = "Fernando", LastName = "Alonso", Code = "ALO", DriverNumber = 14, Nationality = "Spanish", TeamId = 5, IsActive = true, DateOfBirth = new DateTime(1981, 7, 29, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 10, FirstName = "Lance", LastName = "Stroll", Code = "STR", DriverNumber = 18, Nationality = "Canadian", TeamId = 5, IsActive = true, DateOfBirth = new DateTime(1998, 10, 29, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 11, FirstName = "Pierre", LastName = "Gasly", Code = "GAS", DriverNumber = 10, Nationality = "French", TeamId = 6, IsActive = true, DateOfBirth = new DateTime(1996, 2, 7, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 12, FirstName = "Jack", LastName = "Doohan", Code = "DOO", DriverNumber = 7, Nationality = "Australian", TeamId = 6, IsActive = true, DateOfBirth = new DateTime(2003, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 13, FirstName = "Alexander", LastName = "Albon", Code = "ALB", DriverNumber = 23, Nationality = "Thai", TeamId = 7, IsActive = true, DateOfBirth = new DateTime(1996, 3, 23, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 14, FirstName = "Carlos", LastName = "Sainz", Code = "SAI", DriverNumber = 55, Nationality = "Spanish", TeamId = 7, IsActive = true, DateOfBirth = new DateTime(1994, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 15, FirstName = "Yuki", LastName = "Tsunoda", Code = "TSU", DriverNumber = 22, Nationality = "Japanese", TeamId = 8, IsActive = true, DateOfBirth = new DateTime(2000, 5, 11, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 16, FirstName = "Liam", LastName = "Lawson", Code = "LAW", DriverNumber = 30, Nationality = "New Zealander", TeamId = 8, IsActive = true, DateOfBirth = new DateTime(2002, 2, 11, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 17, FirstName = "Nico", LastName = "Hülkenberg", Code = "HUL", DriverNumber = 27, Nationality = "German", TeamId = 9, IsActive = true, DateOfBirth = new DateTime(1987, 8, 19, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 18, FirstName = "Gabriel", LastName = "Bortoleto", Code = "BOR", DriverNumber = 5, Nationality = "Brazilian", TeamId = 9, IsActive = true, DateOfBirth = new DateTime(2004, 10, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 19, FirstName = "Esteban", LastName = "Ocon", Code = "OCO", DriverNumber = 31, Nationality = "French", TeamId = 10, IsActive = true, DateOfBirth = new DateTime(1996, 9, 17, 0, 0, 0, DateTimeKind.Utc) },
            new Driver { Id = 20, FirstName = "Oliver", LastName = "Bearman", Code = "BEA", DriverNumber = 87, Nationality = "British", TeamId = 10, IsActive = true, DateOfBirth = new DateTime(2005, 5, 8, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed - Staze 2025
        modelBuilder.Entity<Circuit>().HasData(
            new Circuit { Id = 1, Name = "Bahrain International Circuit", Location = "Sakhir", Country = "Bahrain", CountryCode = "BH", LapCount = 57, LengthKm = 5.412m },
            new Circuit { Id = 2, Name = "Jeddah Corniche Circuit", Location = "Jeddah", Country = "Saudi Arabia", CountryCode = "SA", LapCount = 50, LengthKm = 6.174m },
            new Circuit { Id = 3, Name = "Albert Park Circuit", Location = "Melbourne", Country = "Australia", CountryCode = "AU", LapCount = 58, LengthKm = 5.278m },
            new Circuit { Id = 4, Name = "Suzuka International Racing Course", Location = "Suzuka", Country = "Japan", CountryCode = "JP", LapCount = 53, LengthKm = 5.807m },
            new Circuit { Id = 5, Name = "Shanghai International Circuit", Location = "Shanghai", Country = "China", CountryCode = "CN", LapCount = 56, LengthKm = 5.451m },
            new Circuit { Id = 6, Name = "Miami International Autodrome", Location = "Miami", Country = "USA", CountryCode = "US", LapCount = 57, LengthKm = 5.412m },
            new Circuit { Id = 7, Name = "Autodromo Enzo e Dino Ferrari", Location = "Imola", Country = "Italy", CountryCode = "IT", LapCount = 63, LengthKm = 4.909m },
            new Circuit { Id = 8, Name = "Circuit de Monaco", Location = "Monte Carlo", Country = "Monaco", CountryCode = "MC", LapCount = 78, LengthKm = 3.337m },
            new Circuit { Id = 9, Name = "Circuit de Barcelona-Catalunya", Location = "Barcelona", Country = "Spain", CountryCode = "ES", LapCount = 66, LengthKm = 4.657m },
            new Circuit { Id = 10, Name = "Circuit Gilles Villeneuve", Location = "Montreal", Country = "Canada", CountryCode = "CA", LapCount = 70, LengthKm = 4.361m }
        );

        // Seed - Prvih 5 utrka sezone 2025
        modelBuilder.Entity<Race>().HasData(
            new Race { Id = 1, SeasonId = 1, CircuitId = 1, Name = "Bahrain Grand Prix", RoundNumber = 1, RaceDate = new DateTime(2025, 3, 2, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 1, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
            new Race { Id = 2, SeasonId = 1, CircuitId = 2, Name = "Saudi Arabian Grand Prix", RoundNumber = 2, RaceDate = new DateTime(2025, 3, 9, 20, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 8, 20, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
            new Race { Id = 3, SeasonId = 1, CircuitId = 3, Name = "Australian Grand Prix", RoundNumber = 3, RaceDate = new DateTime(2025, 3, 16, 6, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 15, 6, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
            new Race { Id = 4, SeasonId = 1, CircuitId = 5, Name = "Chinese Grand Prix", RoundNumber = 4, RaceDate = new DateTime(2025, 3, 23, 7, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 22, 7, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 3, 22, 11, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
            new Race { Id = 5, SeasonId = 1, CircuitId = 6, Name = "Miami Grand Prix", RoundNumber = 5, RaceDate = new DateTime(2025, 5, 4, 20, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 5, 3, 20, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 5, 3, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming }
        );
    }
}
