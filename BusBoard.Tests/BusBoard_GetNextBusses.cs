using Xunit;
using BusBoard.API;
using BusBoard.Models;
using System.Collections.Immutable;

namespace BusBoard.Tests;

public class BusBoard_AreSoonestPredictionsReturned
{
    private readonly TflAPIService _tflAPI = new();

    private readonly List<BusArrivalPrediction> _listOfPredictions =
    [
        new BusArrivalPrediction("10 mins away", DateTime.UtcNow.AddMinutes(10)),
        new BusArrivalPrediction("5 mins away", DateTime.UtcNow.AddMinutes(5)),
        new BusArrivalPrediction("15 mins away", DateTime.UtcNow.AddMinutes(15)),
        new BusArrivalPrediction("3 mins away", DateTime.UtcNow.AddMinutes(3)),
        new BusArrivalPrediction("8 mins away", DateTime.UtcNow.AddMinutes(8)),
        new BusArrivalPrediction("20 mins away", DateTime.UtcNow.AddMinutes(20))
    ];

    [Fact]
    public void SoonestPredictions_InputListOfLength6_ReturnOrderedListOfLength5()
    {
        // Act
        var result = _tflAPI.GetNextBusses(_listOfPredictions.ToImmutableList());

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("3 mins away", result[0].LineName); // Soonest bus
        Assert.Equal("5 mins away", result[1].LineName);
        Assert.Equal("8 mins away", result[2].LineName);
        Assert.Equal("10 mins away", result[3].LineName);
        Assert.Equal("15 mins away", result[4].LineName); // Latest bus
    }

    [Fact]
    public void SoonestPredictions_InputTooLargeNumberOfBusses_ReturnOrderedFullList()
    {
        // Arrange
        int numberOfBusses = 100000;

        // Act
        var result = _tflAPI.GetNextBusses(_listOfPredictions.ToImmutableList(), numberOfBusses);

        // Assert
        Assert.Equal(_listOfPredictions.Count, result.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void SoonestPredictions_InputZeroOrNegativeNumberOfBusses_ThrowsArgumentOutOfRange(int numberOfBusses)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _tflAPI.GetNextBusses(_listOfPredictions.ToImmutableList(), numberOfBusses));
    }
}
