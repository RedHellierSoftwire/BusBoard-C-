using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BusBoard.Web.ViewModels;
using BusBoard.Web.Models;
using BusBoard.API;
using BusBoard.Models;
using System.Collections.Immutable;

namespace BusBoard.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> BusBoard(string postcode)
    {
        BusBoardViewModel returnView = new(postcode, Array.Empty<BusBoardEntry>());

        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        try
        {

            PostcodeData postcodeData = await postcodeAPI.GetPostcodeData(postcode);

            if (postcodeData.Region != "London")
            {
                Console.WriteLine("Please Enter a London Postcode");
                returnView.Postcode = "Please Enter a London Postcode";
                return View(returnView);
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
                    returnView.Postcode = "No stops found near you";
                    
                    return View(returnView);
                }
            }

            Console.WriteLine(Environment.NewLine + "Finding Next Bus Arrival Times...");

            for (int n = 0; n < 2; n++)
            {
                string id = stopPointSearch.StopPoints[n].NaptanId;
                string name = stopPointSearch.StopPoints[n].CommonName;
                string stopLetter = stopPointSearch.StopPoints[n].StopLetter;
                Console.WriteLine(Environment.NewLine + $"Stop {stopLetter}: {name}");

                ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(id);

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

            return View(returnView);
        }
        catch (Exception error)
        {
            Debug.WriteLine(error.Message);
            returnView.Postcode = error.Message;
            return View(returnView);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
