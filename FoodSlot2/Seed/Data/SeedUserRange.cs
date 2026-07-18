using FoodSlot2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSlot2.Seed.Data
{
    public class SeedUserRange
    {
        private readonly FoodSlotContext _context;

        public SeedUserRange(FoodSlotContext context)
        {
            _context = context;
        }
        public void Run()
        {
            if (_context.UserRanges.Any()) return; 

            _context.UserRanges.AddRange(
               new UserRange
               {
                   radius = 500,
                   createTime = DateTime.Now,
                   updateTime = null,
                   userID = 1
               },
                    new UserRange
                    {
                        radius = 1000,
                        createTime = DateTime.Now,
                        updateTime = null,
                        userID=1
                    },
                    new UserRange
                    {                        
                        radius = 3000,
                        createTime = DateTime.Now,
                        updateTime = null,
                        userID = 1
                    }
            );
            _context.SaveChanges();
        }

    }
}
