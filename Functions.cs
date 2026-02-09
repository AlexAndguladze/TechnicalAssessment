using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using GeocodeAssessment.Interfaces;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TechnicalAssessment;

public class Functions
{
    private readonly IGeocodingService _geocodingService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<Functions> _logger;

    public Functions(IGeocodingService geocodingService, ICacheService cacheService, ILogger<Functions> logger)
    {
        _geocodingService = geocodingService;
        _cacheService = cacheService;
        _logger = logger;
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Get, "/Geocode")]
    public async Task<IHttpResult> Geocode([FromQuery] string address)
    {
        try
        {
            _logger.LogInformation("Geocoding: {Address}", address);

            if (string.IsNullOrEmpty(address))
            {
                return HttpResults.BadRequest("Address is required");
            }

            var cachedResponse = await _cacheService.GetCachedResponseAsync(address);

            if(cachedResponse != null)
            {
                _logger.LogInformation("Cache hit for address: {Address}", address);
                return HttpResults.Ok(cachedResponse);
            }

            var geocodeResponse = await _geocodingService.GeocodeAddressAsync(address);

            await _cacheService.SaveResponseAsync(address, geocodeResponse);

            return HttpResults.Ok(geocodeResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", address);
            return HttpResults.InternalServerError("An error occurred while processing the request.");
        }
    }
}
