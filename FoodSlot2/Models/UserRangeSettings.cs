using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class UserRangeSettings
    {
        public int userID { get; set; }
        public virtual User User { get; set; } = null!;
        public int rangeID { get; set; }
        public virtual UserRange UserRange { get; set; } = null!;
    }
}
