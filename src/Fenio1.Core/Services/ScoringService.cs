namespace Fenio1.Core.Services;

using Fenio1.Core.Entities;
using Fenio1.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

public interface IAppDbContext
{
    DbSet<Prediction> Predictions { get; }
    DbSet<PredictionRacePosition> PredictionRacePositions { get; }
    DbSet<PredictionQualifyingPosition> PredictionQualifyingPositions { get; }
    DbSet<PredictionSprintPosition> PredictionSprintPositions { get; }
    DbSet<RaceResult> RaceResults { get; }
    DbSet<QualifyingResult> QualifyingResults { get; }
    DbSet<SprintResult> SprintResults { get; }
    DbSet<RaceExtra> RaceExtras { get; }
    DbSet<PredictionLeaderboard> PredictionLeaderboards { get; }
    DbSet<Race> Races { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ScoringService : IScoringService
{
    private const int PointsPerCorrectPrediction = 2;
    private readonly IAppDbContext _db;

    public ScoringService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> CalculatePredictionPointsAsync(int predictionId)
    {
        var prediction = await _db.Predictions
            .Include(p => p.RacePositions)
            .Include(p => p.QualifyingPositions)
            .Include(p => p.SprintPositions)
            .FirstOrDefaultAsync(p => p.Id == predictionId);

        if (prediction == null) return 0;

        var raceResults = await _db.RaceResults
            .Where(r => r.RaceId == prediction.RaceId)
            .ToListAsync();

        var qualifyingResults = await _db.QualifyingResults
            .Where(r => r.RaceId == prediction.RaceId)
            .ToListAsync();

        var sprintResults = await _db.SprintResults
            .Where(r => r.RaceId == prediction.RaceId)
            .ToListAsync();

        var raceExtra = await _db.RaceExtras
            .FirstOrDefaultAsync(r => r.RaceId == prediction.RaceId);

        var race = await _db.Races.FindAsync(prediction.RaceId);

        int points = 0;

        // --- Provjera poretka u utrci (top 10, svaka pogođena pozicija = 2 boda) ---
        foreach (var predictedPos in prediction.RacePositions)
        {
            var actualResult = raceResults.FirstOrDefault(r =>
                r.Position == predictedPos.Position && r.DriverId == predictedPos.DriverId);
            if (actualResult != null)
                points += PointsPerCorrectPrediction;
        }

        // --- Provjera poretka u kvalifikacijama (top 3) ---
        foreach (var predictedPos in prediction.QualifyingPositions)
        {
            var actualResult = qualifyingResults.FirstOrDefault(r =>
                r.Position == predictedPos.Position && r.DriverId == predictedPos.DriverId);
            if (actualResult != null)
                points += PointsPerCorrectPrediction;
        }

        // --- Provjera poretka u sprint utrci (top 3, ako postoji) ---
        if (race?.HasSprint == true)
        {
            foreach (var predictedPos in prediction.SprintPositions)
            {
                var actualResult = sprintResults.FirstOrDefault(r =>
                    r.Position == predictedPos.Position && r.DriverId == predictedPos.DriverId);
                if (actualResult != null)
                    points += PointsPerCorrectPrediction;
            }
        }

        // --- Provjera posebnih prognoza ---
        if (raceExtra != null)
        {
            if (prediction.PredictedDnfDriverId.HasValue &&
                prediction.PredictedDnfDriverId == raceExtra.DnfDriverId)
                points += PointsPerCorrectPrediction;

            if (prediction.PredictedFastestLapDriverId.HasValue &&
                prediction.PredictedFastestLapDriverId == raceExtra.FastestLapDriverId)
                points += PointsPerCorrectPrediction;

            if (prediction.PredictedScoredPointsDriverId.HasValue &&
                prediction.PredictedScoredPointsDriverId == raceExtra.ScoredPointsDriverId)
                points += PointsPerCorrectPrediction;

            if (prediction.PredictedFastestPitStopDriverId.HasValue &&
                prediction.PredictedFastestPitStopDriverId == raceExtra.FastestPitStopDriverId)
                points += PointsPerCorrectPrediction;

            if (prediction.PredictedDriverOfTheDayId.HasValue &&
                prediction.PredictedDriverOfTheDayId == raceExtra.DriverOfTheDayId)
                points += PointsPerCorrectPrediction;

            if (prediction.PredictedSafetyCarCount.HasValue &&
                prediction.PredictedSafetyCarCount == raceExtra.SafetyCarCount)
                points += PointsPerCorrectPrediction;
        }

        // Spremi bodove
        prediction.TotalPointsForRace = points;
        await _db.SaveChangesAsync();

        return points;
    }

    public async Task<int> GetTotalPointsForUserInSeasonAsync(int userId, int seasonId)
    {
        var predictions = await _db.Predictions
            .Include(p => p.Race)
            .Where(p => p.UserId == userId && p.Race.SeasonId == seasonId && p.TotalPointsForRace.HasValue)
            .ToListAsync();

        return predictions.Sum(p => p.TotalPointsForRace ?? 0);
    }

    public async Task RecalculateRacePointsAsync(int raceId)
    {
        var predictions = await _db.Predictions
            .Where(p => p.RaceId == raceId)
            .ToListAsync();

        foreach (var prediction in predictions)
        {
            await CalculatePredictionPointsAsync(prediction.Id);
        }
    }

    public async Task UpdateLeaderboardAsync(int seasonId)
    {
        var allPredictions = await _db.Predictions
            .Include(p => p.Race)
            .Where(p => p.Race.SeasonId == seasonId && p.TotalPointsForRace.HasValue)
            .GroupBy(p => p.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPoints = g.Sum(p => p.TotalPointsForRace ?? 0),
                RacesParticipated = g.Count()
            })
            .OrderByDescending(x => x.TotalPoints)
            .ToListAsync();

        int position = 1;
        foreach (var entry in allPredictions)
        {
            var leaderboardEntry = await _db.PredictionLeaderboards
                .FirstOrDefaultAsync(l => l.UserId == entry.UserId && l.SeasonId == seasonId);

            if (leaderboardEntry == null)
            {
                _db.PredictionLeaderboards.Add(new PredictionLeaderboard
                {
                    UserId = entry.UserId,
                    SeasonId = seasonId,
                    TotalPoints = entry.TotalPoints,
                    RacesParticipated = entry.RacesParticipated,
                    Position = position++,
                    LastUpdated = DateTime.UtcNow
                });
            }
            else
            {
                leaderboardEntry.TotalPoints = entry.TotalPoints;
                leaderboardEntry.RacesParticipated = entry.RacesParticipated;
                leaderboardEntry.Position = position++;
                leaderboardEntry.LastUpdated = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
    }
}
