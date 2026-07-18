using FoodSlot2.Models;

namespace FoodSlot2.Seed.Data
{
    public class SeedSystemSetting
    {
        private readonly FoodSlotContext _context;

        public SeedSystemSetting(FoodSlotContext context)
        {
            _context = context;
        }
        public void Run()
        {
            if (!_context.SystemSettings.Any()) // 避免重複 Seed
            {
                var systemSettings = new List<SystemSetting>
                {
                    new SystemSetting
                    {
                        settingName = "API 每日使用警戒值",
                        settingValue = "100",
                        valueType = "int",
                        description = "",
                        userID = 1,
                        categoryID = 1
                    },
                    new SystemSetting
                    {
                        settingName = "抽籤歷史紀錄上限",
                        settingValue = "30",
                        valueType = "int",
                        description = "",
                        userID = 1,
                        categoryID = 2
                    },new SystemSetting
                    {
                        settingName = "熱門排行統計期間",
                        settingValue = "7",
                        valueType = "int",
                        description = "",
                        userID = 1,
                        categoryID = 3
                    },new SystemSetting
                    {
                        settingName = "儀表板自動刷新秒數",
                        settingValue = "5000",
                        valueType = "int",
                        description = "",
                        userID = 1,
                        categoryID = 3
                    }

                };
                _context.SystemSettings.AddRange(systemSettings);
                _context.SaveChanges();
            }
        }
    }
}
