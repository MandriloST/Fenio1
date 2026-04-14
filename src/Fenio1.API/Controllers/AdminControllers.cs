namespace Fenio1.API.Controllers;

using Fenio1.API.DTOs;
using Fenio1.Core.Entities;
using Fenio1.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// ============================
//  TEAMS
// ============================

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TeamsController(AppDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TeamDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
    {
        var query = _db.Teams.AsQueryable();
        if (activeOnly) query = query.Where(t => t.IsActive);

        var teams = await query.OrderBy(t => t.Name)
            .Select(t => new TeamDto(t.Id, t.Name, t.ShortName, t.Nationality, t.PrimaryColor, t.IsActive))
            .ToListAsync();

        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TeamDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Teams.FindAsync(id);
        if (t == null) return NotFound();
        return Ok(new TeamDto(t.Id, t.Name, t.ShortName, t.Nationality, t.PrimaryColor, t.IsActive));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TeamDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request)
    {
        var team = new Team
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Nationality = request.Nationality,
            PrimaryColor = request.PrimaryColor,
            IsActive = true
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var dto = new TeamDto(team.Id, team.Name, team.ShortName, team.Nationality, team.PrimaryColor, team.IsActive);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TeamDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamRequest request)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null) return NotFound();

        if (request.Name != null) team.Name = request.Name;
        if (request.ShortName != null) team.ShortName = request.ShortName;
        if (request.Nationality != null) team.Nationality = request.Nationality;
        if (request.PrimaryColor != null) team.PrimaryColor = request.PrimaryColor;
        if (request.IsActive.HasValue) team.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync();
        return Ok(new TeamDto(team.Id, team.Name, team.ShortName, team.Nationality, team.PrimaryColor, team.IsActive));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team == null) return NotFound();
        team.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

// ============================
//  CIRCUITS
// ============================

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class CircuitsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CircuitsController(AppDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CircuitDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var circuits = await _db.Circuits.OrderBy(c => c.Name)
            .Select(c => new CircuitDto(c.Id, c.Name, c.Location, c.Country, c.CountryCode, c.LapCount, c.LengthKm))
            .ToListAsync();
        return Ok(circuits);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CircuitDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _db.Circuits.FindAsync(id);
        if (c == null) return NotFound();
        return Ok(new CircuitDto(c.Id, c.Name, c.Location, c.Country, c.CountryCode, c.LapCount, c.LengthKm));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CircuitDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCircuitRequest request)
    {
        var circuit = new Circuit
        {
            Name = request.Name,
            Location = request.Location,
            Country = request.Country,
            CountryCode = request.CountryCode.ToUpper(),
            LapCount = request.LapCount,
            LengthKm = request.LengthKm
        };

        _db.Circuits.Add(circuit);
        await _db.SaveChangesAsync();

        var dto = new CircuitDto(circuit.Id, circuit.Name, circuit.Location, circuit.Country, circuit.CountryCode, circuit.LapCount, circuit.LengthKm);
        return CreatedAtAction(nameof(GetById), new { id = circuit.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CircuitDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCircuitRequest request)
    {
        var c = await _db.Circuits.FindAsync(id);
        if (c == null) return NotFound();

        if (request.Name != null) c.Name = request.Name;
        if (request.Location != null) c.Location = request.Location;
        if (request.Country != null) c.Country = request.Country;
        if (request.CountryCode != null) c.CountryCode = request.CountryCode.ToUpper();
        if (request.LapCount.HasValue) c.LapCount = request.LapCount.Value;
        if (request.LengthKm.HasValue) c.LengthKm = request.LengthKm.Value;

        await _db.SaveChangesAsync();
        return Ok(new CircuitDto(c.Id, c.Name, c.Location, c.Country, c.CountryCode, c.LapCount, c.LengthKm));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Circuits.FindAsync(id);
        if (c == null) return NotFound();
        _db.Circuits.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

// ============================
//  SEASONS
// ============================

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SeasonsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SeasonsController(AppDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SeasonDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var seasons = await _db.Seasons.OrderByDescending(s => s.Year)
            .Select(s => new SeasonDto(s.Id, s.Year, s.Name, s.IsActive))
            .ToListAsync();
        return Ok(seasons);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SeasonDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Seasons.FindAsync(id);
        if (s == null) return NotFound();
        return Ok(new SeasonDto(s.Id, s.Year, s.Name, s.IsActive));
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(SeasonDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetActive()
    {
        var s = await _db.Seasons.FirstOrDefaultAsync(s => s.IsActive);
        if (s == null) return NotFound(new { message = "Nema aktivne sezone." });
        return Ok(new SeasonDto(s.Id, s.Year, s.Name, s.IsActive));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SeasonDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateSeasonRequest request)
    {
        if (await _db.Seasons.AnyAsync(s => s.Year == request.Year))
            return Conflict(new { message = $"Sezona {request.Year} već postoji." });

        var season = new Season { Year = request.Year, Name = request.Name, IsActive = false };
        _db.Seasons.Add(season);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = season.Id },
            new SeasonDto(season.Id, season.Year, season.Name, season.IsActive));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SeasonDto), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSeasonRequest request)
    {
        var season = await _db.Seasons.FindAsync(id);
        if (season == null) return NotFound();

        if (request.Name != null) season.Name = request.Name;
        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
            {
                // Deaktiviraj sve ostale sezone
                var others = await _db.Seasons.Where(s => s.Id != id && s.IsActive).ToListAsync();
                others.ForEach(s => s.IsActive = false);
            }
            season.IsActive = request.IsActive.Value;
        }

        await _db.SaveChangesAsync();
        return Ok(new SeasonDto(season.Id, season.Year, season.Name, season.IsActive));
    }
}
