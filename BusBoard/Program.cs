using System.Text.Json;
using RestSharp;
using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BusBoard;

class Program
{
    static async Task Main(string[] args)
    {
        // Load User Secrtes
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Console.WriteLine("Enter Stop Code:");
        string id = Console.ReadLine()!;

        // Build and Execute Request
        TflAPIService tflAPI = new();

        List<BusArrivalPrediction> nextFiveBusses = await tflAPI.GetNextNBussesAtStop(id, 12, config);

        // Print first 5 soonest buses
        nextFiveBusses.ForEach(bus => 
            {
                DateTime now = DateTime.UtcNow;
                TimeSpan timeUntilArrival = bus.ExpectedArrival.Subtract(now);
                Console.WriteLine($"{bus.LineName} - arrives in {timeUntilArrival.Minutes} minutes");
            });

    }
}