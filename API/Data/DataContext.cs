using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppTeacher> Teachers { get; set; }
        public DbSet<AppStudent> Students { get; set; }
    }
}