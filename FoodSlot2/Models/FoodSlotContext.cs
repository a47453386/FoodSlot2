using Microsoft.EntityFrameworkCore;
using System;

namespace FoodSlot2.Models
{
    public class FoodSlotContext : DbContext
    {

        public FoodSlotContext(DbContextOptions<FoodSlotContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<UserRange> UserRanges { get; set; }
        public DbSet<Verification> Verifications { get; set; }

        public DbSet<DrawHistory> DrawHistories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<PocketList> PocketLists { get; set; }
        public DbSet<Store> Stores { get; set; }        

        public  DbSet<Geolocation> Geolocations { get; set; }
        public  DbSet<LotteryHistory> LotteryHistories { get; set; }
        public  DbSet<UserFoodSettings> UserFoodSettings { get; set; }
        public  DbSet<UserRangeSettings> UserRangeSettings { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<SystemSettingCategory> SystemSettingCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Food 設定            
            modelBuilder.Entity<Food>(entity =>
            {
                entity.ToTable("Foods");

                // Food -> User
                entity.HasOne(f => f.ManagerUser)
                    .WithMany(u => u.Foods)
                    .HasForeignKey(f => f.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Food 自我關聯
                entity.HasOne(f => f.Parentfood)
                    .WithMany(f => f.Childrens)
                    .HasForeignKey(f => f.parentfoodID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // UserRange 設定            
            modelBuilder.Entity<UserRange>(entity =>
            {
                entity.ToTable("UserRanges");

                entity.HasOne(r => r.ManagerUser)
                    .WithMany(u => u.UserRanges)
                    .HasForeignKey(r => r.userID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

           
            // Verification 設定            
            modelBuilder.Entity<Verification>(entity =>
            {
                entity.ToTable("Verifications");

                entity.HasOne(v => v.User)
                    .WithMany(u => u.Verifications)
                    .HasForeignKey(v => v.userID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // DrawHistory 設定            
            modelBuilder.Entity<DrawHistory>(entity =>
            {
                entity.ToTable("DrawHistories");

                entity.HasOne(d => d.User)
                    .WithMany(u => u.DrawHistories)
                    .HasForeignKey(d => d.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Store)
                    .WithMany(s => s.DrawHistories)
                    .HasForeignKey(d => d.storeID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // PocketList 設定            
            modelBuilder.Entity<PocketList>(entity =>
            {
                entity.ToTable("PocketLists");

                entity.HasOne(p => p.User)
                    .WithMany(u => u.PocketLists)
                    .HasForeignKey(p => p.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Store)
                    .WithMany(s => s.PocketLists)
                    .HasForeignKey(p => p.storeID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

           
            // Feedback 設定           
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedbacks");

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Feedbacks)
                    .HasForeignKey(f => f.userID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // Store 設定           
            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Stores");

                entity.HasOne(s => s.Food)
                    .WithMany(f => f.Stores)
                    .HasForeignKey(s => s.foodID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // SystemSettingCategory 設定          
            modelBuilder.Entity<SystemSettingCategory>(entity =>
            {
                entity.ToTable("SystemSettingCategories");

                entity.HasOne(c => c.User)
                    .WithMany(u => u.SystemSettingCategories)
                    .HasForeignKey(c => c.userID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

           
            // SystemSetting 設定            
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.ToTable("SystemSettings");

                entity.HasOne(s => s.User)
                    .WithMany(u => u.SystemSettings)
                    .HasForeignKey(s => s.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.SystemSettingCategory)
                    .WithMany(c => c.SystemSettings)
                    .HasForeignKey(s => s.categoryID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // Geolocation 設定           
            modelBuilder.Entity<Geolocation>(entity =>
            {
                entity.ToTable("Geolocations");

                entity.HasOne(g => g.User)
                    .WithMany(u => u.Geolocations)
                    .HasForeignKey(g => g.userID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // LotteryHistory 設定           
            modelBuilder.Entity<LotteryHistory>(entity =>
            {
                entity.ToTable("LotteryHistories");

                entity.HasOne(l => l.User)
                    .WithMany(u => u.LotteryHistories)
                    .HasForeignKey(l => l.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Food)
                    .WithMany(f => f.LotteryHistories)
                    .HasForeignKey(l => l.foodID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

           
            // UserFoodSettings 設定            
            modelBuilder.Entity<UserFoodSettings>(entity =>
            {
                entity.ToTable("UserFoodSettings");

                // 複合主鍵
                entity.HasKey(e => new { e.userID, e.foodID });

                // User 關聯
                entity.HasOne(l => l.User)
                    .WithMany(u => u.UserFoodSettings)
                    .HasForeignKey(e => e.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Food 關聯
                entity.HasOne(l => l.Food)
                    .WithMany(f => f.UserFoodSettings)
                    .HasForeignKey(e => e.foodID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            // UserRangeSettings 設定            
            modelBuilder.Entity<UserRangeSettings>(entity =>
            {
                entity.ToTable("UserRangeSettings");

                // 複合主鍵
                entity.HasKey(e => new { e.userID, e.rangeID });

                // User 關聯
                entity.HasOne(l => l.User)
                    .WithMany(u => u.UserRangeSettings)
                    .HasForeignKey(e => e.userID)
                    .OnDelete(DeleteBehavior.Restrict);

                // UserRange 關聯
                entity.HasOne(l => l.UserRange)
                    .WithMany(r => r.UserRangeSettings)
                    .HasForeignKey(e => e.rangeID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
