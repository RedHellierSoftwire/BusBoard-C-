using System.Collections.Immutable;

namespace BusBoard.Models;

public record StopPoint(string NaptanId, string CommonName);

public record StopPointSearchResponse(ImmutableList<StopPoint> StopPoints);