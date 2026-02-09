using Amazon.Runtime;
using GeocodeAssessment.Interfaces;

namespace GeocodeAssessment.Services;

public class GoogleGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GoogleGeocodingService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? throw new InvalidOperationException("API key not found in environment variables.");
    }

    public async Task<string> GeocodeAddressAsync(string address)
    {
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
              
    }
}