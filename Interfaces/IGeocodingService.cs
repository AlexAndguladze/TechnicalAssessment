
namespace GeocodeAssessment.Interfaces;
public interface IGeocodingService
{
    Task<string> GeocodeAddressAsync(string address);
}