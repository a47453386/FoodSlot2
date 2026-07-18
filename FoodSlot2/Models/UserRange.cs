using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class UserRange
    {
        [Key]
        public int rangeID { get; set; }
        public int radius { get; set; }
        public DateTime createTime { get; set; } = DateTime.Now;
        public DateTime? updateTime { get; set; }

        public int userID { get; set; }
        public virtual User ManagerUser { get; set; } = null!;
        public virtual List<UserRangeSettings> UserRangeSettings { get; set; } = new List<UserRangeSettings>();
    }
}
