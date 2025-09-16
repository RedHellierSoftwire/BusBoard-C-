using RestSharp;

namespace BusBoard.API;

public class TflAPIService
{
    private readonly RestClientOptions _options = new("https://api.tfl.gov.uk");

    public RestClient Client { get; }
    public RestRequest? Request { get; set; }
    public TflAPIService(RestRequest? request = null)
    {
        Client = new RestClient(_options);

        if (request is not null)
        {
            Request = request;
        }
    }

    public async Task<RestResponse> ExecuteGet()
    {
        return Request is not null
            ? await Client.GetAsync(Request) 
            : throw new Exception("API Request has not been defined");
    }
}