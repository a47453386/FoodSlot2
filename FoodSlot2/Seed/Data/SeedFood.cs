using FoodSlot2.Models;

namespace FoodSlot2.Seed.Data
{
    public class SeedFood
    {
        private readonly FoodSlotContext _context;

        public SeedFood(FoodSlotContext context)
        {
            _context = context;
        }

        public void Run(string[] imageGuids)
        {
            if (!_context.Foods.Any()) // 避免重複 Seed
            {
                var now = DateTime.Now;

                // 1. 用 Dictionary 建立清晰的「主分類 -> 子分類」對應表
                // Key = 主分類名稱, Value = 該主分類下的子分類名稱清單
                var foodHierarchy = new Dictionary<string, List<string>>
                {
                    { "飯食主義", new List<string> { "白飯", "便當", "健康餐盒", "丼飯", "炒飯", "燴飯", "滷肉飯", "雞肉飯", "粥", "壽司", "咖哩" } },
                    { "麵麵俱到", new List<string> { "陽春麵", "牛肉麵", "炒麵", "拉麵", "烏龍麵", "涼麵", "鍋燒麵", "義大利麵" } },
                    { "肉食/鍋物", new List<string> { "牛排", "生魚片", "熱炒", "海產", "魚料理", "火鍋", "燒烤", "炸雞", "鐵板燒" } },
                    { "中西麵點", new List<string> { "漢堡", "三明治", "披薩", "麵包", "包子", "饅頭", "水餃", "煎餃", "鍋貼", "水煎包", "小籠包" } },
                    { "特色小吃", new List<string> { "關東煮", "臭豆腐", "蚵仔煎", "鹹酥雞", "肉圓", "碗粿", "大腸包小腸", "滷味", "刈包" } },
                    { "異國風味", new List<string> { "日式料理", "韓式料理", "美式料理", "泰式料理", "越式料理", "印度料理", "港式料理", "義式料理", "墨西哥料理" } }
                };
                int imageIndex = 0;
                // 2. 迴圈處理階層寫入
                foreach (var item in foodHierarchy)
                {
                    // 先建立主分類
                    var parentCategory = new Food
                    {
                        foodname = item.Key,
                        photo = "",
                        createTime = now,
                        userID = 1
                    };

                    _context.Foods.Add(parentCategory);

                    // 必須先 SaveChanges()，資料庫才會配發 ID 給這筆主分類
                    _context.SaveChanges();

                    // 建立該主分類底下的所有子分類，並動態綁定剛剛生成的 ID
                    var childCategories = item.Value.Select(childName => new Food
                    {
                        foodname = childName,
                        photo = imageGuids[imageIndex++ % imageGuids.Length],
                        parentfoodID = parentCategory.foodID,
                        createTime = now,
                        userID = 1
                    }).ToList();

                    _context.Foods.AddRange(childCategories);
                }

                // 儲存所有子分類
                _context.SaveChanges();
            }
        }
    }
}
