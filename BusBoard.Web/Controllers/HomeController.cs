using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BusBoard.Web.ViewModels;
using BusBoard.Web.Models;
using BusBoard.API;
using BusBoard.Models;
using System.Collections.Immutable;
using BusBoard.Controllers;

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
                    returnView.Postcode = "No stops found near you";
                    return View(returnView);
                }
            }

            Console.WriteLine(Environment.NewLine + "Finding Next Bus Arrival Times...");

            // Task.WaitAll([.. stopPointSearch.StopPoints.Take(2).Select(stopPoint =>
            //     BusArrivalsController.PrintNextBusArrivalsInformation(stopPoint, tflAPI))]);

            returnView.Postcode = "busses found";
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
