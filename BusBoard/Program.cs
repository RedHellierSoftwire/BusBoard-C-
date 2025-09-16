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

        TflAPI tflAPI = new TflAPI();
        //RestRequestOptions requestOptions =
        //tflAPI.CreateRequest(RestRequestOptions { RequestRoute: "StopPoint/{id}/Arrivals"})

        var options = new RestClientOptions("https://api.tfl.gov.uk/StopPoint");

        var client = new RestClient(options);
        var request = new RestRequest("{id}/Arrivals")
            .AddUrlSegment("id", id)
            .AddParameter("app_key", config["BusBoard:TFLAPI_KEY"]);

        RestResponse response = await client.GetAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"ERROR: {response.ErrorException?.Message}");
            return;
        }

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        List<Prediction>? data = JsonSerializer.Deserialize<List<Prediction>>(response.Content!, serializerOptions);

        data?.Sort((predictionA, predictionB) => DateTime.Compare(predictionA.ExpectedArrival, predictionB.ExpectedArrival));

        for (var i = 0; i < 5; i++)
        {
            DateTime now = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            TimeSpan? timeUntilArrival = data?[i].ExpectedArrival.Subtract(now);
            Console.WriteLine($"{data?[i].LineName} - arrives in {timeUntilArrival?.Minutes} minu");
        }

    }
}