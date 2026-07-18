using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class Verification
    {
        [Key]
        public int verificationID { get; set; }
        public int code { get; set; }
        public DateTime createTime { get; set; } = DateTime.Now;
        public int userID { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
