using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;
using BusBoard.Controllers;
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

        
        UserInputController userInput = new();
        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        try
        {
            string postcode = userInput.GetPostcodeFromUser();

            PostcodeData postcodeData = await postcodeAPI.GetPostcodeData(postcode);

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

            Console.WriteLine(Environment.NewLine + "Finding Next Bus Arrival Times...");

            for (int n = 0; n < 2; n++)
            {
                string id = stopPointSearch.StopPoints[n].NaptanId;
                string name = stopPointSearch.StopPoints[n].CommonName;
                string stopLetter = stopPointSearch.StopPoints[n].StopLetter;
                Console.WriteLine(Environment.NewLine + $"Stop {stopLetter}: {name}");

                ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(id, config);

                if (busArrivalPredictions.Count == 0)
                {
                    Console.WriteLine("No arrivals for this stop");
                    continue;
                }
                List<BusArrivalPrediction> nextBusses = tflAPI.GetNextBusses(busArrivalPredictions);
                

                // Print first 5 soonest buses
                nextBusses.ForEach(bus =>
                {
                    DateTime now = DateTime.UtcNow;
                    int minutesAway = bus.ExpectedArrival.Subtract(now).Minutes;
                    string displayString = bus.LineName;
                    if (minutesAway == 0)
                    {
                        displayString += " - due";
                    }
                    else
                    {
                        displayString += $" - {minutesAway} minute{(minutesAway == 1 ? "" : "s")}";
                    }
                    Console.WriteLine(displayString);
                });
            }
        }
        catch (Exception error)
        {
            Debug.WriteLine(error.Message);
            return;
        }

    }
}