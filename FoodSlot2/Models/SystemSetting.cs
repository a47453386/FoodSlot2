using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class SystemSetting
    {
        [Key]
        public int systemSetingID { get; set; }

        [MaxLength(100)]
        public string settingName { get; set; } = null!;

        [MaxLength(100)]
        public string settingValue { get; set; } = null!;

        [MaxLength(20)]
        public string valueType { get; set; } = null!;

        [MaxLength(200)]
        public string? description { get; set; }
                
    

        public int userID { get; set; }
        public virtual User User { get; set; } = null!;

        public int categoryID { get; set; }
        public virtual SystemSettingCategory SystemSettingCategory { get; set; } = null!;


    }
}
