using FoodSlot2.Models;

namespace FoodSlot2.Seed.Data
{
    public class SeedSystemSettingCategory
    {
        private readonly FoodSlotContext _context;

        public SeedSystemSettingCategory(FoodSlotContext context)
        {
            _context = context;
        }
        public void Run()
        {
            if (!_context.SystemSettingCategories.Any()) // 避免重複 Seed
            {
                var systemSettingCategories = new List<SystemSettingCategory>
                {
                    new SystemSettingCategory
                    {
                        categoryName = "Google Maps API設定",
                        userID = 1
                    },
                    new SystemSettingCategory
                    {
                        categoryName = "會員與帳號相關",
                        userID = 1
                    },
                    new SystemSettingCategory
                    {
                        categoryName = "管理者監控設定",
                        userID = 1
                    },

                };
                _context.SystemSettingCategories.AddRange(systemSettingCategories);
                _context.SaveChanges();
            }
        }
    }
}
