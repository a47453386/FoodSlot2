using FoodSlot2.Models;
using FoodSlot2.Seed.Data;
using FoodSlot2.Services.Interface;
using FoodSlot2.Services.ImageUploadServices;

namespace FoodSlot2.Seed
{
    public class SeedRunner
    {
        private readonly FoodSlotContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IImageUploadService _imageUploadService;
        private readonly SeedSystemSetting _seedSystemSetting;
        private readonly SeedSystemSettingCategory _seedSystemSettingCategory;
        private readonly SeedFood _seedFood;
        private readonly SeedUsers _seedUsers;
        private readonly SeedUserRange _seedUserRange;
        private readonly SeedUserRangeSettings _seedUserRangeSettings;
        private readonly SeedUserFoodSettings _seedUserFoodSettings;

        private readonly Dictionary<string, List<ImageSizeOption>> _folderSizeMapping =
           new()
           {
                { "Food", ImageSizePresets.Food }
           };

        public SeedRunner(
            FoodSlotContext context,
            IWebHostEnvironment env,
            IImageUploadService imageUploadService,
            SeedSystemSetting seedSystemSetting,
            SeedSystemSettingCategory seedSystemSettingCategory,
            SeedFood seedFood,
            SeedUsers seedUsers,
            SeedUserRange seedUserRange,
            SeedUserRangeSettings seedUserRangeSettings,
            SeedUserFoodSettings seedUserFoodSettings)
        {
            _context = context;
            _env = env;
            _imageUploadService = imageUploadService;
            _seedSystemSetting = seedSystemSetting;
            _seedSystemSettingCategory = seedSystemSettingCategory;
            _seedFood = seedFood;           
            _seedUsers = seedUsers;
            _seedUserRange = seedUserRange;
            _seedUserRangeSettings = seedUserRangeSettings;
            _seedUserFoodSettings = seedUserFoodSettings;
        }

        public void Run()
        {
            if (!_context.Users.Any())
            {
                // 1️ 準備 GUID
                var foodImagesGuids = GenerateGuids(57);
                Console.WriteLine("準備 GUID 完成");
                // 2 上傳圖片
                UploadImages(foodImagesGuids);
                Console.WriteLine("上傳圖片 完成");
                // 3 更新Seed資料

                _seedUsers.Run();
                _seedFood.Run(foodImagesGuids);

                _seedSystemSettingCategory.Run();
                _seedSystemSetting.Run();
                _seedUserRange.Run();
                _seedUserFoodSettings.Run();
                _seedUserRangeSettings.Run();

                Console.WriteLine("更新Seed資料 完成");
            }

        }

        private string[] GenerateGuids(int count)
        {
            var guids = new string[count];
            for (int i = 0; i < count; i++)
                guids[i] = Guid.NewGuid().ToString();

            return guids;
        }

        private void UploadImages(string[] guids)
        {
            string seedPhotoPath = Path.Combine(
                _env.ContentRootPath,
                "Seed",
                "Photos"
            );
            var files = Directory.GetFiles(seedPhotoPath)
                        .OrderBy(f => f) // 很重要，確保順序穩定
                        .ToArray();
            if (files.Length < guids.Length)
                throw new Exception($"FoodImages圖片數量不足");

            // 取得對應尺寸設定
            var sizes = _folderSizeMapping.ContainsKey("Food")
                ? _folderSizeMapping["Food"]
                : ImageSizePresets.Food; // 預設使用 Food

            for (int i = 0; i < guids.Length; i++)
            {
                _imageUploadService.UploadFromSeed(
                    seedFolder: "FoodImages",
                    sourceFile: files[i],
                    fileNameWithoutExt: guids[i],
                    sizes: sizes
                );
            }

        }
    }
}
