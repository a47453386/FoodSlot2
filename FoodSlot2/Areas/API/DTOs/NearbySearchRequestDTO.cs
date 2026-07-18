namespace FoodSlot2.Areas.API.DTOs
{
    public class NearbySearchRequestDTO
    {
        public string Keyword { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Radius { get; set; } = 1500;
    }
}
