using FoodSlot2.ViewModels.Slot;

namespace FoodSlot2.Services.Interfaces
{
    public interface IDrawService
    {
        Task<VMSlotDrawResult> DrawAsync(int? userID);
        Task<List<VMFoodSlotItem>> GetFoodsPoolAsync(int? userID);
    }
}