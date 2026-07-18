using System.ComponentModel.DataAnnotations;

namespace FoodSlot2.Models
{
    public class Store
    {
        [Key]
        public int storeID { get; set; }

        [MaxLength(500)]
        public string place_id { get; set; } = null!;

        [MaxLength(50)]
        public string storeName { get; set; }=null!;
        public double lng { get; set; }
        public double lat { get; set; }

        [MaxLength(500)]
        public string weekdayDescriptions { get; set; }=null !;

        public int price_level { get; set; } 

        public float rating { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public int foodID { get; set; }
        public virtual Food Food { get; set; } = null!;
        public virtual List<DrawHistory> DrawHistories { get; set; } = new List<DrawHistory>();
        public virtual List<PocketList> PocketLists { get; set; } = new List<PocketList>();
  


    }   
}
