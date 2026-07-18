using FoodSlot2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSlot2.Seed.Data
{
    public class SeedUserRangeSettings
    {
        private readonly FoodSlotContext _context;

        public SeedUserRangeSettings(FoodSlotContext context)
        {
            _context = context;
        }

        public void Run()
        {
            if (_context.UserRangeSettings.Any()) return;

            _context.UserRangeSettings.AddRange(
               new UserRangeSettings
               {
                   userID = 1,
                   rangeID = 1
               },
               new UserRangeSettings
               {
                   userID = 2,
                   rangeID = 1
               }
                    
            );
            _context.SaveChanges();
        }


    }
}
