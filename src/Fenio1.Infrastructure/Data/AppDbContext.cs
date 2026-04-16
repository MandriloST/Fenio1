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
    public DbSet<RaceDnfEntry> RaceDnfEntries => Set<RaceDnfEntry>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<PredictionRacePosition> PredictionRacePositions => Set<PredictionRacePosition>();
    public DbSet<PredictionQualifyingPosition> PredictionQualifyingPositions => Set<PredictionQualifyingPosition>();
    public DbSet<PredictionSprintPosition> PredictionSprintPositions => Set<PredictionSprintPosition>();
    public DbSet<DriverStanding> DriverStandings => Set<DriverStanding>();
    public DbSet<ConstructorStanding> ConstructorStandings => Set<ConstructorStanding>();
    public DbSet<PredictionLeaderboard> PredictionLeaderboards => Set<PredictionLeaderboard>();

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
            e.HasOne(r => r.FastestLapDriver).WithMany().HasForeignKey(r => r.FastestLapDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.FastestPitStopDriver).WithMany().HasForeignKey(r => r.FastestPitStopDriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.DriverOfTheDay).WithMany().HasForeignKey(r => r.DriverOfTheDayId).OnDelete(DeleteBehavior.SetNull);
        });

        // --- RaceDnfEntry ---
        modelBuilder.Entity<RaceDnfEntry>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => new { d.RaceId, d.DriverId }).IsUnique(); // isti vozač može biti DNF samo jednom po utrci

            e.HasOne(d => d.Race)
                .WithMany(r => r.DnfEntries)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.Driver)
                .WithMany()
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
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

        // Seed data
        SeedData(modelBuilder);
    }

