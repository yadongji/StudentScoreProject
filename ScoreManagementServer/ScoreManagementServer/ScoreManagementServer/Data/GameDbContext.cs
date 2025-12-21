using Microsoft.EntityFrameworkCore;

namespace ScoreManagementServer.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) 
            : base(options)
        {
        }

        // 用户表
        public DbSet<UserData> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 Users 表
            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Username).IsUnique();
            });
        }
    }

    // 用户数据模型
    public class UserData
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}