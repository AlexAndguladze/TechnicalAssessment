namespace GeocodeAssessment.Interfaces;

public interface ICacheService
{
    Task<string> GetCachedResponseAsync(string address);
    Task SaveResponseAsync(string address, string response);
}