namespace Fenio1.API.Controllers;

using Fenio1.API.DTOs;
using Fenio1.Core.Entities;
using Fenio1.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class DriversController : ControllerBase
{
    private readonly AppDbContext _db;

    public DriversController(AppDbContext db) => _db = db;

    /// <summary>
    /// Dohvaća sve aktivne vozače (za dropdown liste).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DriverDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
    {
        var query = _db.Drivers
            .Include(d => d.Team)
            .AsQueryable();

        if (activeOnly) query = query.Where(d => d.IsActive);

        var drivers = await query
            .OrderBy(d => d.LastName)
            .Select(d => new DriverDto(
                d.Id, d.FirstName, d.LastName, d.FirstName + " " + d.LastName,
                d.Code, d.DriverNumber, d.Nationality, d.IsActive,
                d.TeamId, d.Team != null ? d.Team.ShortName : null))
            .ToListAsync();

        return Ok(drivers);
    }

    /// <summary>
    /// Dohvaća sažetak vozača (za dropdown - Id, ime, kod).
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(IEnumerable<DriverSummaryDto>), 200)]
    public async Task<IActionResult> GetSummary()
    {
        var drivers = await _db.Drivers
            .Include(d => d.Team)
            .Where(d => d.IsActive)
            .OrderBy(d => d.LastName)
            .Select(d => new DriverSummaryDto(
                d.Id, d.FirstName + " " + d.LastName, d.Code, d.DriverNumber,
                d.Team != null ? d.Team.ShortName : null))
            .ToListAsync();

        return Ok(drivers);
    }

    /// <summary>
    /// Dohvaća jednog vozača po ID-u.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DriverDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var d = await _db.Drivers.Include(x => x.Team).FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return NotFound();

        return Ok(new DriverDto(d.Id, d.FirstName, d.LastName, d.FirstName + " " + d.LastName,
            d.Code, d.DriverNumber, d.Nationality, d.IsActive,
            d.TeamId, d.Team?.ShortName));
    }

    /// <summary>
    /// Kreira vozača. Samo admin.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DriverDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateDriverRequest request)
    {
        if (await _db.Drivers.AnyAsync(d => d.Code == request.Code.ToUpper()))
            return Conflict(new { message = $"Vozač s kodom '{request.Code}' već postoji." });

        if (await _db.Drivers.AnyAsync(d => d.DriverNumber == request.DriverNumber))
            return Conflict(new { message = $"Broj '{request.DriverNumber}' već je zauzet." });

        var driver = new Driver
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Code = request.Code.ToUpper(),
            DriverNumber = request.DriverNumber,
            Nationality = request.Nationality,
            DateOfBirth = request.DateOfBirth,
            TeamId = request.TeamId,
            IsActive = true
        };

        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync();

        await _db.Entry(driver).Reference(d => d.Team).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = driver.Id },
            new DriverDto(driver.Id, driver.FirstName, driver.LastName, driver.FullName,
                driver.Code, driver.DriverNumber, driver.Nationality, driver.IsActive,
                driver.TeamId, driver.Team?.ShortName));
    }

    /// <summary>
    /// Ažurira vozača. Samo admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DriverDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverRequest request)
    {
        var driver = await _db.Drivers.Include(d => d.Team).FirstOrDefaultAsync(d => d.Id == id);
        if (driver == null) return NotFound();

        if (request.FirstName != null) driver.FirstName = request.FirstName;
        if (request.LastName != null) driver.LastName = request.LastName;
        if (request.Code != null) driver.Code = request.Code.ToUpper();
        if (request.DriverNumber.HasValue) driver.DriverNumber = request.DriverNumber.Value;
        if (request.Nationality != null) driver.Nationality = request.Nationality;
        if (request.IsActive.HasValue) driver.IsActive = request.IsActive.Value;

        // TeamId može biti null (vozač bez tima)
        if (request.TeamId.HasValue || request.TeamId == null)
            driver.TeamId = request.TeamId;

        await _db.SaveChangesAsync();
        await _db.Entry(driver).Reference(d => d.Team).LoadAsync();

        return Ok(new DriverDto(driver.Id, driver.FirstName, driver.LastName, driver.FullName,
            driver.Code, driver.DriverNumber, driver.Nationality, driver.IsActive,
            driver.TeamId, driver.Team?.ShortName));
    }

    /// <summary>
    /// Briše vozača. Samo admin.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _db.Drivers.FindAsync(id);
        if (driver == null) return NotFound();

        driver.IsActive = false; // Soft delete
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
