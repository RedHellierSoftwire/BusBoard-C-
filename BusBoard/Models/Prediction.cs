namespace BusBoard.Models;

public record Prediction
{
    public required string LineName { get; set; }
    public required DateTime ExpectedArrival { get; set; }
}

public class RestRequestOptions
{
    public required string RequestRoute { get; set; }
    public Dictionary<string, string>? UrlSegments { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}