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

        //UserInputController userInput = new();
        //string postcode = userInput.GetPostcodeFromUser();

        // Build and Execute Request
        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        List<BusArrivalPrediction> nextBusses;

        PostcodeData postcodeData;

        try
        {
            postcodeData = await postcodeAPI.GetPostcodeData("N88NS");
            Console.WriteLine($"Region: {postcodeData.Region}, Latitude: {postcodeData.Latitude}, Longitude: {postcodeData.Longitude}");

            if (postcodeData.Region != "London")
            {
                Console.WriteLine("Please Enter a London Postcode");
                return;
            }

            Console.WriteLine("Searching...");
            StopPointSearchResponse stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude);

            if (stopPointSearch.StopPoints.Count < 2)
            {
                Console.WriteLine("Searching...");
                stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude, true);

                if (stopPointSearch.StopPoints.Count == 1)
                {
                    Console.WriteLine("Only one stop found near you");
                }
                else if (stopPointSearch.StopPoints.Count == 0)
                {
                    Console.WriteLine("No stops found near you");
                    return;
                }
            }
            //ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(id, config);
            //nextBusses = tflAPI.GetNextBusses(busArrivalPredictions);
        }
        catch (Exception error)
        {
            Console.WriteLine($"Error: {error.Message}");
            return;
        }

        try
        {
            StopPointSearchResponse stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude);
            Console.WriteLine(stopPointSearch.StopPoints.Count);
            return;
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