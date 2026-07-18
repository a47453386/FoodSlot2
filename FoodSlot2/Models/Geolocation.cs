using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSlot2.Models
{
    public class Geolocation
    {
        [Key]
        public int geolocationID { get; set; }
        [MaxLength(50)]
        public string geolocationName { get; set; } = null!;
        public double geolng { get; set; }
        public double geolat { get; set; }
        public int userID { get; set; }
        public virtual User User { get; set; } = null!;
    }
 }

