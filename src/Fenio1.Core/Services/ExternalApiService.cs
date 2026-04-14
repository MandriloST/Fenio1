namespace Fenio1.Core.Services;

using Fenio1.Core.Interfaces;

/// <summary>
/// Stub za vanjski F1 API servis.
/// Implementirajte MapExternalResultsAsync prema API-u koji koristite (npr. Ergast, OpenF1, SportMonks).
/// </summary>
public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;

    public ExternalApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Dohvaća sirove podatke iz vanjskog API-a.
    /// TODO: Implementirajte mapiranje prema odabranom F1 API-u.
    ///
    /// Preporučeni API-ji:
    /// - Ergast API: http://ergast.com/mrd/ (besplatan, ali deprecated)
    /// - OpenF1 API: https://openf1.org/ (besplatan, real-time)
    /// - SportMonks F1: https://docs.sportmonks.com/ (plaćen)
    ///
    /// Primjer za OpenF1:
    ///   GET https://api.openf1.org/v1/sessions?year=2024&session_name=Race&meeting_key=XX
    ///   GET https://api.openf1.org/v1/position?session_key=XX
    /// </summary>
    public async Task<ExternalRaceData?> FetchRaceResultsAsync(string externalRaceId)
    {
        // TODO: Implementirati prema odabranom API-u
        // Primjer strukture:
        //
        // var response = await _httpClient.GetAsync($"https://api.yourF1provider.com/races/{externalRaceId}");
        // var json = await response.Content.ReadAsStringAsync();
        // var rawData = JsonSerializer.Deserialize<YourApiResponseModel>(json);
        // return MapToExternalRaceData(rawData);

        // Vraća null dok nije implementirano
        await Task.CompletedTask;
        return null;
    }

    /// <summary>
    /// TODO: Implementirati mapiranje sirovih podataka vanjskog API-a u ExternalRaceData model.
    /// Poziva se iz FetchRaceResultsAsync.
    /// </summary>
    private ExternalRaceData MapToExternalRaceData(object rawData)
    {
        // TODO: Implementirati
        throw new NotImplementedException(
            "Implementirajte mapiranje vanjskog API-a prema ExternalRaceData modelu.");
    }
}
