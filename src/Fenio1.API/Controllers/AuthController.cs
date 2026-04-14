namespace Fenio1.API.Controllers;

using Fenio1.API.DTOs;
using Fenio1.Infrastructure.Data;
using Fenio1.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Autentifikacija - login korisnika.
/// Registracija nije dostupna kroz API, korisnici se unose ručno u bazu ili admin endpointom.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthController(AppDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Login - vraća JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Neispravan username ili lozinka." });

        var token = _jwtService.GenerateToken(user);
        var expiry = _jwtService.GetTokenExpiry();

        return Ok(new LoginResponse(token, user.Username, user.Email, user.Role.ToString(), expiry));
    }
}
