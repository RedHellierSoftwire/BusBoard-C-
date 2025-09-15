using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using BusBoard.Models;

using RestSharp;

namespace BusBoard
{
  class Program
  {
        static async Task Main(string[] args)
        {
            var options = new RestClientOptions("https://api.tfl.gov.uk/StopPoint");

            var client = new RestClient(options);
            var request = new RestRequest("{id}/Arrivals")
                .AddUrlSegment("id", "490008660N")
                .AddParameter("app_key", "9335b5c020994e4b867efbed7ca4a091");

            var response = await client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<JsonArray>(response.Content!);
                Console.WriteLine(data?[0]);
            }

        }
  }
}