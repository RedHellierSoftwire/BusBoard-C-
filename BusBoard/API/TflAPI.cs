using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using BusBoard.Models;

using RestSharp;

namespace BusBoard.API;

public class TflAPI
{
    private readonly RestClientOptions _options = new("https://api.tfl.gov.uk");

    public RestClient Client { get; }
    public RestRequest? Request { get; set; }
    public TflAPI(RestRequest? request = null)
    {
        Client = new RestClient(_options);

        if (request is not null)
        {
            Request = request;
        }
    }

    public async Task<RestResponse> ExecuteGet()
    {
        if (Request is not null)
        {
            return await Client.GetAsync(Request);
        }
        else
        {
            throw new Exception("API Request has not been defined, please define using TflAPI.CreateRequest(RestRequestOptions requestOptions)");
        }
    }
}