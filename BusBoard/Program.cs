using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RestSharp;
using BusBoard.Models;
using BusBoard.API;
using System.Collections;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BusBoard;

class Program
{
    static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Console.WriteLine("Enter Stop Code:");
        string id = Console.ReadLine()!;

        RestRequest request = new RestRequest("StopPoint/{id}/Arrivals")
            .AddUrlSegment("id", id)
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        TflAPI tflAPI = new(request);

        RestResponse response = await tflAPI.ExecuteGet();

        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"ERROR: {response.ErrorException?.Message} [{response.StatusCode}]");
            return;
        }

        JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<BusArrivalPrediction>? data = JsonSerializer.Deserialize<List<BusArrivalPrediction>>(response.Content!, serializerOptions);

        if (data is null)
        {
            Debug.WriteLine("ERROR: Data could not be Deserialized");
            return;
        }

        data?.Sort((predictionA, predictionB) => DateTime.Compare(predictionA.ExpectedArrival, predictionB.ExpectedArrival));

        for (var i = 0; i < 5; i++)
        {
            DateTime now = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            TimeSpan? timeUntilArrival = data?[i].ExpectedArrival.Subtract(now);
            Console.WriteLine($"{data?[i].LineName} - arrives in {timeUntilArrival?.Minutes} minu");
        }

    }
}