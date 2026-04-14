namespace Fenio1.API.Controllers;

using Fenio1.API.DTOs;
using Fenio1.Core.Interfaces;
using Fenio1.Core.Services;
using Fenio1.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LeaderboardController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IScoringService _scoring;

    public LeaderboardController(AppDbContext db, IScoringService scoring)
    {
        _db = db;
        _scoring = scoring;
    }

    /// <summary>
    /// Ukupni leaderboard za sezonu (sortirano po bodovima).
    /// </summary>
    [HttpGet("season/{seasonId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LeaderboardEntryDto>), 200)]
    public async Task<IActionResult> GetSeasonLeaderboard(int seasonId)
    {
        var entries = await _db.PredictionLeaderboards
            .Include(l => l.User)
            .Where(l => l.SeasonId == seasonId)
            .OrderBy(l => l.Position)
            .Select(l => new LeaderboardEntryDto(
                l.Position, l.UserId, l.User.Username,
                l.TotalPoints, l.RacesParticipated))
            .ToListAsync();

        return Ok(entries);
    }

    /// <summary>
    /// Leaderboard za aktivnu sezonu.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<LeaderboardEntryDto>), 200)]
    public async Task<IActionResult> GetActiveSeasonLeaderboard()
    {
        var activeSeason = await _db.Seasons.FirstOrDefaultAsync(s => s.IsActive);
        if (activeSeason == null) return NotFound(new { message = "Nema aktivne sezone." });

        return await GetSeasonLeaderboard(activeSeason.Id);
    }

    /// <summary>
    /// Rezultati prognoza po utrci - bodovi svakog korisnika za tu utrku + kumulativ.
    /// </summary>
    [HttpGet("race/{raceId:int}")]
    [ProducesResponseType(typeof(RaceLeaderboardDto), 200)]
    public async Task<IActionResult> GetRaceLeaderboard(int raceId)
    {
        var race = await _db.Races.Include(r => r.Season).FirstOrDefaultAsync(r => r.Id == raceId);
        if (race == null) return NotFound();

        var predictions = await _db.Predictions
            .Include(p => p.User)
            .Where(p => p.RaceId == raceId && p.TotalPointsForRace.HasValue)
            .OrderByDescending(p => p.TotalPointsForRace)
            .ToListAsync();

        // Izračunaj kumulativne bodove za svaki par (korisnik, utrka)
        var scores = new List<RaceScoreEntryDto>();
        int position = 1;

        foreach (var prediction in predictions)
        {
            var cumulative = await _scoring.GetTotalPointsForUserInSeasonAsync(prediction.UserId, race.SeasonId);
            scores.Add(new RaceScoreEntryDto(
                position++,
                prediction.User.Username,
                prediction.TotalPointsForRace ?? 0,
                cumulative
            ));
        }

        return Ok(new RaceLeaderboardDto(raceId, race.Name, scores));
    }

    /// <summary>
    /// Admin - ručno pokretanje recalculation bodova za utrku.
    /// </summary>
    [HttpPost("race/{raceId:int}/recalculate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> RecalculateRace(int raceId)
    {
        var race = await _db.Races.FindAsync(raceId);
        if (race == null) return NotFound();

        await _scoring.RecalculateRacePointsAsync(raceId);
        await _scoring.UpdateLeaderboardAsync(race.SeasonId);

        return Ok(new { message = $"Bodovi za utrku #{raceId} su ponovo izračunati." });
    }
}
