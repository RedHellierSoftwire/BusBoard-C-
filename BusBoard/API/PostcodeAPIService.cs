using BusBoard.Models;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
namespace BusBoard.API;

public class PostcodeAPIService
{
    private readonly RestClient _client = new("https://api.postcodes.io/");

    public async Task<PostcodeData> GetPostcodeData(string postcode)
    {
        RestRequest request = new RestRequest("postcodes/{postcode}")
            .AddUrlSegment("postcode", postcode);

        RestResponse response = await _client.GetAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Error: Postcode not found");
            }
            
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }

        PostcodeData? data;

        try
        {
            var parsedObject = JObject.Parse(response.Content!);
            string resultToken = parsedObject["result"]!.ToString();
            data = JsonConvert.DeserializeObject<PostcodeData>(resultToken);
        }
        catch (Exception error)
        {
            throw; // new Exception($"Error: Could not retrieve postcode data: {error.GetType} - {error.Message}");
        }

        if (data is null)
        {
            throw new Exception("Error: Could not retrieve postcode data - Data could not be Deserialized");
        }

        return data;
    }
}