namespace Fenio1.API.Controllers;

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
public class RacesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IScoringService _scoring;
    private readonly IExternalApiService _externalApi;

    public RacesController(AppDbContext db, IScoringService scoring, IExternalApiService externalApi)
    {
        _db = db;
        _scoring = scoring;
        _externalApi = externalApi;
    }

    // ==================
    //  READ
    // ==================

    /// <summary>
    /// Dohvaća sve utrke, opcionalno filtrirane po sezoni.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RaceSummaryDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int? seasonId)
    {
        var query = _db.Races.Include(r => r.Circuit).Include(r => r.Season).AsQueryable();

        if (seasonId.HasValue) query = query.Where(r => r.SeasonId == seasonId);

        var races = await query
            .OrderBy(r => r.RoundNumber)
            .Select(r => new RaceSummaryDto(
                r.Id, r.Name, r.RoundNumber, r.RaceDate, r.HasSprint,
                r.Status.ToString(), r.Circuit.Name, r.Circuit.Country))
            .ToListAsync();

        return Ok(races);
    }

    /// <summary>
    /// Dohvaća detalje jedne utrke.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RaceDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _db.Races
            .Include(x => x.Circuit)
            .Include(x => x.Season)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (r == null) return NotFound();

        return Ok(MapToRaceDto(r));
    }

    /// <summary>
    /// Dohvaća rezultate utrke.
    /// </summary>
    [HttpGet("{id:int}/results")]
    [ProducesResponseType(typeof(RaceResultsDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetResults(int id)
    {
        var race = await _db.Races.FindAsync(id);
        if (race == null) return NotFound();

        var raceResults = await _db.RaceResults
            .Include(r => r.Driver)
            .Where(r => r.RaceId == id)
            .OrderBy(r => r.Position)
            .ToListAsync();

        var qualResults = await _db.QualifyingResults
            .Include(r => r.Driver)
            .Where(r => r.RaceId == id)
            .OrderBy(r => r.Position)
            .ToListAsync();

        var sprintResults = race.HasSprint ? await _db.SprintResults
            .Include(r => r.Driver)
            .Where(r => r.RaceId == id)
            .OrderBy(r => r.Position)
            .ToListAsync() : null;


        var extra = await _db.RaceExtras
            .Include(e => e.FastestLapDriver)
            //.Include(e => e.ScoredPointsDriver)
            .Include(e => e.FastestPitStopDriver)
            .Include(e => e.DriverOfTheDay)
            .FirstOrDefaultAsync(e => e.RaceId == id);

        // NOVO - učitaj DNF listu zasebno
        var dnfEntries = await _db.RaceDnfEntries
            .Include(d => d.Driver)
            .Where(d => d.RaceId == id)
            .ToListAsync();
       

        return Ok(new RaceResultsDto(
            id,
            race.Name,
            raceResults.Select(r => new RacePositionDetailDto(r.Position, r.DriverId, r.Driver.FullName, r.Driver.Code, r.DidNotFinish)).ToList(),
            qualResults.Select(r => new QualifyingPositionDetailDto(r.Position, r.DriverId, r.Driver.FullName, r.Driver.Code)).ToList(),
            sprintResults?.Select(r => new SprintPositionDetailDto(r.Position, r.DriverId, r.Driver.FullName, r.Driver.Code)).ToList(),
            extra == null && !dnfEntries.Any() ? null : new RaceExtraDetailDto(
                dnfEntries.Select(d => new DriverSummaryDto(
                d.Driver.Id, d.Driver.FullName, d.Driver.Code, d.Driver.DriverNumber, null
                )).ToList(),
                //extra.DnfDriver == null ? null : new DriverSummaryDto(extra.DnfDriver.Id, extra.DnfDriver.FullName, extra.DnfDriver.Code, extra.DnfDriver.DriverNumber, null),
                extra?.FastestLapDriver == null ? null : new DriverSummaryDto(extra.FastestLapDriver.Id, extra.FastestLapDriver.FullName, extra.FastestLapDriver.Code, extra.FastestLapDriver.DriverNumber, null),
                //extra.ScoredPointsDriver == null ? null : new DriverSummaryDto(extra.ScoredPointsDriver.Id, extra.ScoredPointsDriver.FullName, extra.ScoredPointsDriver.Code, extra.ScoredPointsDriver.DriverNumber, null),
                extra?.FastestPitStopDriver == null ? null : new DriverSummaryDto(extra.FastestPitStopDriver.Id, extra.FastestPitStopDriver.FullName, extra.FastestPitStopDriver.Code, extra.FastestPitStopDriver.DriverNumber, null),
                extra?.DriverOfTheDay == null ? null : new DriverSummaryDto(extra.DriverOfTheDay.Id, extra.DriverOfTheDay.FullName, extra.DriverOfTheDay.Code, extra.DriverOfTheDay.DriverNumber, null),
                extra?.SafetyCarCount ?? 0
            )
        ));
    }

    // ==================
    //  ADMIN - CRUD
    // ==================

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RaceDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRaceRequest request)
    {
        if (!await _db.Seasons.AnyAsync(s => s.Id == request.SeasonId))
            return BadRequest(new { message = "Sezona ne postoji." });

        if (!await _db.Circuits.AnyAsync(c => c.Id == request.CircuitId))
            return BadRequest(new { message = "Staza ne postoji." });

        if (await _db.Races.AnyAsync(r => r.SeasonId == request.SeasonId && r.RoundNumber == request.RoundNumber))
            return Conflict(new { message = $"Runda {request.RoundNumber} već postoji u toj sezoni." });

        var race = new Race
        {
            SeasonId = request.SeasonId,
            CircuitId = request.CircuitId,
            Name = request.Name,
            RoundNumber = request.RoundNumber,
            RaceDate = request.RaceDate,
            QualifyingDate = request.QualifyingDate,
            SprintDate = request.SprintDate,
            HasSprint = request.HasSprint,
            Status = RaceStatus.Upcoming,
            ExternalApiId = request.ExternalApiId
        };

        _db.Races.Add(race);
        await _db.SaveChangesAsync();
        await _db.Entry(race).Reference(r => r.Circuit).LoadAsync();
        await _db.Entry(race).Reference(r => r.Season).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = race.Id }, MapToRaceDto(race));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RaceDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRaceRequest request)
    {
        var race = await _db.Races
            .Include(r => r.Circuit).Include(r => r.Season)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (race == null) return NotFound();

        if (request.CircuitId.HasValue) race.CircuitId = request.CircuitId.Value;
        if (request.Name != null) race.Name = request.Name;
        if (request.RoundNumber.HasValue) race.RoundNumber = request.RoundNumber.Value;
        if (request.RaceDate.HasValue) race.RaceDate = request.RaceDate.Value;
        if (request.QualifyingDate.HasValue) race.QualifyingDate = request.QualifyingDate.Value;
        if (request.SprintDate.HasValue) race.SprintDate = request.SprintDate.Value;
        if (request.HasSprint.HasValue) race.HasSprint = request.HasSprint.Value;
        if (request.Status.HasValue) race.Status = request.Status.Value;
        if (request.ExternalApiId != null) race.ExternalApiId = request.ExternalApiId;

        await _db.SaveChangesAsync();
        await _db.Entry(race).Reference(r => r.Circuit).LoadAsync();
        await _db.Entry(race).Reference(r => r.Season).LoadAsync();

        return Ok(MapToRaceDto(race));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        var race = await _db.Races.FindAsync(id);
        if (race == null) return NotFound();
        _db.Races.Remove(race);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ==================
    //  ADMIN - UNOS REZULTATA
    // ==================

    /// <summary>
    /// Admin unosi kompletne rezultate utrke (ručno).
    /// Sadrži: top 10 utrke, top 3 kvalifikacije, top 3 sprint (ako postoji), i posebne kategorije.
    /// Nakon unosa, automatski izračunava bodove za sve prognoze.
    /// </summary>
    [HttpPost("{id:int}/results")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SubmitResults(int id, [FromBody] SubmitRaceResultsRequest request)
    {
        var race = await _db.Races.FindAsync(id);
        if (race == null) return NotFound();

        // Validacija
        if (request.RacePositions.Count != 10)
            return BadRequest(new { message = "Potrebno je unijeti točno 10 pozicija za utrku." });

        if (request.QualifyingPositions.Count != 3)
            return BadRequest(new { message = "Potrebno je unijeti točno 3 pozicije za kvalifikacije." });

        if (race.HasSprint && (request.SprintPositions == null || request.SprintPositions.Count != 3))
            return BadRequest(new { message = "Ova utrka ima sprint - potrebno je unijeti 3 sprint pozicije." });

        // Obriši stare rezultate
        var oldRaceResults = await _db.RaceResults.Where(r => r.RaceId == id).ToListAsync();
        var oldQualResults = await _db.QualifyingResults.Where(r => r.RaceId == id).ToListAsync();
        var oldSprintResults = await _db.SprintResults.Where(r => r.RaceId == id).ToListAsync();
        var oldExtras = await _db.RaceExtras.Where(r => r.RaceId == id).ToListAsync();
        var oldDnf = await _db.RaceDnfEntries.Where(r => r.RaceId == id).ToListAsync(); // NOVO

        _db.RaceResults.RemoveRange(oldRaceResults);
        _db.QualifyingResults.RemoveRange(oldQualResults);
        _db.SprintResults.RemoveRange(oldSprintResults);
        _db.RaceExtras.RemoveRange(oldExtras);
        _db.RaceDnfEntries.RemoveRange(oldDnf); // NOVO

        // Unesi top 10 rezultate utrke (bez DNF-a ovdje)
        foreach (var pos in request.RacePositions)
        {
            _db.RaceResults.Add(new RaceResult
            {
                RaceId = id,
                DriverId = pos.DriverId,
                Position = pos.Position,
                DidNotFinish = false
            });
        }

        // Unesi DNF listu u zasebnu tablicu (admin može unijeti više vozača)
        var validDnfIds = (request.Extras.DnfDriverIds ?? new List<int>())
            .Where(dId => dId > 0)
            .Distinct()
            .ToList();

        foreach (var driverId in validDnfIds)
        {
            _db.RaceDnfEntries.Add(new RaceDnfEntry
            {
                RaceId = id,
                DriverId = driverId
            });
        }
        // Unesi kvalifikacije
        foreach (var pos in request.QualifyingPositions)
        {
            _db.QualifyingResults.Add(new QualifyingResult
            {
                RaceId = id,
                DriverId = pos.DriverId,
                Position = pos.Position
            });
        }

        // Unesi sprint (ako postoji)
        if (race.HasSprint && request.SprintPositions != null)
        {
            foreach (var pos in request.SprintPositions)
            {
                _db.SprintResults.Add(new SprintResult
                {
                    RaceId = id,
                    DriverId = pos.DriverId,
                    Position = pos.Position
                });
            }
        }

        // Unesi posebne kategorije
        _db.RaceExtras.Add(new RaceExtra
        {
            RaceId = id,
            //DnfDriverId = request.Extras.DnfDriverId,
            FastestLapDriverId = request.Extras.FastestLapDriverId,
            //ScoredPointsDriverId = request.Extras.ScoredPointsDriverId,
            FastestPitStopDriverId = request.Extras.FastestPitStopDriverId,
            DriverOfTheDayId = request.Extras.DriverOfTheDayId,
            SafetyCarCount = request.Extras.SafetyCarCount
        });

        // Označi utrku kao završenu
        race.Status = RaceStatus.Completed;

        await _db.SaveChangesAsync();

        // Izračunaj bodove za sve prognoze ove utrke
        await _scoring.RecalculateRacePointsAsync(id);

        // Ažuriraj leaderboard za sezonu
        await _scoring.UpdateLeaderboardAsync(race.SeasonId);

        return Ok(new { message = "Rezultati uspješno uneseni. Bodovi su izračunati.", raceId = id });
    }

    /// <summary>
    /// Admin uvozi rezultate iz vanjskog F1 API-a.
    /// Implementaciju mapiranja podataka treba dodati u ExternalApiService.
    /// </summary>
    [HttpPost("{id:int}/results/import-external")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(501)]
    public async Task<IActionResult> ImportFromExternalApi(int id)
    {
        var race = await _db.Races.FindAsync(id);
        if (race == null) return NotFound();

        if (string.IsNullOrEmpty(race.ExternalApiId))
            return BadRequest(new { message = "Utrka nema postavljen ExternalApiId. Unesite ga prije importa." });

        var externalData = await _externalApi.FetchRaceResultsAsync(race.ExternalApiId);

        if (externalData == null)
            return StatusCode(501, new
            {
                message = "Vanjski API servis još nije implementiran. " +
                          "Implementirajte FetchRaceResultsAsync u ExternalApiService.cs " +
                          "i MapToExternalRaceData prema odabranom F1 API-u (OpenF1, Ergast, SportMonks...)."
            });

        return Ok(new
        {
            message = "Podaci dohvaćeni iz vanjskog API-a. Mapiranje i unos još nisu implementirani.",
            externalData
        });
    }

    // ==================
    //  HELPER
    // ==================

    private static RaceDto MapToRaceDto(Race r) => new(
        r.Id, r.SeasonId, r.Season.Name, r.CircuitId,
        r.Circuit.Name, r.Circuit.Location, r.Circuit.Country,
        r.Name, r.RoundNumber, r.RaceDate, r.QualifyingDate, r.SprintDate,
        r.HasSprint, r.Status.ToString(), r.ExternalApiId
    );
}
