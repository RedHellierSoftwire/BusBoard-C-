using System.Collections.Immutable;
using BusBoard.Models;

namespace BusBoard.Web.Models;

public class BusBoardEntry
{
    public string NaptanId { get; set; }
    public string CommonName { get; set; }
    public string StopLetter { get; set; }
    public ImmutableList<BusArrivalPrediction> Arrivals { get; set; }

    public BusBoardEntry(string naptanId, string commonName, string stopLetter)
    {
        NaptanId = naptanId;
        CommonName = commonName;
        StopLetter = stopLetter;
        Arrivals = [];
    }

}
