using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class Food
    {
        [Key]
        public int foodID { get; set; }

        [MaxLength(50)]
        public string foodname { get; set; } = null!;
        [MaxLength(36)]
        public string photo { get; set; } = null!;
        public DateTime createTime { get; set; } = DateTime.Now;
        public DateTime? updateTime { get; set; }

        public int? parentfoodID { get; set; }
        public virtual Food? Parentfood { get; set; }
        public virtual List<Food> Childrens { get; set; } = new List<Food>();
        public int userID { get; set; }
        public virtual User ManagerUser { get; set; } = null!;
        public virtual List<LotteryHistory> LotteryHistories { get; set; } = new List<LotteryHistory>();
        public virtual List<Store> Stores { get; set; } =new List<Store>();
        public virtual List<UserFoodSettings> UserFoodSettings { get; set; } = new List<UserFoodSettings>();
    }
}
