using FoodSlot2.Services.Interfaces;
using FoodSlot2.Models;
using FoodSlot2.ViewModels.Slot;
using Microsoft.EntityFrameworkCore;

namespace FoodSlot.Services.SlotService
{
    public class DrawService : IDrawService
    {
        private readonly FoodSlotContext _context;

        public DrawService(FoodSlotContext context)
        {
            _context = context;
        }
        //獎池列表
        public async Task<List<VMFoodSlotItem>> GetFoodsPoolAsync(int? userID)
        {
            int actualUserID = userID ?? 1; // 有登入就用 userID，沒有就用 1 (預設會員)

            // 取得會員設定的主食 ID
            List<int> foodIDs = await _context.UserFoodSettings
                .Where(x => x.userID == actualUserID)
                .Select(x => x.foodID)
                .ToListAsync();

            // 防呆，判斷是否為子類別，並取得主食的其他資料
            List<Food> foods = await _context.Foods
                .Where(x => foodIDs.Contains(x.foodID) && x.parentfoodID != null)
                .ToListAsync();

            if (!foods.Any())
            {
                throw new Exception("無可抽取食物資料");
            }

            // 把每一筆 Food 轉成 VMFoodSlotItem 並回傳
            return foods.Select(ConvertToVM).ToList();
        }

        //抽籤
        public async Task<VMSlotDrawResult> DrawAsync(int? userID)
        {
            // 抽籤時，呼叫GetFoodsPoolAsync方法拿到該會員的食物清單！
            List<VMFoodSlotItem> foodsPool = await GetFoodsPoolAsync(userID);

            // 隨機抽出一個中獎項目 (改從 foodsPool 陣列裡抽)
            //foodsPool.Count意思是foodsPool[]總共有幾筆資料
            //Random.Shared.Next()隨機產生數字
            //範例：foodsPool.Count=4，Random.Shared.Next(4)，可能結果為0123
            VMFoodSlotItem selectedVM = foodsPool[Random.Shared.Next(foodsPool.Count)];

            // 建立拉霸輪盤資料
            // 1.建立空的List
            List<VMFoodSlotItem> reelItems = [];

            for (int i = 0; i < 29; i++)
            {
                //2-1.每次迴圈隨機取得一個使用者設定的主食
                VMFoodSlotItem randomFood = foodsPool[Random.Shared.Next(foodsPool.Count)];
                //2-2.加入到 reelItems 清單尾端
                reelItems.Add(randomFood);
            }

            // 3.最後一格固定中獎項目
            reelItems.Add(selectedVM);

            return new VMSlotDrawResult
            {
                selectedFood = selectedVM,//中獎項目
                reelItems = reelItems,//拉霸動畫的資料列表
                foodsPool = foodsPool //獎池列表
            };
        }
        
        //把 Food Model 轉成 VM
        private static VMFoodSlotItem ConvertToVM(Food food)
        {
            return new VMFoodSlotItem
            {
                foodID = food.foodID,
                foodname = food.foodname,
                photoUrl = $"/FoodImages/{food.photo}.webp"
            };
        }
    }
}