using FoodSlot2.Models;

namespace FoodSlot2.Seed.Data
{
    public class SeedUserFoodSettings
    {
        private readonly FoodSlotContext _context;

        public SeedUserFoodSettings(FoodSlotContext context)
        {
            _context = context;
        }
        public void Run()
        {
            if (!_context.UserFoodSettings.Any()) // 避免重複 Seed
            {
                var userFoodSettings = new List<UserFoodSettings>();

                for (int userId = 1; userId <= 2; userId++)
                {
                    for (int foodId = 2; foodId <= 11; foodId++)
                    {
                        userFoodSettings.Add(new UserFoodSettings
                        {
                            userID = userId,
                            foodID = foodId
                        });
                    }
                }

                _context.UserFoodSettings.AddRange(userFoodSettings);
                _context.SaveChanges();
            }
        }
    }
}
