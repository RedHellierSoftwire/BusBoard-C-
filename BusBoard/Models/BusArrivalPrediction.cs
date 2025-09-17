namespace BusBoard.Models;

public record BusArrivalPrediction
{
    public required string LineName { get; set; }
    public required DateTime ExpectedArrival { get; set; }
}