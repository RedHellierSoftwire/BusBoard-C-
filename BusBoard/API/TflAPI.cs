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
    public TflAPI(RestRequestOptions? requestOptions = null)
    {
        Client = new RestClient(_options);

        if (requestOptions is not null)
        {
            CreateRequest(requestOptions);
        }
    }

    public void CreateRequest(RestRequestOptions requestOptions)
    {

        RestRequest request = new("StopPoint/{id}/Arrivals");

        if (requestOptions.UrlSegments is not null)
        {
            foreach (KeyValuePair<string, string> urlSegment in requestOptions.UrlSegments)
            {
                request.AddUrlSegment(urlSegment.Key, urlSegment.Value);
            }
        }

        if (requestOptions.Parameters is not null)
        {
            foreach (KeyValuePair<string, string> parameter in requestOptions.Parameters)
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }
        }

        Request = request;
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