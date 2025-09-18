using BusBoard.Models;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Text.Json;
namespace BusBoard.API;

public class PostcodeAPIService
{
    // private static readonly JsonSerializerOptions _serializerOptions = new()
    // {
    //     PropertyNameCaseInsensitive = true
    // };

    private readonly APIService _apiService = new("https://api.postcodes.io");

    public async Task<PostcodeData> GetPostcodeData(string postcode)
    {
        RestRequest request = new RestRequest("postcodes/{postcode}")
            .AddUrlSegment("postcode", postcode);

        RestResponse response = await _apiService.Client.GetAsync(request);

        if (!response.IsSuccessful)
        {
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
            throw new Exception(error.Message);
        }

        if (data is null)
        {
            throw new Exception("Data could not be Deserialized");
        }

        return data;
    }
}