using FoodSlot2.Services.Interfaces;

using Newtonsoft.Json.Linq;
using FoodSlot2.Areas.API.DTOs;

namespace FoodSlot2.Services.APIResultService
{
    public class APIResultService : IAPIResultService
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        public APIResultService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<StoreDTO>> NearbySearchAsync(
            NearbySearchRequestDTO request)
        {
            List<StoreDTO> stores = new();

            string apiKey =
                _configuration["GoogleApi:ApiKey"]!;

            string url =
                $"https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                $"?location={request.Latitude},{request.Longitude}" +
                $"&radius={request.Radius}" +
                $"&keyword={request.Keyword}" +
                $"&language=zh-TW" +
                $"&key={apiKey}";

            HttpResponseMessage response =
                await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return stores;
            }

            string json =
                await response.Content.ReadAsStringAsync();

            JObject data = JObject.Parse(json);

            // Google API Status 檢查
            string status =
                data["status"]?.ToString()
                ?? "";

            if (status != "OK" && status != "ZERO_RESULTS")
            {
                Console.WriteLine($"Google API Error: {status}");

                Console.WriteLine(json);

                return stores;
            }

            JArray? results =
                data["results"] as JArray;

            if (results == null)
            {
                return stores;
            }

            foreach (var item in results)
            {
                // Place ID
                string placeId =
                    item["place_id"]?.ToString()
                    ?? "";

                // Photo Reference
                string photoReference =
                    item["photos"]?[0]?["photo_reference"]
                    ?.ToString()
                    ?? "";

                // Google Photo URL
                string photoUrl = "";
                //
                bool? openNow = null;
                var openingHours = item["opening_hours"];
                if (openingHours != null && openingHours["open_now"] != null)
                {
                    openNow = openingHours["open_now"]?.Value<bool>();
                }

                if (!string.IsNullOrEmpty(photoReference))
                {
                    photoUrl =
                        $"https://maps.googleapis.com/maps/api/place/photo" +
                        $"?maxwidth=400" +
                        $"&photo_reference={photoReference}" +
                        $"&key={apiKey}";
                }

                // Google Map URL
                string googleMapUrl =
                    $"https://www.google.com/maps/place/?q=place_id:{placeId}";

                stores.Add(new StoreDTO
                {
                    Name =
                        item["name"]?.ToString()
                        ?? "",

                    Address =
                        item["vicinity"]?.ToString()
                        ?? "",

                    Rating =
                        item["rating"]?.Value<double>()
                        ?? 0,

                    Latitude =
                        item["geometry"]?["location"]?["lat"]
                        ?.Value<double>()
                        ?? 0,

                    Longitude =
                        item["geometry"]?["location"]?["lng"]
                        ?.Value<double>()
                        ?? 0,

                    PlaceId = placeId,

                    PhotoUrl = photoUrl,

                    GoogleMapUrl = googleMapUrl,

                    OpenNow = openNow
                });
            }

            return stores;
        }
    }
}