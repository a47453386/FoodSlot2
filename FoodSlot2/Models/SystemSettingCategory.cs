using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSlot2.Models
{
    public class SystemSettingCategory
    {
        [Key]
        public int categoryID { get; set; }

        
        [MaxLength(100)]
        public string categoryName { get; set; }=null!;

        

        public int userID { get; set; }
        public virtual User User { get; set; } = null!;


        public virtual List<SystemSetting> SystemSettings { get; set; } = new List<SystemSetting>();

    }
}
