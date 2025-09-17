using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;

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

        List<BusArrivalPrediction> nextBusses;

        try
        {
            ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(id, config);
            nextBusses = tflAPI.GetNextBussesAtStop(busArrivalPredictions);
        }
        catch (Exception error)
        {
            Console.WriteLine($"Error: {error.Message}");
            return;
        }

        // Print first 5 soonest buses
        nextBusses.ForEach(bus => 
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeUntilArrival = bus.ExpectedArrival.Subtract(now);
            Console.WriteLine($"{bus.LineName} - arrives in {timeUntilArrival.Minutes} minutes");
        });

    }
}