using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;
using BusBoard.Controllers;

namespace BusBoard;

class Program
{
    static async Task Main(string[] args)
    {
        // Load User Secrtes
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();


        // TO BE REMOVED
        Console.WriteLine("Enter Stop Code:");
        string id = Console.ReadLine()!;

        UserInputController userInput = new();
        string postcode = userInput.GetPostcodeFromUser();

        // Build and Execute Request
        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        List<BusArrivalPrediction> nextBusses;

        try
        {
            PostcodeData postcodeData = await postcodeAPI.GetPostcodeData(postcode);
            Console.WriteLine($"Region: {postcodeData.Region}, Latitude: {postcodeData.Latitude}, Longitude: {postcodeData.Longitude}");
            return;
            ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(id, config);
            nextBusses = tflAPI.GetNextBusses(busArrivalPredictions);
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