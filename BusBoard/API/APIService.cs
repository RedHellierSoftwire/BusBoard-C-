using RestSharp;

namespace BusBoard.API;

public class APIService
{
    public RestClient Client { get; }
    public APIService(string baseUrl)
    {
        RestClientOptions options = new(baseUrl);
        Client = new RestClient(options);
    }
}