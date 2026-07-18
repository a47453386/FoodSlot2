using FoodSlot2.Models;

namespace FoodSlot2.Seed.Data
{
    public class SeedUsers
    {
        private readonly FoodSlotContext _context;

        public SeedUsers(FoodSlotContext context)
        {
            _context = context;
        }

        public void Run()
        {
            if (_context.Users.Any()) return;

            _context.Users.AddRange(
               new User
               {
                   username = "管理者1號",
                   email = "a47453386@gmail.com",
                   password = "0000",
                   isAdmin = true,
                   createTime = DateTime.Now,
                   lastLoginTime = DateTime.Now,
               },
               new User
               {
                   username = "管理者2號",
                   email= "bread1206@gmail.com",
                   password = "0000",
                   isAdmin = true,
                   createTime= DateTime.Now,
                   lastLoginTime= DateTime.Now,
               }
                   
            );
            _context.SaveChanges();
        }
    }
}
