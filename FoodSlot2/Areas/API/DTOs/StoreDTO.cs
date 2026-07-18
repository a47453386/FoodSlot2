namespace FoodSlot2.Areas.API.DTOs
{
    public class StoreDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public double Rating { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PlaceId { get; set; } = "";

        public string PhotoUrl { get; set; } = "";

        public string GoogleMapUrl { get; set; } = "";

        public bool? OpenNow { get; set; }
    }
}
