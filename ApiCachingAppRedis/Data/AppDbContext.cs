using ApiCachingAppRedis.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCachingAppRedis.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            
        }
    }
}
