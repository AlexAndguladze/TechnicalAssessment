using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using GeocodeAssessment.Interfaces;
using Microsoft.Extensions.Logging;

namespace GeocodeAssesment.Services;

public class DynamoDbCacheService : ICacheService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    private readonly ILogger _logger;

    public DynamoDbCacheService(IAmazonDynamoDB dynamoDb, ILogger<DynamoDbCacheService> logger)
    {
        _dynamoDb = dynamoDb;
        _tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE") ?? throw new InvalidOperationException("DynamoDB table name not found in environment variables.");
        _logger = logger;
    }
    public async Task<string> GetCachedResponseAsync(string address)
    {
        try
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Address", new AttributeValue { S = address } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);
            
            if (response.Item.Count == 0)
            {
                return null;
            }

            var ttl = long.Parse(response.Item["TTL"].N);
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if(ttl < currentTime)
            {
                return null;
            }

            return response.Item["Response"].S;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving cached response for address: {Address}", address);

            return null;
        }
    }

    public async Task SaveResponseAsync(string address, string response)
    {
        var ttl = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Address", new AttributeValue { S = address } },
                { "Response", new AttributeValue { S = response } },
                { "TTL", new AttributeValue { N = ttl.ToString() } }
            
            }
        };

        await _dynamoDb.PutItemAsync(request);
    }
}