private static void SeedData(ModelBuilder modelBuilder)
        {
            // ========================
            //  KORISNICI
            // ========================
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@fenio1.com", PasswordHash = "$2a$11$JahuhaS3JzM37Jl0lAmTqe6mzZW.w7M8J9lBZVsvSHAUiqAmvYk9O", Role = UserRole.Admin, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 2, Username = "user1", Email = "user1@fenio1.com", PasswordHash = "$2a$11$ECvhVBe0IePbMVg3UAaAkuZC2g3BIjw1QcYRW4P1AwLygItqr7sw.", Role = UserRole.User, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 3, Username = "ivan", Email = "ivan.marasov@gmail.com", PasswordHash = "$2a$11$TrTOTp5Hf2eXGbmemWFVieALSO6cHEybeBeB1BYEpNQ2G9LWeFYNq", Role = UserRole.User, IsActive = true, CreatedAt = new DateTime(2026, 4, 14, 9, 9, 10, DateTimeKind.Utc) },
                new User { Id = 4, Username = "petar", Email = "petar.baskovic@gmail.com", PasswordHash = "$2a$11$FmLfoMwlZhDHHubRW8HV5ePwUvM2bOiPNjMWTQt0esSTYaPg9WFqe", Role = UserRole.User, IsActive = true, CreatedAt = new DateTime(2026, 4, 14, 9, 9, 30, DateTimeKind.Utc) }
            );

            // ========================
            //  SEZONA
            // ========================
            modelBuilder.Entity<Season>().HasData(
                new Season { Id = 1, Year = 2026, Name = "Formula 1 2026 Season", IsActive = true }
            );

            // ========================
            //  TIMOVI
            // ========================
            modelBuilder.Entity<Team>().HasData(
                new Team { Id = 1, Name = "Red Bull Racing", ShortName = "Red Bull", Nationality = "Austrian", PrimaryColor = "#3671C6", IsActive = true },
                new Team { Id = 2, Name = "Scuderia Ferrari", ShortName = "Ferrari", Nationality = "Italian", PrimaryColor = "#E8002D", IsActive = true },
                new Team { Id = 3, Name = "Mercedes-AMG Petronas", ShortName = "Mercedes", Nationality = "German", PrimaryColor = "#27F4D2", IsActive = true },
                new Team { Id = 4, Name = "McLaren F1 Team", ShortName = "McLaren", Nationality = "British", PrimaryColor = "#FF8000", IsActive = true },
                new Team { Id = 5, Name = "Aston Martin Aramco", ShortName = "Aston Martin", Nationality = "British", PrimaryColor = "#229971", IsActive = true },
                new Team { Id = 6, Name = "BWT Alpine F1 Team", ShortName = "Alpine", Nationality = "French", PrimaryColor = "#0093CC", IsActive = true },
                new Team { Id = 7, Name = "Williams Racing", ShortName = "Williams", Nationality = "British", PrimaryColor = "#64C4FF", IsActive = true },
                new Team { Id = 8, Name = "Visa Cash App RB", ShortName = "RB", Nationality = "Italian", PrimaryColor = "#6692FF", IsActive = true },
                new Team { Id = 9, Name = "Audi", ShortName = "Audi", Nationality = "Swiss", PrimaryColor = "#52E252", IsActive = true },
                new Team { Id = 10, Name = "Haas F1 Team", ShortName = "Haas", Nationality = "American", PrimaryColor = "#B6BABD", IsActive = true },
                new Team { Id = 11, Name = "Cadillac", ShortName = "Cadillac", Nationality = "American", PrimaryColor = "#B6BABD", IsActive = true }
            );

            // ========================
            //  VOZAČI
            // ========================
            modelBuilder.Entity<Driver>().HasData(
                new Driver { Id = 1, FirstName = "Max", LastName = "Verstappen", Code = "VER", DriverNumber = 1, Nationality = "Dutch", DateOfBirth = new DateTime(1997, 9, 30, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 1 },
                new Driver { Id = 2, FirstName = "Sergio", LastName = "Perez", Code = "PER", DriverNumber = 11, Nationality = "Mexican", DateOfBirth = new DateTime(1990, 1, 26, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 11 },
                new Driver { Id = 3, FirstName = "Charles", LastName = "Leclerc", Code = "LEC", DriverNumber = 16, Nationality = "Monégasque", DateOfBirth = new DateTime(1997, 10, 16, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 2 },
                new Driver { Id = 4, FirstName = "Lewis", LastName = "Hamilton", Code = "HAM", DriverNumber = 44, Nationality = "British", DateOfBirth = new DateTime(1985, 1, 7, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 2 },
                new Driver { Id = 5, FirstName = "George", LastName = "Russell", Code = "RUS", DriverNumber = 63, Nationality = "British", DateOfBirth = new DateTime(1998, 2, 15, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 3 },
                new Driver { Id = 6, FirstName = "Kimi", LastName = "Antonelli", Code = "ANT", DriverNumber = 12, Nationality = "Italian", DateOfBirth = new DateTime(2006, 8, 25, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 3 },
                new Driver { Id = 7, FirstName = "Lando", LastName = "Norris", Code = "NOR", DriverNumber = 4, Nationality = "British", DateOfBirth = new DateTime(1999, 11, 13, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 4 },
                new Driver { Id = 8, FirstName = "Oscar", LastName = "Piastri", Code = "PIA", DriverNumber = 81, Nationality = "Australian", DateOfBirth = new DateTime(2001, 4, 6, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 4 },
                new Driver { Id = 9, FirstName = "Fernando", LastName = "Alonso", Code = "ALO", DriverNumber = 14, Nationality = "Spanish", DateOfBirth = new DateTime(1981, 7, 29, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 5 },
                new Driver { Id = 10, FirstName = "Lance", LastName = "Stroll", Code = "STR", DriverNumber = 18, Nationality = "Canadian", DateOfBirth = new DateTime(1998, 10, 29, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 5 },
                new Driver { Id = 11, FirstName = "Pierre", LastName = "Gasly", Code = "GAS", DriverNumber = 10, Nationality = "French", DateOfBirth = new DateTime(1996, 2, 7, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 6 },
                new Driver { Id = 12, FirstName = "Franco", LastName = "Colapinto", Code = "COL", DriverNumber = 43, Nationality = "Argentinian", DateOfBirth = new DateTime(2003, 1, 20, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 6 },
                new Driver { Id = 13, FirstName = "Alexander", LastName = "Albon", Code = "ALB", DriverNumber = 23, Nationality = "Thai", DateOfBirth = new DateTime(1996, 3, 23, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 7 },
                new Driver { Id = 14, FirstName = "Carlos", LastName = "Sainz", Code = "SAI", DriverNumber = 55, Nationality = "Spanish", DateOfBirth = new DateTime(1994, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 7 },
                new Driver { Id = 15, FirstName = "Arvid", LastName = "Lindblad", Code = "LIN", DriverNumber = 41, Nationality = "British", DateOfBirth = new DateTime(2000, 5, 11, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 8 },
                new Driver { Id = 16, FirstName = "Liam", LastName = "Lawson", Code = "LAW", DriverNumber = 30, Nationality = "New Zealander", DateOfBirth = new DateTime(2002, 2, 11, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 8 },
                new Driver { Id = 17, FirstName = "Nico", LastName = "Hülkenberg", Code = "HUL", DriverNumber = 27, Nationality = "German", DateOfBirth = new DateTime(1987, 8, 19, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 9 },
                new Driver { Id = 18, FirstName = "Gabriel", LastName = "Bortoleto", Code = "BOR", DriverNumber = 5, Nationality = "Brazilian", DateOfBirth = new DateTime(2004, 10, 14, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 9 },
                new Driver { Id = 19, FirstName = "Esteban", LastName = "Ocon", Code = "OCO", DriverNumber = 31, Nationality = "French", DateOfBirth = new DateTime(1996, 9, 17, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 10 },
                new Driver { Id = 20, FirstName = "Oliver", LastName = "Bearman", Code = "BEA", DriverNumber = 87, Nationality = "British", DateOfBirth = new DateTime(2005, 5, 8, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 10 },
                new Driver { Id = 21, FirstName = "Valteri", LastName = "Bottas", Code = "BOT", DriverNumber = 77, Nationality = "Finn", DateOfBirth = new DateTime(2003, 3, 3, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 11 },
                new Driver { Id = 22, FirstName = "Isack", LastName = "Hadjar", Code = "HAD", DriverNumber = 6, Nationality = "French", DateOfBirth = new DateTime(2003, 3, 3, 0, 0, 0, DateTimeKind.Utc), IsActive = true, TeamId = 1 }
            );

            // ========================
            //  STAZE
            // ========================
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
                new Circuit { Id = 10, Name = "Circuit Gilles Villeneuve", Location = "Montreal", Country = "Canada", CountryCode = "CA", LapCount = 70, LengthKm = 4.361m },
                new Circuit { Id = 11, Name = "Red Bull Ring", Location = "Spielberg", Country = "Austria", CountryCode = "AT", LapCount = 71, LengthKm = 4.318m },
                new Circuit { Id = 12, Name = "Silverstone Circuit", Location = "Silverstone", Country = "Great Britain", CountryCode = "GB", LapCount = 52, LengthKm = 5.891m },
                new Circuit { Id = 13, Name = "Circuit de Spa-Francorchamps", Location = "Stavelot", Country = "Belgium", CountryCode = "BE", LapCount = 44, LengthKm = 7.004m },
                new Circuit { Id = 14, Name = "Hungaroring", Location = "Mogyoród", Country = "Hungary", CountryCode = "HU", LapCount = 70, LengthKm = 4.381m },
                new Circuit { Id = 15, Name = "Circuit Zandvoort", Location = "Zandvoort", Country = "Netherlands", CountryCode = "NL", LapCount = 72, LengthKm = 4.259m },
                new Circuit { Id = 16, Name = "Monza Circuit", Location = "Monza", Country = "Italy", CountryCode = "IT", LapCount = 53, LengthKm = 5.793m },
                new Circuit { Id = 17, Name = "Madring", Location = "Madrid", Country = "Spain", CountryCode = "ES", LapCount = 55, LengthKm = 5.47m },
                new Circuit { Id = 18, Name = "Baku City Circuit", Location = "Baku", Country = "Azerbaijan", CountryCode = "AZ", LapCount = 51, LengthKm = 6.003m },
                new Circuit { Id = 19, Name = "Marina Bay Street Circuit", Location = "Singapore", Country = "Singapore", CountryCode = "SG", LapCount = 61, LengthKm = 4.94m },
                new Circuit { Id = 20, Name = "Circuit of the Americas", Location = "Austin", Country = "USA", CountryCode = "US", LapCount = 56, LengthKm = 5.513m },
                new Circuit { Id = 21, Name = "Autódromo Hermanos Rodríguez", Location = "Mexico City", Country = "Mexico", CountryCode = "MX", LapCount = 71, LengthKm = 4.304m },
                new Circuit { Id = 22, Name = "Interlagos Circuit", Location = "São Paulo", Country = "Brazil", CountryCode = "BR", LapCount = 71, LengthKm = 4.309m },
                new Circuit { Id = 23, Name = "Las Vegas Strip Circuit", Location = "Paradise", Country = "USA", CountryCode = "US", LapCount = 50, LengthKm = 6.201m },
                new Circuit { Id = 24, Name = "Lusail International Circuit", Location = "Lusail", Country = "Qatar", CountryCode = "QA", LapCount = 57, LengthKm = 5.38m },
                new Circuit { Id = 25, Name = "Yas Marina Circuit", Location = "Abu Dhabi", Country = "UAE", CountryCode = "AE", LapCount = 58, LengthKm = 5.281m }
            );

            // ========================
            //  UTRKE
            // ========================
            modelBuilder.Entity<Race>().HasData(
                new Race { Id = 1, SeasonId = 1, CircuitId = 3, Name = "Australian Grand Prix", RoundNumber = 1, RaceDate = new DateTime(2025, 3, 8, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 7, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Completed },
                new Race { Id = 2, SeasonId = 1, CircuitId = 5, Name = "Chinese Grand Prix", RoundNumber = 2, RaceDate = new DateTime(2025, 3, 15, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 14, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 3, 14, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Completed },
                new Race { Id = 3, SeasonId = 1, CircuitId = 4, Name = "Japanese Grand Prix", RoundNumber = 3, RaceDate = new DateTime(2025, 3, 29, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 3, 28, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Completed },
                new Race { Id = 4, SeasonId = 1, CircuitId = 6, Name = "Miami Grand Prix", RoundNumber = 4, RaceDate = new DateTime(2025, 5, 3, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 5, 2, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 5, 2, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
                new Race { Id = 5, SeasonId = 1, CircuitId = 10, Name = "Canadian Grand Prix", RoundNumber = 5, RaceDate = new DateTime(2025, 5, 24, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 5, 23, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 5, 23, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
                new Race { Id = 6, SeasonId = 1, CircuitId = 8, Name = "Monaco Grand Prix", RoundNumber = 6, RaceDate = new DateTime(2025, 6, 7, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 6, 6, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 7, SeasonId = 1, CircuitId = 9, Name = "Barcelona-Catalunya Grand Prix", RoundNumber = 7, RaceDate = new DateTime(2025, 6, 14, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 6, 13, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 8, SeasonId = 1, CircuitId = 11, Name = "Austrian Grand Prix", RoundNumber = 8, RaceDate = new DateTime(2025, 6, 28, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 6, 27, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 9, SeasonId = 1, CircuitId = 12, Name = "British Grand Prix", RoundNumber = 9, RaceDate = new DateTime(2025, 7, 5, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 7, 4, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 7, 4, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
                new Race { Id = 10, SeasonId = 1, CircuitId = 13, Name = "Belgian Grand Prix", RoundNumber = 10, RaceDate = new DateTime(2025, 7, 19, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 7, 18, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 11, SeasonId = 1, CircuitId = 14, Name = "Hungarian Grand Prix", RoundNumber = 11, RaceDate = new DateTime(2025, 7, 26, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 7, 25, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 12, SeasonId = 1, CircuitId = 15, Name = "Dutch Grand Prix", RoundNumber = 12, RaceDate = new DateTime(2025, 8, 23, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 8, 22, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 8, 22, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
                new Race { Id = 13, SeasonId = 1, CircuitId = 16, Name = "Italian Grand Prix", RoundNumber = 13, RaceDate = new DateTime(2025, 9, 6, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 9, 5, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 14, SeasonId = 1, CircuitId = 17, Name = "Spanish Grand Prix", RoundNumber = 14, RaceDate = new DateTime(2025, 9, 13, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 9, 12, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 15, SeasonId = 1, CircuitId = 18, Name = "Azerbaijan Grand Prix", RoundNumber = 15, RaceDate = new DateTime(2025, 9, 26, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 9, 25, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 16, SeasonId = 1, CircuitId = 19, Name = "Singapore Grand Prix", RoundNumber = 16, RaceDate = new DateTime(2025, 10, 11, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 10, 10, 15, 0, 0, DateTimeKind.Utc), SprintDate = new DateTime(2025, 10, 10, 15, 0, 0, DateTimeKind.Utc), HasSprint = true, Status = RaceStatus.Upcoming },
                new Race { Id = 17, SeasonId = 1, CircuitId = 20, Name = "United States Grand Prix", RoundNumber = 17, RaceDate = new DateTime(2025, 10, 25, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 10, 24, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 18, SeasonId = 1, CircuitId = 21, Name = "Mexico City Grand Prix", RoundNumber = 18, RaceDate = new DateTime(2025, 11, 1, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 10, 31, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 19, SeasonId = 1, CircuitId = 22, Name = "São Paulo Grand Prix", RoundNumber = 19, RaceDate = new DateTime(2025, 11, 8, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 11, 7, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 20, SeasonId = 1, CircuitId = 23, Name = "Las Vegas Grand Prix", RoundNumber = 20, RaceDate = new DateTime(2025, 11, 21, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 11, 20, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 21, SeasonId = 1, CircuitId = 24, Name = "Qatar Grand Prix", RoundNumber = 21, RaceDate = new DateTime(2025, 11, 29, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 11, 28, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming },
                new Race { Id = 22, SeasonId = 1, CircuitId = 25, Name = "Abu Dhabi Grand Prix", RoundNumber = 22, RaceDate = new DateTime(2025, 12, 6, 15, 0, 0, DateTimeKind.Utc), QualifyingDate = new DateTime(2025, 12, 5, 15, 0, 0, DateTimeKind.Utc), HasSprint = false, Status = RaceStatus.Upcoming }
            );

            // ========================
            //  REZULTATI UTRKA (Race 1, 2, 3)
            // ========================
            modelBuilder.Entity<RaceResult>().HasData(
                // Race 1 - Australian GP
                new RaceResult { Id = 1, RaceId = 1, DriverId = 5, Position = 1, DidNotFinish = false },
                new RaceResult { Id = 2, RaceId = 1, DriverId = 6, Position = 2, DidNotFinish = false },
                new RaceResult { Id = 3, RaceId = 1, DriverId = 3, Position = 3, DidNotFinish = false },
                new RaceResult { Id = 4, RaceId = 1, DriverId = 4, Position = 4, DidNotFinish = false },
                new RaceResult { Id = 5, RaceId = 1, DriverId = 7, Position = 5, DidNotFinish = false },
                new RaceResult { Id = 6, RaceId = 1, DriverId = 1, Position = 6, DidNotFinish = false },
                new RaceResult { Id = 7, RaceId = 1, DriverId = 20, Position = 7, DidNotFinish = false },
                new RaceResult { Id = 8, RaceId = 1, DriverId = 15, Position = 8, DidNotFinish = false },
                new RaceResult { Id = 9, RaceId = 1, DriverId = 18, Position = 9, DidNotFinish = false },
                new RaceResult { Id = 10, RaceId = 1, DriverId = 11, Position = 10, DidNotFinish = false },
                // Race 2 - Chinese GP
                new RaceResult { Id = 11, RaceId = 2, DriverId = 6, Position = 1, DidNotFinish = false },
                new RaceResult { Id = 12, RaceId = 2, DriverId = 5, Position = 2, DidNotFinish = false },
                new RaceResult { Id = 13, RaceId = 2, DriverId = 4, Position = 3, DidNotFinish = false },
                new RaceResult { Id = 14, RaceId = 2, DriverId = 3, Position = 4, DidNotFinish = false },
                new RaceResult { Id = 15, RaceId = 2, DriverId = 20, Position = 5, DidNotFinish = false },
                new RaceResult { Id = 16, RaceId = 2, DriverId = 11, Position = 6, DidNotFinish = false },
                new RaceResult { Id = 17, RaceId = 2, DriverId = 16, Position = 7, DidNotFinish = false },
                new RaceResult { Id = 18, RaceId = 2, DriverId = 22, Position = 8, DidNotFinish = false },
                new RaceResult { Id = 19, RaceId = 2, DriverId = 14, Position = 9, DidNotFinish = false },
                new RaceResult { Id = 20, RaceId = 2, DriverId = 12, Position = 10, DidNotFinish = false },
                // Race 3 - Japanese GP
                new RaceResult { Id = 21, RaceId = 3, DriverId = 6, Position = 1, DidNotFinish = false },
                new RaceResult { Id = 22, RaceId = 3, DriverId = 8, Position = 2, DidNotFinish = false },
                new RaceResult { Id = 23, RaceId = 3, DriverId = 3, Position = 3, DidNotFinish = false },
                new RaceResult { Id = 24, RaceId = 3, DriverId = 5, Position = 4, DidNotFinish = false },
                new RaceResult { Id = 25, RaceId = 3, DriverId = 7, Position = 5, DidNotFinish = false },
                new RaceResult { Id = 26, RaceId = 3, DriverId = 4, Position = 6, DidNotFinish = false },
                new RaceResult { Id = 27, RaceId = 3, DriverId = 11, Position = 7, DidNotFinish = false },
                new RaceResult { Id = 28, RaceId = 3, DriverId = 1, Position = 8, DidNotFinish = false },
                new RaceResult { Id = 29, RaceId = 3, DriverId = 16, Position = 9, DidNotFinish = false },
                new RaceResult { Id = 30, RaceId = 3, DriverId = 19, Position = 10, DidNotFinish = false }
            );

            // ========================
            //  REZULTATI KVALIFIKACIJA
            // ========================
            modelBuilder.Entity<QualifyingResult>().HasData(
                new QualifyingResult { Id = 1, RaceId = 1, DriverId = 5, Position = 1 },
                new QualifyingResult { Id = 2, RaceId = 1, DriverId = 6, Position = 2 },
                new QualifyingResult { Id = 3, RaceId = 1, DriverId = 22, Position = 3 },
                new QualifyingResult { Id = 4, RaceId = 2, DriverId = 6, Position = 1 },
                new QualifyingResult { Id = 5, RaceId = 2, DriverId = 5, Position = 2 },
                new QualifyingResult { Id = 6, RaceId = 2, DriverId = 4, Position = 3 },
                new QualifyingResult { Id = 7, RaceId = 3, DriverId = 6, Position = 1 },
                new QualifyingResult { Id = 8, RaceId = 3, DriverId = 5, Position = 2 },
                new QualifyingResult { Id = 9, RaceId = 3, DriverId = 8, Position = 3 }
            );

            // ========================
            //  SPRINT REZULTATI (Race 2)
            // ========================
            modelBuilder.Entity<SprintResult>().HasData(
                new SprintResult { Id = 1, RaceId = 2, DriverId = 5, Position = 1, DidNotFinish = false },
                new SprintResult { Id = 2, RaceId = 2, DriverId = 3, Position = 2, DidNotFinish = false },
                new SprintResult { Id = 3, RaceId = 2, DriverId = 4, Position = 3, DidNotFinish = false }
            );

            // ========================
            //  RACE EXTRAS (bez DnfDriverId i ScoredPointsDriverId)
            // ========================
            modelBuilder.Entity<RaceExtra>().HasData(
                new RaceExtra { Id = 1, RaceId = 1, FastestLapDriverId = 1, FastestPitStopDriverId = 5, DriverOfTheDayId = 1, SafetyCarCount = 0 },
                new RaceExtra { Id = 2, RaceId = 2, FastestLapDriverId = 6, FastestPitStopDriverId = 4, DriverOfTheDayId = 6, SafetyCarCount = 1 },
                new RaceExtra { Id = 3, RaceId = 3, FastestLapDriverId = 6, FastestPitStopDriverId = 4, DriverOfTheDayId = 8, SafetyCarCount = 2 }
            );

            // ========================
            //  DNF ENTRIES
            // ========================
            modelBuilder.Entity<RaceDnfEntry>().HasData(
                // Race 1 - Australian GP
                new RaceDnfEntry { Id = 1, RaceId = 1, DriverId = 10 },
                new RaceDnfEntry { Id = 2, RaceId = 1, DriverId = 9 },
                new RaceDnfEntry { Id = 3, RaceId = 1, DriverId = 21 },
                new RaceDnfEntry { Id = 4, RaceId = 1, DriverId = 22 },
                new RaceDnfEntry { Id = 5, RaceId = 1, DriverId = 8 },
                new RaceDnfEntry { Id = 6, RaceId = 1, DriverId = 17 },
                // Race 2 - Chinese GP
                new RaceDnfEntry { Id = 7, RaceId = 2, DriverId = 13 },
                new RaceDnfEntry { Id = 8, RaceId = 2, DriverId = 18 },
                new RaceDnfEntry { Id = 9, RaceId = 2, DriverId = 7 },
                new RaceDnfEntry { Id = 10, RaceId = 2, DriverId = 8 },
                new RaceDnfEntry { Id = 11, RaceId = 2, DriverId = 10 },
                new RaceDnfEntry { Id = 12, RaceId = 2, DriverId = 9 },
                new RaceDnfEntry { Id = 13, RaceId = 2, DriverId = 1 },
                // Race 3 - Japanese GP
                new RaceDnfEntry { Id = 14, RaceId = 3, DriverId = 20 },
                new RaceDnfEntry { Id = 15, RaceId = 3, DriverId = 10 }
            );

            // ========================
            //  PROGNOZE KORISNIKA
            // ========================
            modelBuilder.Entity<Prediction>().HasData(
                new Prediction { Id = 1, UserId = 4, RaceId = 1, SubmittedAt = new DateTime(2026, 4, 16, 10, 59, 59, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 10, PredictedDnfDriverId = 10, PredictedFastestLapDriverId = 1, PredictedScoredPointsDriverId = 11, PredictedFastestPitStopDriverId = 3, PredictedDriverOfTheDayId = 4, PredictedSafetyCarCount = 2 },
                new Prediction { Id = 2, UserId = 4, RaceId = 2, SubmittedAt = new DateTime(2026, 4, 16, 8, 7, 3, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 12, PredictedDnfDriverId = 10, PredictedFastestLapDriverId = 6, PredictedScoredPointsDriverId = 15, PredictedFastestPitStopDriverId = 3, PredictedDriverOfTheDayId = 4, PredictedSafetyCarCount = 0 },
                new Prediction { Id = 3, UserId = 4, RaceId = 3, SubmittedAt = new DateTime(2026, 4, 16, 8, 9, 22, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 16, PredictedDnfDriverId = 10, PredictedFastestLapDriverId = 6, PredictedScoredPointsDriverId = 11, PredictedFastestPitStopDriverId = 3, PredictedDriverOfTheDayId = 4, PredictedSafetyCarCount = 0 },
                new Prediction { Id = 4, UserId = 3, RaceId = 1, SubmittedAt = new DateTime(2026, 4, 16, 8, 15, 8, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 2, PredictedDnfDriverId = 10, PredictedFastestLapDriverId = 3, PredictedScoredPointsDriverId = 13, PredictedFastestPitStopDriverId = 7, PredictedDriverOfTheDayId = 3, PredictedSafetyCarCount = 2 },
                new Prediction { Id = 5, UserId = 3, RaceId = 2, SubmittedAt = new DateTime(2026, 4, 16, 8, 16, 39, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 8, PredictedDnfDriverId = 9, PredictedFastestLapDriverId = 5, PredictedScoredPointsDriverId = 17, PredictedFastestPitStopDriverId = 3, PredictedDriverOfTheDayId = 6, PredictedSafetyCarCount = 1 },
                new Prediction { Id = 6, UserId = 3, RaceId = 3, SubmittedAt = new DateTime(2026, 4, 16, 8, 32, 14, DateTimeKind.Utc), IsLocked = false, TotalPointsForRace = 8, PredictedDnfDriverId = 16, PredictedFastestLapDriverId = 5, PredictedScoredPointsDriverId = 17, PredictedFastestPitStopDriverId = 3, PredictedDriverOfTheDayId = 7, PredictedSafetyCarCount = 2 }
            );

            // ========================
            //  POZICIJE PROGNOZA - UTRKA
            // ========================
            modelBuilder.Entity<PredictionRacePosition>().HasData(
                // Prediction 1 (petar, Race 1)
                new PredictionRacePosition { Id = 1, PredictionId = 1, Position = 1, DriverId = 4 },
                new PredictionRacePosition { Id = 2, PredictionId = 1, Position = 2, DriverId = 5 },
                new PredictionRacePosition { Id = 3, PredictionId = 1, Position = 3, DriverId = 3 },
                new PredictionRacePosition { Id = 4, PredictionId = 1, Position = 4, DriverId = 1 },
                new PredictionRacePosition { Id = 5, PredictionId = 1, Position = 5, DriverId = 7 },
                new PredictionRacePosition { Id = 6, PredictionId = 1, Position = 6, DriverId = 8 },
                new PredictionRacePosition { Id = 7, PredictionId = 1, Position = 7, DriverId = 6 },
                new PredictionRacePosition { Id = 8, PredictionId = 1, Position = 8, DriverId = 22 },
                new PredictionRacePosition { Id = 9, PredictionId = 1, Position = 9, DriverId = 11 },
                new PredictionRacePosition { Id = 10, PredictionId = 1, Position = 10, DriverId = 14 },
                // Prediction 2 (petar, Race 2)
                new PredictionRacePosition { Id = 11, PredictionId = 2, Position = 1, DriverId = 5 },
                new PredictionRacePosition { Id = 12, PredictionId = 2, Position = 2, DriverId = 4 },
                new PredictionRacePosition { Id = 13, PredictionId = 2, Position = 3, DriverId = 6 },
                new PredictionRacePosition { Id = 14, PredictionId = 2, Position = 4, DriverId = 3 },
                new PredictionRacePosition { Id = 15, PredictionId = 2, Position = 5, DriverId = 7 },
                new PredictionRacePosition { Id = 16, PredictionId = 2, Position = 6, DriverId = 8 },
                new PredictionRacePosition { Id = 17, PredictionId = 2, Position = 7, DriverId = 1 },
                new PredictionRacePosition { Id = 18, PredictionId = 2, Position = 8, DriverId = 22 },
                new PredictionRacePosition { Id = 19, PredictionId = 2, Position = 9, DriverId = 14 },
                new PredictionRacePosition { Id = 20, PredictionId = 2, Position = 10, DriverId = 15 },
                // Prediction 3 (petar, Race 3)
                new PredictionRacePosition { Id = 21, PredictionId = 3, Position = 1, DriverId = 5 },
                new PredictionRacePosition { Id = 22, PredictionId = 3, Position = 2, DriverId = 6 },
                new PredictionRacePosition { Id = 23, PredictionId = 3, Position = 3, DriverId = 3 },
                new PredictionRacePosition { Id = 24, PredictionId = 3, Position = 4, DriverId = 4 },
                new PredictionRacePosition { Id = 25, PredictionId = 3, Position = 5, DriverId = 7 },
                new PredictionRacePosition { Id = 26, PredictionId = 3, Position = 6, DriverId = 8 },
                new PredictionRacePosition { Id = 27, PredictionId = 3, Position = 7, DriverId = 11 },
                new PredictionRacePosition { Id = 28, PredictionId = 3, Position = 8, DriverId = 1 },
                new PredictionRacePosition { Id = 29, PredictionId = 3, Position = 9, DriverId = 22 },
                new PredictionRacePosition { Id = 30, PredictionId = 3, Position = 10, DriverId = 19 },
                // Prediction 4 (ivan, Race 1)
                new PredictionRacePosition { Id = 31, PredictionId = 4, Position = 1, DriverId = 3 },
                new PredictionRacePosition { Id = 32, PredictionId = 4, Position = 2, DriverId = 4 },
                new PredictionRacePosition { Id = 33, PredictionId = 4, Position = 3, DriverId = 7 },
                new PredictionRacePosition { Id = 34, PredictionId = 4, Position = 4, DriverId = 5 },
                new PredictionRacePosition { Id = 35, PredictionId = 4, Position = 5, DriverId = 1 },
                new PredictionRacePosition { Id = 36, PredictionId = 4, Position = 6, DriverId = 6 },
                new PredictionRacePosition { Id = 37, PredictionId = 4, Position = 7, DriverId = 8 },
                new PredictionRacePosition { Id = 38, PredictionId = 4, Position = 8, DriverId = 14 },
                new PredictionRacePosition { Id = 39, PredictionId = 4, Position = 9, DriverId = 11 },
                new PredictionRacePosition { Id = 40, PredictionId = 4, Position = 10, DriverId = 9 },
                // Prediction 5 (ivan, Race 2)
                new PredictionRacePosition { Id = 41, PredictionId = 5, Position = 1, DriverId = 5 },
                new PredictionRacePosition { Id = 42, PredictionId = 5, Position = 2, DriverId = 6 },
                new PredictionRacePosition { Id = 43, PredictionId = 5, Position = 3, DriverId = 3 },
                new PredictionRacePosition { Id = 44, PredictionId = 5, Position = 4, DriverId = 7 },
                new PredictionRacePosition { Id = 45, PredictionId = 5, Position = 5, DriverId = 4 },
                new PredictionRacePosition { Id = 46, PredictionId = 5, Position = 6, DriverId = 8 },
                new PredictionRacePosition { Id = 47, PredictionId = 5, Position = 7, DriverId = 1 },
                new PredictionRacePosition { Id = 48, PredictionId = 5, Position = 8, DriverId = 11 },
                new PredictionRacePosition { Id = 49, PredictionId = 5, Position = 9, DriverId = 17 },
                new PredictionRacePosition { Id = 50, PredictionId = 5, Position = 10, DriverId = 14 },
                // Prediction 6 (ivan, Race 3)
                new PredictionRacePosition { Id = 51, PredictionId = 6, Position = 1, DriverId = 6 },
                new PredictionRacePosition { Id = 52, PredictionId = 6, Position = 2, DriverId = 5 },
                new PredictionRacePosition { Id = 53, PredictionId = 6, Position = 3, DriverId = 3 },
                new PredictionRacePosition { Id = 54, PredictionId = 6, Position = 4, DriverId = 7 },
                new PredictionRacePosition { Id = 55, PredictionId = 6, Position = 5, DriverId = 4 },
                new PredictionRacePosition { Id = 56, PredictionId = 6, Position = 6, DriverId = 8 },
                new PredictionRacePosition { Id = 57, PredictionId = 6, Position = 7, DriverId = 14 },
                new PredictionRacePosition { Id = 58, PredictionId = 6, Position = 8, DriverId = 1 },
                new PredictionRacePosition { Id = 59, PredictionId = 6, Position = 9, DriverId = 11 },
                new PredictionRacePosition { Id = 60, PredictionId = 6, Position = 10, DriverId = 17 }
            );

            // ========================
            //  POZICIJE PROGNOZA - KVALIFIKACIJE
            // ========================
            modelBuilder.Entity<PredictionQualifyingPosition>().HasData(
                new PredictionQualifyingPosition { Id = 1, PredictionId = 1, Position = 1, DriverId = 1 },
                new PredictionQualifyingPosition { Id = 2, PredictionId = 1, Position = 2, DriverId = 3 },
                new PredictionQualifyingPosition { Id = 3, PredictionId = 1, Position = 3, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 4, PredictionId = 2, Position = 1, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 5, PredictionId = 2, Position = 2, DriverId = 6 },
                new PredictionQualifyingPosition { Id = 6, PredictionId = 2, Position = 3, DriverId = 3 },
                new PredictionQualifyingPosition { Id = 7, PredictionId = 3, Position = 1, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 8, PredictionId = 3, Position = 2, DriverId = 6 },
                new PredictionQualifyingPosition { Id = 9, PredictionId = 3, Position = 3, DriverId = 3 },
                new PredictionQualifyingPosition { Id = 10, PredictionId = 4, Position = 1, DriverId = 3 },
                new PredictionQualifyingPosition { Id = 11, PredictionId = 4, Position = 2, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 12, PredictionId = 4, Position = 3, DriverId = 1 },
                new PredictionQualifyingPosition { Id = 13, PredictionId = 5, Position = 1, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 14, PredictionId = 5, Position = 2, DriverId = 3 },
                new PredictionQualifyingPosition { Id = 15, PredictionId = 5, Position = 3, DriverId = 6 },
                new PredictionQualifyingPosition { Id = 16, PredictionId = 6, Position = 1, DriverId = 5 },
                new PredictionQualifyingPosition { Id = 17, PredictionId = 6, Position = 2, DriverId = 6 },
                new PredictionQualifyingPosition { Id = 18, PredictionId = 6, Position = 3, DriverId = 3 }
            );

            // ========================
            //  POZICIJE PROGNOZA - SPRINT (Race 2)
            // ========================
            modelBuilder.Entity<PredictionSprintPosition>().HasData(
                new PredictionSprintPosition { Id = 1, PredictionId = 2, Position = 1, DriverId = 5 },
                new PredictionSprintPosition { Id = 2, PredictionId = 2, Position = 2, DriverId = 6 },
                new PredictionSprintPosition { Id = 3, PredictionId = 2, Position = 3, DriverId = 3 },
                new PredictionSprintPosition { Id = 4, PredictionId = 5, Position = 1, DriverId = 5 },
                new PredictionSprintPosition { Id = 5, PredictionId = 5, Position = 2, DriverId = 6 },
                new PredictionSprintPosition { Id = 6, PredictionId = 5, Position = 3, DriverId = 7 }
            );

            // ========================
            //  LEADERBOARD
            // ========================
            modelBuilder.Entity<PredictionLeaderboard>().HasData(
                new PredictionLeaderboard { Id = 1, UserId = 4, SeasonId = 1, TotalPoints = 38, RacesParticipated = 3, Position = 1, LastUpdated = new DateTime(2026, 4, 16, 11, 36, 47, DateTimeKind.Utc) },
                new PredictionLeaderboard { Id = 2, UserId = 3, SeasonId = 1, TotalPoints = 18, RacesParticipated = 3, Position = 2, LastUpdated = new DateTime(2026, 4, 16, 11, 36, 47, DateTimeKind.Utc) }
            );
        }
}
