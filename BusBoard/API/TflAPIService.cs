using System.Diagnostics;
using System.Text.Json;
using BusBoard.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace BusBoard.API;

public class TflAPIService
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    
    private readonly RestClientOptions _options = new("https://api.tfl.gov.uk");

    public RestClient Client { get; }
    public TflAPIService()
    {
        Client = new RestClient(_options);
    }

    public async Task<List<BusArrivalPrediction>> GetNextNBussesAtStop(string stopId, int n, IConfigurationRoot config)
    {
        if (n <= 0)
        {
            throw new ArgumentException("n must be greater than 0");
        }

        RestRequest request = new RestRequest("StopPoint/{id}/Arrivals")
            .AddUrlSegment("id", stopId)
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        RestResponse response = await Client.GetAsync(request);

        List<BusArrivalPrediction>? data = null;

        try
        {
            data = JsonSerializer.Deserialize<List<BusArrivalPrediction>>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            throw new Exception(error.Message);
        }

        if (data is null | data!.Count == 0)
        {
            throw new Exception("Data could not be Deserialized");
        }

        data.Sort((predictionA, predictionB) => DateTime.Compare(predictionA.ExpectedArrival, predictionB.ExpectedArrival));

        if (n >= data.Count)
        {
            n = data.Count;
            Debug.WriteLine($"Only {data.Count} arrivals found, returning all of them.");
        }

        return [.. data.Take(n)];
    }
}