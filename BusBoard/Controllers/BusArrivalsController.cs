using System;
using System.Collections.Immutable;
using System.Diagnostics;
using BusBoard.Models;

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
}
