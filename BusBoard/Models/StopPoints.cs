using System.Collections.Immutable;

namespace BusBoard.Models;

public record StopPoint(string NaptanId, string CommonName, string StopLetter);

public record StopPointSearchResponse(ImmutableList<StopPoint> StopPoints);