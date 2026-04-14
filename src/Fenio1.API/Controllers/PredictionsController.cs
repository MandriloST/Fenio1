namespace Fenio1.API.Controllers;

using System.Security.Claims;
using Fenio1.API.DTOs;
using Fenio1.Core.Entities;
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
public class PredictionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IScoringService _scoring;

    public PredictionsController(AppDbContext db, IScoringService scoring)
    {
        _db = db;
        _scoring = scoring;
    }

    // ==================
    //  READ
    // ==================

    /// <summary>
    /// Dohvaća sve prognoze trenutnog korisnika, opcionalno filtrirane po sezoni.
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<PredictionDto>), 200)]
    public async Task<IActionResult> GetMine([FromQuery] int? seasonId, [FromQuery] int? raceId)
    {
        var userId = GetCurrentUserId();
        var query = _db.Predictions
            .Include(p => p.Race).ThenInclude(r => r.Season)
            .Include(p => p.RacePositions).ThenInclude(rp => rp.Driver)
            .Include(p => p.QualifyingPositions).ThenInclude(qp => qp.Driver)
            .Include(p => p.SprintPositions).ThenInclude(sp => sp.Driver)
            .Include(p => p.PredictedDnfDriver)
            .Include(p => p.PredictedFastestLapDriver)
            .Include(p => p.PredictedScoredPointsDriver)
            .Include(p => p.PredictedFastestPitStopDriver)
            .Include(p => p.PredictedDriverOfTheDay)
            .Where(p => p.UserId == userId)
            .AsQueryable();

        if (seasonId.HasValue) query = query.Where(p => p.Race.SeasonId == seasonId);
        if (raceId.HasValue) query = query.Where(p => p.RaceId == raceId);

        var predictions = await query.OrderByDescending(p => p.Race.RoundNumber).ToListAsync();

        // Za provjeru točnosti trebamo i stvarne rezultate
        var raceIds = predictions.Select(p => p.RaceId).Distinct().ToList();
        var raceResults = await _db.RaceResults.Where(r => raceIds.Contains(r.RaceId)).ToListAsync();
        var qualResults = await _db.QualifyingResults.Where(r => raceIds.Contains(r.RaceId)).ToListAsync();
        var sprintResults = await _db.SprintResults.Where(r => raceIds.Contains(r.RaceId)).ToListAsync();

        return Ok(predictions.Select(p => MapToPredictionDto(p, raceResults, qualResults, sprintResults)));
    }

    /// <summary>
    /// Dohvaća prognozu za određenu utrku za trenutnog korisnika.
    /// </summary>
    [HttpGet("my/race/{raceId:int}")]
    [ProducesResponseType(typeof(PredictionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMyPredictionForRace(int raceId)
    {
        var userId = GetCurrentUserId();
        var prediction = await LoadFullPrediction(userId, raceId);

        if (prediction == null) return NotFound(new { message = "Nemate prognozu za ovu utrku." });

        var raceResults = await _db.RaceResults.Where(r => r.RaceId == raceId).ToListAsync();
        var qualResults = await _db.QualifyingResults.Where(r => r.RaceId == raceId).ToListAsync();
        var sprintResults = await _db.SprintResults.Where(r => r.RaceId == raceId).ToListAsync();

        return Ok(MapToPredictionDto(prediction, raceResults, qualResults, sprintResults));
    }

    /// <summary>
    /// Admin - dohvaća sve prognoze za određenu utrku.
    /// </summary>
    [HttpGet("race/{raceId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<PredictionDto>), 200)]
    public async Task<IActionResult> GetAllForRace(int raceId)
    {
        var predictions = await _db.Predictions
            .Include(p => p.User)
            .Include(p => p.Race)
            .Include(p => p.RacePositions).ThenInclude(rp => rp.Driver)
            .Include(p => p.QualifyingPositions).ThenInclude(qp => qp.Driver)
            .Include(p => p.SprintPositions).ThenInclude(sp => sp.Driver)
            .Include(p => p.PredictedDnfDriver)
            .Include(p => p.PredictedFastestLapDriver)
            .Include(p => p.PredictedScoredPointsDriver)
            .Include(p => p.PredictedFastestPitStopDriver)
            .Include(p => p.PredictedDriverOfTheDay)
            .Where(p => p.RaceId == raceId)
            .ToListAsync();

        var raceResults = await _db.RaceResults.Where(r => r.RaceId == raceId).ToListAsync();
        var qualResults = await _db.QualifyingResults.Where(r => r.RaceId == raceId).ToListAsync();
        var sprintResults = await _db.SprintResults.Where(r => r.RaceId == raceId).ToListAsync();

        return Ok(predictions.Select(p => MapToPredictionDto(p, raceResults, qualResults, sprintResults)));
    }

    // ==================
    //  CREATE / UPDATE
    // ==================

    /// <summary>
    /// Unosi ili ažurira prognozu za određenu utrku.
    /// Ne može se mijenjati ako je prognoza zaključana (admin zaključa nakon utrke).
    /// </summary>
    [HttpPost("race/{raceId:int}")]
    [ProducesResponseType(typeof(PredictionDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> SubmitOrUpdate(int raceId, [FromBody] SubmitPredictionRequest request)
    {
        var userId = GetCurrentUserId();

        var race = await _db.Races.FindAsync(raceId);
        if (race == null) return NotFound(new { message = "Utrka ne postoji." });

        if (race.Status == RaceStatus.Completed)
            return BadRequest(new { message = "Utrka je već završena. Prognoza se ne može mijenjati." });

        // Validacija
        if (request.RacePositions.Count != 10)
            return BadRequest(new { message = "Potrebno je unijeti točno 10 vozača za utrku." });

        if (request.QualifyingPositions.Count != 3)
            return BadRequest(new { message = "Potrebno je unijeti točno 3 vozača za kvalifikacije." });

        if (race.HasSprint && (request.SprintPositions == null || request.SprintPositions.Count != 3))
            return BadRequest(new { message = "Ova utrka ima sprint - unesite 3 sprint pozicije." });

        // Provjeri duplikate vozača unutar kategorije
        if (request.RacePositions.Select(p => p.DriverId).Distinct().Count() != request.RacePositions.Count)
            return BadRequest(new { message = "Isti vozač ne može biti na više pozicija u utrci." });

        var existing = await LoadFullPrediction(userId, raceId);

        if (existing != null)
        {
            if (existing.IsLocked)
                return BadRequest(new { message = "Prognoza je zaključana i ne može se mijenjati." });

            // Obrisi stare i zamijeni novim
            _db.PredictionRacePositions.RemoveRange(existing.RacePositions);
            _db.PredictionQualifyingPositions.RemoveRange(existing.QualifyingPositions);
            _db.PredictionSprintPositions.RemoveRange(existing.SprintPositions);

            existing.UpdatedAt = DateTime.UtcNow;
            existing.PredictedDnfDriverId = request.PredictedDnfDriverId;
            existing.PredictedFastestLapDriverId = request.PredictedFastestLapDriverId;
            existing.PredictedScoredPointsDriverId = request.PredictedScoredPointsDriverId;
            existing.PredictedFastestPitStopDriverId = request.PredictedFastestPitStopDriverId;
            existing.PredictedDriverOfTheDayId = request.PredictedDriverOfTheDayId;
            existing.PredictedSafetyCarCount = request.PredictedSafetyCarCount;
            existing.TotalPointsForRace = null; // reset

            AddPredictionPositions(existing.Id, request, raceId);
            await _db.SaveChangesAsync();

            var updatedPrediction = await LoadFullPrediction(userId, raceId);
            return Ok(MapToPredictionDto(updatedPrediction!, [], [], []));
        }

        // Nova prognoza
        var prediction = new Prediction
        {
            UserId = userId,
            RaceId = raceId,
            SubmittedAt = DateTime.UtcNow,
            PredictedDnfDriverId = request.PredictedDnfDriverId,
            PredictedFastestLapDriverId = request.PredictedFastestLapDriverId,
            PredictedScoredPointsDriverId = request.PredictedScoredPointsDriverId,
            PredictedFastestPitStopDriverId = request.PredictedFastestPitStopDriverId,
            PredictedDriverOfTheDayId = request.PredictedDriverOfTheDayId,
            PredictedSafetyCarCount = request.PredictedSafetyCarCount
        };

        _db.Predictions.Add(prediction);
        await _db.SaveChangesAsync();

        AddPredictionPositions(prediction.Id, request, raceId);
        await _db.SaveChangesAsync();

        var savedPrediction = await LoadFullPrediction(userId, raceId);
        return Ok(MapToPredictionDto(savedPrediction!, [], [], []));
    }

    /// <summary>
    /// Admin zaključava ili otključava prognozu.
    /// </summary>
    [HttpPatch("{predictionId:int}/lock")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SetLock(int predictionId, [FromQuery] bool locked)
    {
        var prediction = await _db.Predictions.FindAsync(predictionId);
        if (prediction == null) return NotFound();

        prediction.IsLocked = locked;
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Prognoza {(locked ? "zaključana" : "otključana")}.", predictionId });
    }

    // ==================
    //  SCORING
    // ==================

    /// <summary>
    /// Bodovi korisnika za jednu utrku (bodovi za tu utrku + kumulativni za sezonu).
    /// </summary>
    [HttpGet("my/score/race/{raceId:int}")]
    [ProducesResponseType(typeof(UserRaceScoreDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMyScoreForRace(int raceId)
    {
        var userId = GetCurrentUserId();
        var prediction = await _db.Predictions
            .Include(p => p.Race)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.RaceId == raceId);

        if (prediction == null) return NotFound(new { message = "Prognoza za ovu utrku nije pronađena." });

        var cumulative = await _scoring.GetTotalPointsForUserInSeasonAsync(userId, prediction.Race.SeasonId);
        var user = await _db.Users.FindAsync(userId);

        return Ok(new UserRaceScoreDto(
            userId, user!.Username, raceId, prediction.Race.Name,
            prediction.TotalPointsForRace ?? 0, cumulative));
    }

    // ==================
    //  HELPERS
    // ==================

    private int GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException();
        return int.Parse(sub);
    }

    private async Task<Prediction?> LoadFullPrediction(int userId, int raceId)
        => await _db.Predictions
            .Include(p => p.User)
            .Include(p => p.Race)
            .Include(p => p.RacePositions).ThenInclude(rp => rp.Driver)
            .Include(p => p.QualifyingPositions).ThenInclude(qp => qp.Driver)
            .Include(p => p.SprintPositions).ThenInclude(sp => sp.Driver)
            .Include(p => p.PredictedDnfDriver)
            .Include(p => p.PredictedFastestLapDriver)
            .Include(p => p.PredictedScoredPointsDriver)
            .Include(p => p.PredictedFastestPitStopDriver)
            .Include(p => p.PredictedDriverOfTheDay)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.RaceId == raceId);

    private void AddPredictionPositions(int predictionId, SubmitPredictionRequest request, int raceId)
    {
        foreach (var pos in request.RacePositions)
            _db.PredictionRacePositions.Add(new PredictionRacePosition { PredictionId = predictionId, Position = pos.Position, DriverId = pos.DriverId });

        foreach (var pos in request.QualifyingPositions)
            _db.PredictionQualifyingPositions.Add(new PredictionQualifyingPosition { PredictionId = predictionId, Position = pos.Position, DriverId = pos.DriverId });

        if (request.SprintPositions != null)
            foreach (var pos in request.SprintPositions)
                _db.PredictionSprintPositions.Add(new PredictionSprintPosition { PredictionId = predictionId, Position = pos.Position, DriverId = pos.DriverId });
    }

    private static PredictionDto MapToPredictionDto(
        Prediction p,
        List<RaceResult> raceResults,
        List<QualifyingResult> qualResults,
        List<SprintResult> sprintResults)
    {
        return new PredictionDto(
            p.Id, p.UserId, p.User?.Username ?? "",
            p.RaceId, p.Race?.Name ?? "",
            p.SubmittedAt, p.UpdatedAt, p.IsLocked, p.TotalPointsForRace,
            p.RacePositions.OrderBy(x => x.Position).Select(pos =>
            {
                var isCorrect = raceResults.Any(r => r.Position == pos.Position && r.DriverId == pos.DriverId);
                return new PredictionPositionDetailDto(pos.Position, pos.DriverId, pos.Driver?.FullName ?? "", pos.Driver?.Code ?? "", isCorrect);
            }).ToList(),
            p.QualifyingPositions.OrderBy(x => x.Position).Select(pos =>
            {
                var isCorrect = qualResults.Any(r => r.Position == pos.Position && r.DriverId == pos.DriverId);
                return new PredictionPositionDetailDto(pos.Position, pos.DriverId, pos.Driver?.FullName ?? "", pos.Driver?.Code ?? "", isCorrect);
            }).ToList(),
            p.SprintPositions.Any() ? p.SprintPositions.OrderBy(x => x.Position).Select(pos =>
            {
                var isCorrect = sprintResults.Any(r => r.Position == pos.Position && r.DriverId == pos.DriverId);
                return new PredictionPositionDetailDto(pos.Position, pos.DriverId, pos.Driver?.FullName ?? "", pos.Driver?.Code ?? "", isCorrect);
            }).ToList() : null,
            new PredictionExtrasDetailDto(
                p.PredictedDnfDriver == null ? null : new DriverSummaryDto(p.PredictedDnfDriver.Id, p.PredictedDnfDriver.FullName, p.PredictedDnfDriver.Code, p.PredictedDnfDriver.DriverNumber, null),
                p.PredictedFastestLapDriver == null ? null : new DriverSummaryDto(p.PredictedFastestLapDriver.Id, p.PredictedFastestLapDriver.FullName, p.PredictedFastestLapDriver.Code, p.PredictedFastestLapDriver.DriverNumber, null),
                p.PredictedScoredPointsDriver == null ? null : new DriverSummaryDto(p.PredictedScoredPointsDriver.Id, p.PredictedScoredPointsDriver.FullName, p.PredictedScoredPointsDriver.Code, p.PredictedScoredPointsDriver.DriverNumber, null),
                p.PredictedFastestPitStopDriver == null ? null : new DriverSummaryDto(p.PredictedFastestPitStopDriver.Id, p.PredictedFastestLapDriver?.FullName ?? "", p.PredictedFastestPitStopDriver.Code, p.PredictedFastestPitStopDriver.DriverNumber, null),
                p.PredictedDriverOfTheDay == null ? null : new DriverSummaryDto(p.PredictedDriverOfTheDay.Id, p.PredictedDriverOfTheDay.FullName, p.PredictedDriverOfTheDay.Code, p.PredictedDriverOfTheDay.DriverNumber, null),
                p.PredictedSafetyCarCount
            )
        );
    }
}
