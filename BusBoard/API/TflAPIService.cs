using System.Collections.Immutable;
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

    private readonly RestClient _client = new("https://api.tfl.gov.uk");

    public async Task<ImmutableList<BusArrivalPrediction>> GetBusArrivalPredictionsForStop(string stopId)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        
        RestRequest request = new RestRequest("StopPoint/{id}/Arrivals")
            .AddUrlSegment("id", stopId)
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        RestResponse response = await _client.GetAsync(request);

        ImmutableList<BusArrivalPrediction>? data = null;

        try
        {
            data = JsonSerializer.Deserialize<ImmutableList<BusArrivalPrediction>>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            throw new Exception($"Error: Could not retrieve Arrival data: {error.GetType} - {error.Message}");
        }

        if (data is null)
        {
            throw new Exception("Error: Could not retrieve Arrival data - Data could not be Deserialized");
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

        RestResponse response = await _client.GetAsync(request);

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
            throw new Exception($"Error: Could not retrieve Stop data: {error.GetType} - {error.Message}");
        }

        if (data is null)
        {
            throw new Exception("Error: Could not retrieve Stop data - Data could not be Deserialized");
        }

        return data;
    }
}