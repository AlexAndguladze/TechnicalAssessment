using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using GeocodeAssesment.Services;
using GeocodeAssessment.Interfaces;
using GeocodeAssessment.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TechnicalAssessment;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddHttpClient();

        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient());
        
        services.AddSingleton<IGeocodingService, GoogleGeocodingService>();
        services.AddSingleton<ICacheService, DynamoDbCacheService>();
    }
}
