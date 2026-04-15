namespace Fenio1.UI.Services;

using System.Net.Http.Json;
using Fenio1.UI.Models;
using Microsoft.Extensions.Logging;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient http, ILogger<ApiService> logger)
    {
        _http = http;
        _logger = logger;
    }

    // ========================
    //  AUTH
    // ========================

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("LOGIN → username: {Username}", request.Username);
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            _logger.LogInformation("LOGIN ← status: {Status}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("LOGIN FAILED: {Body}", body);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LOGIN EXCEPTION");
            return null;
        }
    }

    // ========================
    //  SEASONS
    // ========================

    public async Task<List<SeasonDto>> GetSeasonsAsync()
       => await _http.GetFromJsonAsync<List<SeasonDto>>("api/seasons") ?? new();

    public async Task<SeasonDto?> GetActiveSeasonAsync()
    {
        try { return await _http.GetFromJsonAsync<SeasonDto>("api/seasons/active"); }
        catch { return null; }
    }

    // ========================
    //  RACES
    // ========================

    public async Task<List<RaceSummaryDto>> GetRacesAsync(int? seasonId = null)
    {
        var url = seasonId.HasValue ? $"api/races?seasonId={seasonId}" : "api/races";
        return await _http.GetFromJsonAsync<List<RaceSummaryDto>>(url) ?? new();
    }

    public async Task<RaceDto?> GetRaceAsync(int id)
        => await _http.GetFromJsonAsync<RaceDto>($"api/races/{id}");

    public async Task<RaceResultsDto?> GetRaceResultsAsync(int raceId)
    {
        try { return await _http.GetFromJsonAsync<RaceResultsDto>($"api/races/{raceId}/results"); }
        catch { return null; }
    }

    public async Task<bool> SubmitRaceResultsAsync(int raceId, SubmitRaceResultsRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/races/{raceId}/results", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<RaceDto?> CreateRaceAsync(CreateRaceRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/races", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<RaceDto>();
    }

    // ========================
    //  DRIVERS
    // ========================

    public async Task<List<DriverSummaryDto>> GetDriversSummaryAsync()
        => await _http.GetFromJsonAsync<List<DriverSummaryDto>>("api/drivers/summary") ?? new();

    public async Task<List<DriverDto>> GetDriversAsync()
        => await _http.GetFromJsonAsync<List<DriverDto>>("api/drivers") ?? new();

    public async Task<DriverDto?> CreateDriverAsync(CreateDriverRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/drivers", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<DriverDto>();
    }

    public async Task<bool> DeleteDriverAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/drivers/{id}");
        return response.IsSuccessStatusCode;
    }

    // ========================
    //  TEAMS
    // ========================

    public async Task<List<TeamDto>> GetTeamsAsync()
        => await _http.GetFromJsonAsync<List<TeamDto>>("api/teams") ?? new();

    public async Task<TeamDto?> CreateTeamAsync(CreateTeamRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/teams", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TeamDto>();
    }

    // ========================
    //  CIRCUITS
    // ========================

    public async Task<List<CircuitDto>> GetCircuitsAsync()
        => await _http.GetFromJsonAsync<List<CircuitDto>>("api/circuits") ?? new();

    public async Task<CircuitDto?> CreateCircuitAsync(CreateCircuitRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/circuits", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CircuitDto>();
    }

    // ========================
    //  PREDICTIONS
    // ========================

    public async Task<PredictionDto?> GetMyPredictionForRaceAsync(int raceId)
    {
        try { return await _http.GetFromJsonAsync<PredictionDto>($"api/predictions/my/race/{raceId}"); }
        catch { return null; }
    }

    public async Task<List<PredictionDto>> GetMyPredictionsAsync(int? seasonId = null)
    {
        var url = seasonId.HasValue ? $"api/predictions/my?seasonId={seasonId}" : "api/predictions/my";
        return await _http.GetFromJsonAsync<List<PredictionDto>>(url) ?? new();
    }

    public async Task<(bool Success, string? Error)> SubmitPredictionAsync(int raceId, SubmitPredictionRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/predictions/race/{raceId}", request);
        if (response.IsSuccessStatusCode) return (true, null);
        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<List<PredictionDto>> GetAllPredictionsForRaceAsync(int raceId)
        => await _http.GetFromJsonAsync<List<PredictionDto>>($"api/predictions/race/{raceId}") ?? new();

    // ========================
    //  LEADERBOARD
    // ========================

    public async Task<List<LeaderboardEntryDto>> GetActiveLeaderboardAsync()
        => await _http.GetFromJsonAsync<List<LeaderboardEntryDto>>("api/leaderboard/active") ?? new();

    public async Task<List<LeaderboardEntryDto>> GetSeasonLeaderboardAsync(int seasonId)
        => await _http.GetFromJsonAsync<List<LeaderboardEntryDto>>($"api/leaderboard/season/{seasonId}") ?? new();

    public async Task<RaceLeaderboardDto?> GetRaceLeaderboardAsync(int raceId)
        => await _http.GetFromJsonAsync<RaceLeaderboardDto>($"api/leaderboard/race/{raceId}");

    // ========================
    //  USERS (Admin)
    // ========================

    public async Task<List<UserDto>> GetUsersAsync()
        => await _http.GetFromJsonAsync<List<UserDto>>("api/users") ?? new();

    public async Task<UserDto?> CreateUserAsync(CreateUserRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/users", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/users/{id}");
        return response.IsSuccessStatusCode;
    }
}
