using System.Text.Json;
using BusBoard.Models;
using RestSharp;

namespace BusBoard.API;

public class PostcodeAPIService
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    
    private readonly RestClientOptions _options = new("https://api.postcodes.io");

    public RestClient Client { get; }
    public PostcodeAPIService()
    {
        Client = new RestClient(_options);
    }

    public async Task<PostcodeLatAndLon> GetLatAndLonFromPostcode(string postcode)
    {
        RestRequest request = new RestRequest("postcodes/{postcode}")
            .AddUrlSegment("postcode", postcode);

        RestResponse response = await Client.GetAsync(request);

        PostcodeLatAndLon? data;

        try
        {
            data = JsonSerializer.Deserialize<PostcodeLatAndLon>(response.Content!, _serializerOptions);
        }
        catch (Exception error)
        {
            throw new Exception(error.Message);
        }

        if (data is null)
        {
            throw new Exception("Data could not be Deserialized");
        }

        return data;
    }
}