using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class UserFoodSettings
    {
        public int userID { get; set; }
        public virtual User User { get; set; } = null!;
        public int foodID { get; set; }
        public virtual Food Food { get; set; } = null!;
    }
}
