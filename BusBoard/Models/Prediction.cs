namespace BusBoard.Models;

public class Prediction
{
    public required string LineName { get; set; }
    public required DateTime ExpectedArrival { get; set; }
}