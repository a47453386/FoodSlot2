using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class User
    {
        [Key]
        public int userID { get; set; }
        [MaxLength(50)]
        public string username { get; set; } = null!;
        [MaxLength(255)]
        public string password { get; set; } = null!;
        [MaxLength(100)]
        public string email { get; set; } = null!;
        public bool isAdmin { get; set; } = false;
        public DateTime createTime { get; set; } = DateTime.Now;
        public DateTime lastLoginTime { get; set; } = DateTime.Now;

        public virtual List<Food> Foods { get; set; } = new List<Food>();
        public virtual List<UserRange> UserRanges { get; set; } = new List<UserRange>();
        public virtual List<Verification> Verifications { get; set; } = new List<Verification>();
        public virtual List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual List<PocketList> PocketLists { get; set; } = new List<PocketList>();
        public virtual List<DrawHistory> DrawHistories { get; set; } = new List<DrawHistory>();
        public virtual List<Geolocation> Geolocations { get; set; } = new List<Geolocation>();
        public virtual List<LotteryHistory> LotteryHistories { get; set; } = new List<LotteryHistory>();
        public virtual List<UserFoodSettings> UserFoodSettings { get; set; } = new List<UserFoodSettings>();
        public virtual List<UserRangeSettings> UserRangeSettings { get; set; } = new List<UserRangeSettings>();
        public virtual List<SystemSetting> SystemSettings { get; set; } = new List<SystemSetting>();
        public virtual List<SystemSettingCategory> SystemSettingCategories { get; set; } = new List<SystemSettingCategory>();
    }
}
