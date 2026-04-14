namespace Fenio1.API.Controllers;

using Fenio1.API.DTOs;
using Fenio1.Core.Entities;
using Fenio1.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Upravljanje korisnicima - samo admin.
/// Korisnici se ne mogu registrirati sami, već ih samo admin može kreirati.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    /// <summary>
    /// Dohvaća sve korisnike.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserDto(u.Id, u.Username, u.Email, u.Role.ToString(), u.IsActive, u.CreatedAt))
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Dohvaća jednog korisnika po ID-u.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new UserDto(user.Id, user.Username, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt));
    }

    /// <summary>
    /// Kreira novog korisnika (samo admin).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            return Conflict(new { message = $"Username '{request.Username}' već postoji." });

        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = $"Email '{request.Email}' već postoji." });

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new UserDto(user.Id, user.Username, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, dto);
    }

    /// <summary>
    /// Ažurira korisnika (admin može promijeniti email, lozinku, rolu, status).
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (request.Email != null)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
                return Conflict(new { message = $"Email '{request.Email}' već postoji." });
            user.Email = request.Email;
        }

        if (request.Password != null)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        if (request.Role.HasValue)
            user.Role = request.Role.Value;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync();

        return Ok(new UserDto(user.Id, user.Username, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt));
    }

    /// <summary>
    /// Briše korisnika.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Zaštita - ne može se obrisati zadnji admin
        if (user.Role == UserRole.Admin)
        {
            var adminCount = await _db.Users.CountAsync(u => u.Role == UserRole.Admin && u.IsActive);
            if (adminCount <= 1)
                return BadRequest(new { message = "Nije moguće obrisati zadnjeg aktivnog admina." });
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
