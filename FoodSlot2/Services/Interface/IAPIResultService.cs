using FoodSlot2.Areas.API.DTOs;

namespace FoodSlot2.Services.Interfaces
{
    public interface IAPIResultService
    {
        Task<List<StoreDTO>> NearbySearchAsync(
            NearbySearchRequestDTO request);
    }
}