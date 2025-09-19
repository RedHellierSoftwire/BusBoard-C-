using System.Collections.Immutable;
using System.Diagnostics;
using BusBoard.API;
using BusBoard.Models;
using Microsoft.Extensions.Configuration;

namespace BusBoard.Controllers;

public static class BusArrivalsController
{
    public static List<BusArrivalPrediction> GetNextBusses(ImmutableList<BusArrivalPrediction> predictions, int numberOfBusses = 5)
    {
        if (numberOfBusses <= 0)
        {
            throw new ArgumentException("Number of Busses must be greater than 0");
        }

        if (numberOfBusses > predictions.Count)
        {
            numberOfBusses = predictions.Count;
            Debug.WriteLine($"Only {predictions.Count} arrivals found, returning all of them.");
        }

        IOrderedEnumerable<BusArrivalPrediction> orderedData = predictions.OrderBy(prediction => prediction.ExpectedArrival);

        return [.. orderedData.Take(numberOfBusses)];
    }

    public static async Task PrintNextBusArrivalsInformation(StopPoint stopPoint, TflAPIService tflAPI)
    {
        ImmutableList<BusArrivalPrediction> busArrivalPredictions = await tflAPI.GetBusArrivalPredictionsForStop(stopPoint.NaptanId);

        Console.WriteLine(Environment.NewLine + $"Stop {stopPoint.StopLetter}: {stopPoint.CommonName}");

        if (busArrivalPredictions.Count == 0)
        {
            Console.WriteLine("No arrivals for this stop");
        }

        List<BusArrivalPrediction> nextBusses = GetNextBusses(busArrivalPredictions);

        nextBusses.ForEach(PrintBusArrivalInformation);
    }

    public static void PrintBusArrivalInformation(BusArrivalPrediction bus)
    {
        DateTime now = DateTime.UtcNow;
        int minutesAway = bus.ExpectedArrival.Subtract(now).Minutes;
        string displayString = GetBusArrivalDisplayString(bus.LineName, minutesAway);
        Console.WriteLine(displayString);
    }

    public static string GetBusArrivalDisplayString(string lineName, int minutesAway)
    {
        string displayString = lineName;
        if (minutesAway == 0)
        {
            displayString += " - due";
        }
        else
        {
            displayString += $" - {minutesAway} minute{(minutesAway == 1 ? "" : "s")}";
        }

        return displayString;
    }
}
