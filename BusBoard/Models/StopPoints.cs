using System.Collections.Immutable;

namespace BusBoard.Models;

public record StopPoint(string NaptanId);

public record StopPointSearchResponse(ImmutableList<StopPoint> StopPoints);