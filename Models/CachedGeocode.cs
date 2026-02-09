namespace GeocodeAssessment.Models;

public class CachedGeocode
{
    public string Address { get; set; }
    public string Response { get; set; }
    public long TTL { get; set; }
}