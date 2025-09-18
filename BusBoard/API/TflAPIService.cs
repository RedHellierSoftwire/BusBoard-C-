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

    public async Task<ImmutableList<BusArrivalPrediction>> GetBusArrivalPredictionsForStop(string stopId, IConfigurationRoot config)
    {
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

        if (data is null || data!.Count == 0)
        {
            throw new Exception("Data could not be Deserialized");
        }

        return data;

    }

    public async Task<StopPointSearchResponse> GetStopPointsNearLocation(double latitude, double longitude, bool expandSearch = false)
    {
        RestRequest request = new RestRequest("/StopPoint")
            .AddParameter("lat", latitude)
            .AddParameter("lon", longitude)
            .AddParameter("stopTypes", "NaptanPublicBusCoachTram")
            .AddParameter("radius", expandSearch ? 2000 : 500);

        RestResponse response = await _apiService.Client.GetAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }

        StopPointSearchResponse? data;

        try
        {
            data = JsonSerializer.Deserialize<StopPointSearchResponse>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            throw new Exception(error.Message);
        }

        if (data is null)
        {
            throw new Exception("Data could not be Deserialized");
        }

        return data;
    }

    public List<BusArrivalPrediction> GetNextBusses(ImmutableList<BusArrivalPrediction> predictions, int numberOfBusses = 5)
    {
        if (numberOfBusses <= 0)
        {
            throw new ArgumentException("Number of Busses must be greater than 0");
        }

        if (numberOfBusses > predictions.Count)
        {
            numberOfBusses = predictions.Count;
            Debug.WriteLine($"Only {predictions.Count} arrivals found, returning all of them.");
        }

        IOrderedEnumerable<BusArrivalPrediction> orderedData = predictions.OrderBy(prediction => prediction.ExpectedArrival);

        return [.. orderedData.Take(numberOfBusses)];
    }
}