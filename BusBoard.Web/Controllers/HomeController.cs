using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BusBoard.Web.ViewModels;
using BusBoard.Web.Models;
using BusBoard.API;
using BusBoard.Models;
using System.Collections.Immutable;
using BusBoard.Controllers;
using Microsoft.VisualBasic;

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
        BusBoardViewModel returnView = new(postcode);

        TflAPIService tflAPI = new();
        PostcodeAPIService postcodeAPI = new();

        try
        {
            postcode = UserInputController.ValidatePostcodeFromUser(postcode);
            PostcodeData postcodeData = await postcodeAPI.GetPostcodeData(postcode);

            if (postcodeData.Region != "London")
            {
                returnView.ErrorMessage = "Please Enter a London Postcode";
                return View(returnView);
            }
            StopPointSearchResponse stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude);

            if (stopPointSearch.StopPoints.Count < 2)
            {
                stopPointSearch = await tflAPI.GetStopPointsNearLocation(postcodeData.Latitude, postcodeData.Longitude, true);

                if (stopPointSearch.StopPoints.Count == 1)
                {
                    returnView.ErrorMessage = "Only one stop found near you";
                }
                else if (stopPointSearch.StopPoints.Count == 0)
                {
                    returnView.ErrorMessage = "No stops found near you";
                    return View(returnView);
                }
            }

            List<BusBoardEntry> busBoardEntries = [];

            Task.WaitAll([.. stopPointSearch.StopPoints.Take(2).Select(async stopPoint =>
            {
                var busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(stopPoint.NaptanId);
                var nextBusses = BusArrivalsController.GetNextBusses(busArrivalPredictions);
                busBoardEntries.Add(new BusBoardEntry(stopPoint, nextBusses));

            })]);

            returnView.BusBoardEntries = busBoardEntries;
            return View(returnView);
        }
        catch (Exception error)
        {
            Debug.WriteLine(error.Message);
            returnView.ErrorMessage = error.Message;
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
