namespace Fenio1.UI.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JwtAuthStateProvider> _logger;
    private const string TokenKey = "fenio1_token";

    public JwtAuthStateProvider(
        ILocalStorageService localStorage,
        HttpClient httpClient,
        ILogger<JwtAuthStateProvider> logger)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogDebug("GetAuthState → nema tokena, Anonymous");
            return Anonymous();
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("GetAuthState → token istekao ({Expiry}), briše se", jwt.ValidTo);
                await _localStorage.RemoveItemAsync(TokenKey);
                return Anonymous();
            }

            _logger.LogDebug("GetAuthState → autentificiran kao {Name}, uloga: {Role}",
                jwt.Subject,
                jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAuthState → nevažeći token");
            return Anonymous();
        }

        SetAuthHeader(token);
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(string token)
    {
        _logger.LogInformation("LoginAsync → sprema token u LocalStorage");
        await _localStorage.SetItemAsync(TokenKey, token);
        SetAuthHeader(token);
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task LogoutAsync()
    {
        _logger.LogInformation("LogoutAsync → briše token");
        await _localStorage.RemoveItemAsync(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    public async Task<string?> GetTokenAsync()
        => await _localStorage.GetItemAsync<string>(TokenKey);

    private void SetAuthHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        _logger.LogDebug("SetAuthHeader → Authorization header postavljen");
    }

    private static AuthenticationState Anonymous()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims;
    }
}
