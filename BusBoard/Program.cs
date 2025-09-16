using System.Text.Json;
using RestSharp;
using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BusBoard;

class Program
{
    
    private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    
    static async Task Main(string[] args)
    {
        // Load User Secrtes
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Console.WriteLine("Enter Stop Code:");
        string id = Console.ReadLine()!;

        // Build and Execute Request
        RestRequest request = new RestRequest("StopPoint/{id}/placeTypes")
            .AddUrlSegment("id", id)
            .AddParameter("placeTypes", "CarPark")
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        TflAPIService tflAPI = new(request);

        RestResponse response = await tflAPI.ExecuteGet();

        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"ERROR: {response.ErrorException?.Message}");
            return;
        }

        // Deserialize Data
        List<BusArrivalPrediction>? data = null;

        try
        {
            data = JsonSerializer.Deserialize<List<BusArrivalPrediction>>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            Debug.WriteLine($"ERROR: {error.Message}");
        }

        if (data is null | data!.Count == 0)
        {
            Debug.WriteLine("ERROR: Data could not be Deserialized");
            return;
        }

        data.Sort((predictionA, predictionB) => DateTime.Compare(predictionA.ExpectedArrival, predictionB.ExpectedArrival));

        // Print first 5 soonest buses
        for (var i = 0; i < 5; i++)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan? timeUntilArrival = data?[i].ExpectedArrival.Subtract(now);
            Console.WriteLine($"{data?[i].LineName} - arrives in {timeUntilArrival?.Minutes} minutes");
        }

    }
}