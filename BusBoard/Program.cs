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
using System.Collections;

namespace BusBoard;

class Program
{
    static async Task Main(string[] args)
    {
        var options = new RestClientOptions("https://api.tfl.gov.uk/StopPoint");

        var client = new RestClient(options);
        var request = new RestRequest("{id}/Arrivals")
            .AddUrlSegment("id", "490008660N")
            .AddParameter("app_key", "9335b5c020994e4b867efbed7ca4a091");

        var response = await client.GetAsync(request);

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
            Console.WriteLine(data?[i].LineName);
            DateTime now = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            TimeSpan? timeUntilArrival = data?[i].ExpectedArrival.Subtract(now);
            Console.WriteLine(timeUntilArrival?.Minutes);
        }

    }
}