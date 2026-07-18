namespace FoodSlot2.ViewModels.Slot
{
    public class VMSlotDrawResult
    {
        public VMFoodSlotItem selectedFood { get; set; } = null!;

        public List<VMFoodSlotItem> reelItems { get; set; } = [];

        public List<VMFoodSlotItem> foodsPool { get; set; } = [];
    }
}
