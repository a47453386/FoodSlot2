using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class DrawHistory
    {
        [Key]
        public int drawHistoryID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public int userID { get; set; }
        public virtual User User { get; set; } = null!;
        public int storeID { get; set; }
        public virtual Store Store { get; set; } = null!;
    }
}
