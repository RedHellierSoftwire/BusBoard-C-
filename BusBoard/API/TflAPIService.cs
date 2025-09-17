using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Linq;
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

    private readonly APIService _apiService = new("https://api.tfl.gov.uk");

    public async Task<List<BusArrivalPrediction>> GetNextBussesAtStop(string stopId, IConfigurationRoot config, int numberOfBusses = 5)
    {
        if (numberOfBusses <= 0)
        {
            throw new ArgumentException("n must be greater than 0");
        }

        RestRequest request = new RestRequest("StopPoint/{id}/Arrivals")
            .AddUrlSegment("id", stopId)
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        RestResponse response = await _apiService.Client.GetAsync(request);

        ImmutableList<BusArrivalPrediction>? data = null;

        try
        {
            data = JsonSerializer.Deserialize<ImmutableList<BusArrivalPrediction>>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            throw new Exception(error.Message);
        }

        if (data is null | data!.Count == 0)
        {
            throw new Exception("Data could not be Deserialized");
        }

        if (numberOfBusses >= data.Count)
        {
            numberOfBusses = data.Count;
            Debug.WriteLine($"Only {data.Count} arrivals found, returning all of them.");
        }

        IOrderedEnumerable<BusArrivalPrediction> orderedData = data.OrderBy(prediction => prediction.ExpectedArrival);

        return [.. orderedData.Take(numberOfBusses)];
    }
}