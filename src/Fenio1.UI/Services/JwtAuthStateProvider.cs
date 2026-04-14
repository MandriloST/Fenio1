namespace Fenio1.UI.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private const string TokenKey = "fenio1_token";

    public JwtAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return Anonymous();

        // Provjeri expiry
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync(TokenKey);
                return Anonymous();
            }
        }
        catch
        {
            return Anonymous();
        }

        SetAuthHeader(token);
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        SetAuthHeader(token);
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task LogoutAsync()
    {
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
