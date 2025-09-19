using BusBoard.Models;
using BusBoard.API;
using Microsoft.Extensions.Configuration;
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
        
        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        try
        {
            string postcode = UserInputController.GetStringInputFromUser("Enter Postcode: ");
            postcode = UserInputController.ValidatePostcodeFromUser(postcode);

            PostcodeData postcodeData = await postcodeAPI.GetPostcodeData(postcode);

            if (postcodeData.Region != "London")
            {
                Console.WriteLine("Please Enter a London Postcode");
                return;
            }

            Console.WriteLine("Searching for nearby stops...");
            StopPointSearchResponse stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude);

            if (stopPointSearch.StopPoints.Count < 2)
            {
                Console.WriteLine("Searching for nearby stops...");
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
            
            Task.WaitAll([.. stopPointSearch.StopPoints.Take(2).Select(stopPoint => 
                BusArrivalsController.PrintNextBusArrivalsInformation(stopPoint, tflAPI, config))]);
        }
        catch (Exception error)
        {
            Debug.WriteLine(error.Message);
            return;
        }

    }
}